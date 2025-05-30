using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private string configFileName;

    [Header("Dependencias Principales")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CardRegistrySO cardRegistry;

    // Estado del juego
    private Card _firstSelectedCard = null;
    private Card _secondSelectedCard = null;
    private bool _canSelect = false; // Inicia false hasta que la config cargue
    private int _pairsFound = 0;
    private int _totalPairs = 0;
    private bool gameRunning = false;
    private bool _isConfigLoadedAndValid = false;

    // Estadísticas de la partida
    private float _startTime = 0f;
    private int _totalClicks = 0;

    // Referencias UI - Juego
    [Header("UI del Juego")]
    [SerializeField] private TextMeshProUGUI timeTextUI;
    [SerializeField] private TextMeshProUGUI clicksTextUI;
    [SerializeField] private TextMeshProUGUI pairsTextUI;
    [SerializeField] private TextMeshProUGUI scoreTextUI;
    // [SerializeField] private GameObject gameOverPanel;

    // Referencias UI - Leaderboard y Entrada de Nombre
    [Header("UI del Leaderboard")]
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private GameObject nameEntryPanel;
    [SerializeField] private LeaderboardUI leaderboardUIScript;

    // Datos de la última partida para el leaderboard
    private float _lastGamePlayTime;
    private int _lastGameTotalClicks;
    private int _lastGameFinalScore;

    [Header("Audio SFX")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip selectCardSFX;
    [SerializeField] private AudioClip incorrectPairSFX;
    [SerializeField] private AudioClip correctPairSFX;
    // [SerializeField] private ParticleSystem victoryParticleEffect; // Si usas ParticleSystem en escena
    [SerializeField] private GameObject victoryParticlePrefab; // Si instancías prefab
    [SerializeField] private Transform particleSpawnPoint;


    [Header("Navegación de Escena")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    void Awake()
    {
        string defaultConfig = "gameConfig.json";
        configFileName = PlayerPrefs.GetString(DifficultySelector.DifficultyPlayerPrefKey, defaultConfig);
    }

    void Start()
    {
        if (leaderboardUIScript == null)
        {
            leaderboardUIScript = FindFirstObjectByType<LeaderboardUI>();
            if (leaderboardUIScript == null) Debug.LogWarning("GameManager: Script LeaderboardUI no encontrado.");
        }

        if (nameEntryPanel != null) nameEntryPanel.SetActive(false);

        if (gridManager == null) Debug.LogError("GameManager: GridManager no asignado.", this);
        if (cardRegistry == null) Debug.LogError("GameManager: CardRegistrySO no asignado.", this);
        if (sfxAudioSource == null)
        {
            sfxAudioSource = FindFirstObjectByType<AudioSource>(); // Intenta encontrar alguno
            if (sfxAudioSource == null) Debug.LogWarning("GameManager: SfxAudioSource no asignado y no se encontró uno en la escena.");
        }


        if (gridManager == null || cardRegistry == null)
        {
            enabled = false;
            return;
        }

        cardRegistry.InitializeLookup();
        StartCoroutine(LoadConfigurationAndInitializeGame());
    }

    void Update()
    {
        if (_isConfigLoadedAndValid && gameRunning && _canSelect && _pairsFound < _totalPairs)
        {
            UpdateTimeUI();
        }
    }

    private IEnumerator LoadConfigurationAndInitializeGame()
    {
        _isConfigLoadedAndValid = false;
        gameRunning = false;
        _canSelect = false;

        ConfigLoader loader = new ConfigLoader();
        GameConfig loadedConfig = null;

        yield return StartCoroutine(loader.LoadAndValidateConfigAsync(configFileName, cardRegistry,
            (configResult) => {
                loadedConfig = configResult;
            }
        ));

        if (loadedConfig != null)
        {
            _isConfigLoadedAndValid = true;
            SetupGameWithConfig(loadedConfig);
        }
        else
        {
            Debug.LogError($"GameManager: Falló la carga/validación de '{configFileName}'. El juego no puede iniciar.");
        }
    }

    private void SetupGameWithConfig(GameConfig config)
    {
        if (gridManager != null) gridManager.ClearGrid();

        bool gridCreated = gridManager.CreateGrid(config, OnCardSelected);
        if (gridCreated)
        {
            _totalPairs = config.blocks.Length / 2;
            _pairsFound = 0;
            _totalClicks = 0;
            _firstSelectedCard = null;
            _secondSelectedCard = null;
            _startTime = Time.time;

            InitializeUI();

            gameRunning = true;
            _canSelect = true;
        }
        else
        {
            Debug.LogError("GameManager: Falló la creación de la grilla con una configuración válida.");
            gameRunning = false;
        }
    }

    private void InitializeUI()
    {
        UpdateClicksUI();
        UpdatePairsUI();
        UpdateTimeUI();
        if (scoreTextUI != null) scoreTextUI.text = $"Puntaje: 0";
    }

    public void OnCardSelected(Card selectedCard)
    {
        if (!_isConfigLoadedAndValid || !_canSelect || selectedCard.IsMatched || selectedCard.IsRevealed) return;

        PlaySFX(selectCardSFX);
        _totalClicks++;
        UpdateClicksUI();
        selectedCard.RevealCard();

        if (_firstSelectedCard == null)
        {
            _firstSelectedCard = selectedCard;
        }
        else
        {
            _secondSelectedCard = selectedCard;
            _canSelect = false;
            CheckForMatch();
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("MainMenuSceneName no especificado en GameManager.");
        }
    }

    private void CheckForMatch()
    {
        if (_firstSelectedCard.CardType.id == _secondSelectedCard.CardType.id)
        {
            PlaySFX(correctPairSFX);
            _firstSelectedCard.SetMatched();
            _secondSelectedCard.SetMatched();
            _pairsFound++;
            UpdatePairsUI();
            if (_pairsFound == _totalPairs) EndGame();
            else ResetSelection(true);
        }
        else
        {
            PlaySFX(incorrectPairSFX);
            StartCoroutine(HideUnmatchedCards());
        }
    }

    private IEnumerator HideUnmatchedCards()
    {
        yield return new WaitForSeconds(1.0f);
        if (_firstSelectedCard != null) _firstSelectedCard.HideCard();
        if (_secondSelectedCard != null) _secondSelectedCard.HideCard();
        ResetSelection(false);
    }

    private void ResetSelection(bool pairWasFound)
    {
        _firstSelectedCard = null;
        _secondSelectedCard = null;
        _canSelect = true;
    }

    private void UpdateTimeUI()
    {
        if (timeTextUI == null) return;
        float elapsedTime = (gameRunning && _startTime > 0f) ? (Time.time - _startTime) : 0f;
        if (elapsedTime < 0f) elapsedTime = 0f;

        int minutes = (int)(elapsedTime / 60f);
        int seconds = (int)(elapsedTime % 60f);
        timeTextUI.text = $"Tiempo: {minutes:00}:{seconds:00}";
    }

    private void UpdateClicksUI()
    {
        if (clicksTextUI != null) clicksTextUI.text = $"Clicks: {_totalClicks}";
    }

    private void UpdatePairsUI()
    {
        if (pairsTextUI != null) pairsTextUI.text = $"Pares: {_pairsFound}/{_totalPairs}";
    }

    private void UpdateScoreUI(int score)
    {
        if (scoreTextUI != null) scoreTextUI.text = $"Puntaje: {score}";
    }

    public void RestartGame()
    {
        if (nameEntryPanel != null) nameEntryPanel.SetActive(false);
        // if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (leaderboardUIScript != null) leaderboardUIScript.Hide();

        // gridManager.ClearGrid() se llama dentro de SetupGameWithConfig
        StartCoroutine(LoadConfigurationAndInitializeGame());
    }

    private void EndGame()
    {
        gameRunning = false;
        _canSelect = false;
        float totalPlayTimeThisGame = (_startTime > 0f) ? (Time.time - _startTime) : 0f;
        int finalScoreThisGame = CalculateScore(totalPlayTimeThisGame, _totalClicks, _pairsFound);
        UpdateScoreUI(finalScoreThisGame);

        _lastGamePlayTime = totalPlayTimeThisGame;
        _lastGameTotalClicks = _totalClicks;
        _lastGameFinalScore = finalScoreThisGame;

        if (victoryParticlePrefab != null)
        {
            Vector3 spawnPosition = (particleSpawnPoint != null) ? particleSpawnPoint.position : this.transform.position;
            Quaternion spawnRotation = (particleSpawnPoint != null) ? particleSpawnPoint.rotation : Quaternion.identity;
            Instantiate(victoryParticlePrefab, spawnPosition, spawnRotation);
        }

        if (leaderboardManager != null)
        {
            if (nameEntryPanel != null && playerNameInputField != null)
            {
                nameEntryPanel.SetActive(true);
                playerNameInputField.text = "";
            }
            else
            {
                leaderboardManager.AddEntry("AAA", _lastGameFinalScore, _lastGameTotalClicks, Mathf.RoundToInt(_lastGamePlayTime));
                if (leaderboardUIScript != null) leaderboardUIScript.DisplayLeaderboard();
            }
        }
        else Debug.LogWarning("LeaderboardManager no encontrado. Puntaje no se guardará.");
    }

    public void SubmitScoreToLeaderboard()
    {
        if (leaderboardManager == null)
        {
            Debug.LogError("LeaderboardManager no asignado. No se puede enviar puntaje.");
            if (nameEntryPanel != null) nameEntryPanel.SetActive(false);
            return;
        }
        string playerName = "AAA";
        if (playerNameInputField != null && !string.IsNullOrWhiteSpace(playerNameInputField.text))
        {
            playerName = playerNameInputField.text;
        }
        else Debug.LogWarning("Nombre de jugador vacío o InputField no asignado. Usando AAA.");

        leaderboardManager.AddEntry(playerName, _lastGameFinalScore, _lastGameTotalClicks, Mathf.RoundToInt(_lastGamePlayTime));
        if (nameEntryPanel != null) nameEntryPanel.SetActive(false);

        if (leaderboardUIScript != null) leaderboardUIScript.DisplayLeaderboard();
        else Debug.LogWarning("LeaderboardUIScript no asignado.");
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource != null && clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }

    /*
    private void SaveResults(float time, int clicks, int pairs, int score)
    {
        ResultsData resultsDataInstance = new ResultsData {
            total_clicks = clicks, total_time = Mathf.RoundToInt(time),
            pairs = pairs, score = score
        };
        ResultsWrapper wrapper = new ResultsWrapper { results = resultsDataInstance };
        string jsonResults = JsonUtility.ToJson(wrapper, true);
        string resultsFilePath = Path.Combine(Application.persistentDataPath, "gameResults.json");
        try {
            File.WriteAllText(resultsFilePath, jsonResults);
        } catch (System.Exception e) {
            Debug.LogError($"Error guardando resultados individuales en '{resultsFilePath}': {e.Message}");
        }
    }
    */

    public static int CalculateScore(float time, int clicks, int pairs)
    {
        int score = (pairs * 1000) - (int)(time * 5) - (clicks * 10);
        return Mathf.Max(0, score);
    }
}

/*//
[System.Serializable]
*//*public class ResultsData
{
    public int total_clicks;
    public int total_time;
    public int pairs;
    public int score;
}

[System.Serializable]
public class ResultsWrapper
{
    public ResultsData results;
}*/
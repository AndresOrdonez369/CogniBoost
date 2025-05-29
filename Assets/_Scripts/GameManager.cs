using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using System.IO; 
using TMPro;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    // Nombre del archivo de configuración, se determina desde PlayerPrefs.
    private string configFileName;

    [Header("Dependencias Principales")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CardRegistrySO cardRegistry;

    // Estado del juego
    private Card _firstSelectedCard = null;
    private Card _secondSelectedCard = null;
    private bool _canSelect = true; // Controla si el jugador puede seleccionar cartas.
    private int _pairsFound = 0;
    private int _totalPairs = 0;
    private bool gameRunning = false; // Indica si una partida está activa.

    // Estadísticas de la partida
    private float _startTime = 0f;
    private int _totalClicks = 0;

    // Referencias UI - Juego
    [Header("UI del Juego")]
    [SerializeField] private TextMeshProUGUI timeTextUI;
    [SerializeField] private TextMeshProUGUI clicksTextUI;
    [SerializeField] private TextMeshProUGUI pairsTextUI;
    [SerializeField] private TextMeshProUGUI scoreTextUI;
    // [SerializeField] private GameObject gameOverPanel; // Opcional: Panel de resumen de fin de juego.

    // Referencias UI - Leaderboard y Entrada de Nombre
    [Header("UI del Leaderboard")]
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private GameObject nameEntryPanel;
    [SerializeField] private LeaderboardUI leaderboardUIScript; // Script que controla la UI del leaderboard.

    // Datos de la última partida para el leaderboard
    private float _lastGamePlayTime;
    private int _lastGameTotalClicks;
    private int _lastGameFinalScore;

    [Header("Audio SFX")]
    [Tooltip("El AudioSource que reproducirá los efectos de sonido.")]
    [SerializeField] private AudioSource sfxAudioSource;
    [Tooltip("Sonido al seleccionar una carta.")]
    [SerializeField] private AudioClip selectCardSFX;
    [Tooltip("Sonido cuando se forma un par incorrecto.")]
    [SerializeField] private AudioClip incorrectPairSFX;
    [Tooltip("Sonido cuando se forma un par correcto.")]
    [SerializeField] private AudioClip correctPairSFX;


    [Header("Navegación de Escena")]
    [Tooltip("Nombre de la escena de menú a la que volver.")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene"; // Reemplazar con el nombre real.

    void Awake()
    {
        string defaultConfig = "gameConfig.json"; // Configuración por defecto.
        configFileName = PlayerPrefs.GetString(DifficultySelector.DifficultyPlayerPrefKey, defaultConfig);
    }

    void Start()
    {
        if (leaderboardUIScript == null)
        {
            leaderboardUIScript = FindFirstObjectByType<LeaderboardUI>();
            if (leaderboardUIScript == null) Debug.LogWarning("GameManager: Script LeaderboardUI no encontrado en la escena.");
        }

        if (nameEntryPanel != null) nameEntryPanel.SetActive(false);

        if (gridManager == null) Debug.LogError("GameManager: GridManager no asignado.", this);
        if (cardRegistry == null) Debug.LogError("GameManager: CardRegistrySO no asignado.", this);

        if (gridManager == null || cardRegistry == null)
        {
            enabled = false; // Deshabilitar si faltan dependencias críticas.
            return;
        }

    

        cardRegistry.InitializeLookup(); // Prepara el registro de tipos de carta.
        LoadAndSetupGame(); // Carga la configuración inicial del juego.
                            // gameRunning y _canSelect se establecen al final de LoadAndSetupGame o RestartGame.


        if (sfxAudioSource == null)
        {
            // Intentar encontrar un AudioSource en un objeto llamado SFX_Player o en este mismo objeto
            GameObject sfxPlayerObject = GameObject.Find("SFX_Player");
            if (sfxPlayerObject != null)
            {
                sfxAudioSource = sfxPlayerObject.GetComponent<AudioSource>();
            }
            if (sfxAudioSource == null) // Si aún no se encuentra, buscar en este mismo GameObject
            {
                sfxAudioSource = GetComponent<AudioSource>();
            }
            if (sfxAudioSource == null) // Si sigue sin encontrarse, crear uno (o loggear error)
            {
                Debug.LogWarning("GameManager: No se encontró un AudioSource para SFX. Creando uno nuevo en este GameObject.");
                sfxAudioSource = gameObject.AddComponent<AudioSource>();
                sfxAudioSource.playOnAwake = false;
                sfxAudioSource.spatialBlend = 0f; // 2D
            }
        }
    }

    void Update()
    {
        // Actualizar el temporizador de la UI solo si el juego está activo.
        if (gameRunning && _canSelect && _pairsFound < _totalPairs)
        {
            UpdateTimeUI();
        }
    }

    // Carga la configuración del juego y crea la grilla.
    void LoadAndSetupGame()
    {
        ConfigLoader loader = new ConfigLoader();
        GameConfig config = loader.LoadAndValidateConfig(configFileName, cardRegistry);

        if (config != null)
        {
            bool gridCreated = gridManager.CreateGrid(config, OnCardSelected);
            if (gridCreated)
            {
                _totalPairs = config.blocks.Length / 2;
                _pairsFound = 0;
                _totalClicks = 0;
                _firstSelectedCard = null;
                _secondSelectedCard = null;
                _startTime = Time.time;

                InitializeUI(); // Resetea los textos de la UI del juego.

                gameRunning = true; // Marcar que el juego está listo y corriendo.
                _canSelect = true;  // Permitir selección de cartas.
                // Debug.Log($"Juego configurado con: {configFileName}");
            }
            else
            {
                gameRunning = false;
                Debug.LogError("GameManager: Falló la creación de la grilla.");
            }
        }
        else
        {
            gameRunning = false;
            Debug.LogError($"GameManager: Falló al cargar o validar la configuración: '{configFileName}'.");
        }
    }

    // Inicializa/Resetea los textos de la UI del juego.
    private void InitializeUI()
    {
        UpdateClicksUI();
        UpdatePairsUI();
        UpdateTimeUI();

        if (scoreTextUI != null)
        {
            scoreTextUI.text = $"Puntaje: 0";
        }
    }

    // Callback para cuando se selecciona una carta.
    public void OnCardSelected(Card selectedCard)
    {
        if (!_canSelect || selectedCard.IsMatched || selectedCard.IsRevealed) return;
        
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
            _canSelect = false; // Prevenir más selecciones hasta resolver el par.
            CheckForMatch();
        }
    }

    // Vuelve a la escena del menú principal.
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Asegurar que el tiempo esté normal.

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("MainMenuSceneName no especificado. No se puede volver al menú.");
        }
    }

    // Verifica si las dos cartas seleccionadas forman un par.
    private void CheckForMatch()
    {
        if (_firstSelectedCard.CardType.id == _secondSelectedCard.CardType.id)
        {
            PlaySFX(correctPairSFX);
            _firstSelectedCard.SetMatched();
            _secondSelectedCard.SetMatched();
            _pairsFound++;
            UpdatePairsUI();
            // Debug.Log($"Par encontrado! ID: {_firstSelectedCard.CardType.id}. Pares: {_pairsFound}/{_totalPairs}");

            if (_pairsFound == _totalPairs)
            {
                EndGame();
            }
            else
            {
                ResetSelection(true); // Par encontrado, preparar para siguiente selección.
            }
        }
        else
        {
            PlaySFX(incorrectPairSFX);
            StartCoroutine(HideUnmatchedCards());
        }
    }

    // Corrutina para ocultar cartas no emparejadas después de un breve retraso.
    private IEnumerator HideUnmatchedCards()
    {
        
        yield return new WaitForSeconds(1.0f);
        if (_firstSelectedCard != null) _firstSelectedCard.HideCard();
        if (_secondSelectedCard != null) _secondSelectedCard.HideCard();
        ResetSelection(false); // No fue par.
    }

    // Resetea las cartas seleccionadas.
    private void ResetSelection(bool pairWasFound)
    {
        _firstSelectedCard = null;
        _secondSelectedCard = null;
        _canSelect = true;
    }

    // Actualiza el texto del tiempo en la UI.
    private void UpdateTimeUI()
    {
        if (timeTextUI == null) return;
        float elapsedTime = (gameRunning && _startTime > 0f) ? (Time.time - _startTime) : 0f;
        if (elapsedTime < 0f) elapsedTime = 0f; // Seguridad.

        int minutes = (int)(elapsedTime / 60f);
        int seconds = (int)(elapsedTime % 60f);
        timeTextUI.text = $"Tiempo: {minutes:00}:{seconds:00}";
    }

    // Actualiza el texto de clics en la UI.
    private void UpdateClicksUI()
    {
        if (clicksTextUI != null) clicksTextUI.text = $"Clicks: {_totalClicks}";
    }

    // Actualiza el texto de pares en la UI.
    private void UpdatePairsUI()
    {
        if (pairsTextUI != null) pairsTextUI.text = $"Pares: {_pairsFound}/{_totalPairs}";
    }

    // Actualiza el texto del puntaje en la UI.
    private void UpdateScoreUI(int score)
    {
        if (scoreTextUI != null) scoreTextUI.text = $"Puntaje: {score}";
    }

    // Reinicia el juego para una nueva partida (misma dificultad).
    public void RestartGame()
    {
        // Debug.Log("Reiniciando juego...");

        if (nameEntryPanel != null) nameEntryPanel.SetActive(false);
        // if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (leaderboardUIScript != null) leaderboardUIScript.Hide();

        if (gridManager != null)
        {
            gridManager.ClearGrid();
        }
        else
        {
            Debug.LogError("GridManager no asignado. No se puede limpiar la grilla para reiniciar.");
        }

        LoadAndSetupGame(); // Recarga configuración y resetea el estado del juego.
        // gameRunning y _canSelect son establecidos por LoadAndSetupGame.
    }

    // Se ejecuta cuando todos los pares han sido encontrados.
    private void EndGame()
    {
        gameRunning = false;
        _canSelect = false;

        float totalPlayTimeThisGame = (_startTime > 0f) ? (Time.time - _startTime) : 0f;
        int finalScoreThisGame = CalculateScore(totalPlayTimeThisGame, _totalClicks, _pairsFound);

        // Debug.Log($"Fin del Juego! Tiempo: {totalPlayTimeThisGame:F2}s, Clicks: {_totalClicks}, Score: {finalScoreThisGame}");
        UpdateScoreUI(finalScoreThisGame);

        _lastGamePlayTime = totalPlayTimeThisGame;
        _lastGameTotalClicks = _totalClicks;
        _lastGameFinalScore = finalScoreThisGame;



        if (leaderboardManager != null)
        {
            if (nameEntryPanel != null && playerNameInputField != null)
            {
                nameEntryPanel.SetActive(true);
                playerNameInputField.text = ""; // Limpiar nombre previo.
                // Opcional: playerNameInputField.Select(); playerNameInputField.ActivateInputField();
            }
            else
            {
                // Debug.Log("Panel de entrada de nombre no configurado. Guardando en leaderboard con nombre por defecto.");
                leaderboardManager.AddEntry("AAA", _lastGameFinalScore, _lastGameTotalClicks, Mathf.RoundToInt(_lastGamePlayTime));
                if (leaderboardUIScript != null) leaderboardUIScript.DisplayLeaderboard();
            }
        }
        else
        {
            Debug.LogWarning("LeaderboardManager no encontrado. El puntaje no se guardará.");
        }
    }

    // Envía el puntaje al leaderboard después de que el jugador ingresa su nombre.
    public void SubmitScoreToLeaderboard()
    {
        if (leaderboardManager == null)
        {
            Debug.LogError("LeaderboardManager no asignado. No se puede enviar el puntaje.");
            if (nameEntryPanel != null) nameEntryPanel.SetActive(false);
            return;
        }

        string playerName = "AAA"; // Nombre por defecto.
        if (playerNameInputField != null && !string.IsNullOrWhiteSpace(playerNameInputField.text))
        {
            playerName = playerNameInputField.text;
        }
        else
        {
            Debug.LogWarning("Nombre de jugador vacío o InputField no asignado. Usando nombre por defecto 'AAA'.");
        }

        leaderboardManager.AddEntry(playerName, _lastGameFinalScore, _lastGameTotalClicks, Mathf.RoundToInt(_lastGamePlayTime));

        if (nameEntryPanel != null) nameEntryPanel.SetActive(false);
        // if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (leaderboardUIScript != null)
        {
            leaderboardUIScript.DisplayLeaderboard();
        }
        else
        {
            Debug.LogWarning("LeaderboardUIScript no asignado. No se puede mostrar UI del leaderboard.");
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource != null && clip != null)
        {
           
            sfxAudioSource.PlayOneShot(clip);
        }
    }


    // Método para guardar resultados individuales (actualmente no llamado desde otra parte del script).

    /* private void SaveResults(float time, int clicks, int pairs, int score)
     {
         ResultsData resultsDataInstance = new ResultsData
         {
             total_clicks = clicks,
             total_time = Mathf.RoundToInt(time),
             pairs = pairs,
             score = score
         };
         ResultsWrapper wrapper = new ResultsWrapper { results = resultsDataInstance };
         string jsonResults = JsonUtility.ToJson(wrapper, true);
         string resultsFilePath = Path.Combine(Application.persistentDataPath, "gameResults.json");

         try
         {
             File.WriteAllText(resultsFilePath, jsonResults);
             // Debug.Log($"Resultados individuales guardados en: {resultsFilePath}");
         }
         catch (System.Exception e)
         {
             Debug.LogError($"Error al guardar resultados individuales en '{resultsFilePath}': {e.Message}");
         }
     }*/

    // Calcula el puntaje basado en tiempo, clics y pares.
    private int CalculateScore(float time, int clicks, int pairs)
    {
        int score = (pairs * 1000) - (int)(time * 5) - (clicks * 10);
        return Mathf.Max(0, score); // Prevenir puntaje negativo.
    }
}

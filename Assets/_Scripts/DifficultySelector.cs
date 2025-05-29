// DifficultySelector.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Para cargar la escena del juego
using UnityEngine.UI; // Si necesitas referencias a botones para añadir listeners por código

public class DifficultySelector : MonoBehaviour
{
    [Header("Scene Management")]
    [Tooltip("El nombre de la escena principal del juego de memoria.")]
    [SerializeField] private string gameSceneName = "MemoryGameScene";

 
    public const string DifficultyPlayerPrefKey = "SelectedDifficulty";
    public const string ConfigFileEasy = "gameConfig.json";   
    public const string ConfigFileMedium = "gameConfig_3x4.json";
    public const string ConfigFileHard = "gameConfig_4x5.json";

 
    public void OnSelectEasyDifficulty()
    {
        Debug.Log("Dificultad Fácil seleccionada.");
        SetDifficultyAndLoadGame(ConfigFileEasy);
    }

    public void OnSelectMediumDifficulty()
    {
        Debug.Log("Dificultad Media seleccionada.");
        SetDifficultyAndLoadGame(ConfigFileMedium);
    }

    public void OnSelectHardDifficulty()
    {
        Debug.Log("Dificultad Difícil seleccionada.");
        SetDifficultyAndLoadGame(ConfigFileHard);
    }

    private void SetDifficultyAndLoadGame(string configFileNameToLoad)
    {
 
        PlayerPrefs.SetString(DifficultyPlayerPrefKey, configFileNameToLoad);
        PlayerPrefs.Save(); // Asegurar que se guarde la escena con playerPrefs
        Debug.Log($"Dificultad '{configFileNameToLoad}' guardada en PlayerPrefs.");

        // Se cargaa la escena del juego
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            Debug.Log($"Cargando escena del juego: {gameSceneName}");
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("Nombre de la escena del juego no especificado en DifficultySelector.");
        }
    }
}
// MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Para cargar escenas si "Iniciar" carga otra escena


public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("El panel principal del menú con los botones Iniciar, Créditos, Salir.")]
    [SerializeField] private GameObject mainMenuPanel;

    [Tooltip("El panel que muestra la información de créditos.")]
    [SerializeField] private GameObject creditosPanel;

    [Header("Scene Management")]
    [SerializeField] private string gameSceneName = "GameScene";

    void Start()
    {
        // Asegurarse de que el estado inicial de los paneles sea el correcto
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("MainMenuPanel no está asignado en MainMenuManager.");
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("CreditosPanel no está asignado en MainMenuManager.");
        }
    }

    // --- Métodos para los botones del MainMenuPanel 

    public void OnIniciarClicked()
    {
       SceneManager.LoadScene(gameSceneName);
    }

    public void OnCreditosClicked()
    {
        Debug.Log("Botón Créditos presionado.");
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        if (creditosPanel != null)
        {
            creditosPanel.SetActive(true);
        }
    }

    public void OnSalirClicked()
    {
        Debug.Log("Botón Salir presionado.");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- Método para el botón del CreditosPanel

    public void OnVolverDesdeCreditosClicked()
    {
        Debug.Log("Botón Volver desde Créditos presionado.");
        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }
}
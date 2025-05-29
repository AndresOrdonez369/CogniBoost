// MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Para cargar escenas si "Iniciar" carga otra escena


public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("El panel principal del men� con los botones Iniciar, Cr�ditos, Salir.")]
    [SerializeField] private GameObject mainMenuPanel;

    [Tooltip("El panel que muestra la informaci�n de cr�ditos.")]
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
            Debug.LogError("MainMenuPanel no est� asignado en MainMenuManager.");
        }

        if (creditosPanel != null)
        {
            creditosPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("CreditosPanel no est� asignado en MainMenuManager.");
        }
    }

    // --- M�todos para los botones del MainMenuPanel 

    public void OnIniciarClicked()
    {
       SceneManager.LoadScene(gameSceneName);
    }

    public void OnCreditosClicked()
    {
        Debug.Log("Bot�n Cr�ditos presionado.");
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
        Debug.Log("Bot�n Salir presionado.");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- M�todo para el bot�n del CreditosPanel

    public void OnVolverDesdeCreditosClicked()
    {
        Debug.Log("Bot�n Volver desde Cr�ditos presionado.");
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
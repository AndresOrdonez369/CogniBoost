using UnityEngine;
using UnityEngine.UI; // Para Button, si se usa
using TMPro; // Para TextMeshProUGUI
using System.Collections.Generic;
using System.Linq; // Para .Take()

public class LeaderboardUI : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Referencia al gestor principal del leaderboard.")]
    [SerializeField] private LeaderboardManager leaderboardManager;
    [Tooltip("El panel UI que contiene todos los elementos del leaderboard.")]
    [SerializeField] private GameObject leaderboardDisplayPanel;
    [Tooltip("El objeto padre (con un LayoutGroup) donde se instancian las filas del leaderboard.")]
    [SerializeField] private Transform leaderboardContentParent;
    [Tooltip("El prefab para una fila individual del leaderboard.")]
    [SerializeField] private GameObject leaderboardRowPrefab;


    void Start()
    {
        // Intentar obtener el LeaderboardManager si no está asignado en el Inspector.
        if (leaderboardManager == null)
        {
            // Debug.LogError("LeaderboardManager no asignado en LeaderboardUI. Intentando encontrarlo...");
            leaderboardManager = FindFirstObjectByType<LeaderboardManager>();
            if (leaderboardManager == null)
            {
                Debug.LogError("LeaderboardManager no se pudo encontrar. LeaderboardUI no funcionará.");
                if (leaderboardDisplayPanel != null) leaderboardDisplayPanel.SetActive(false);
                enabled = false; // Deshabilitar este script si no puede funcionar.
                return;
            }
        }

        // Asegurarse de que el panel del leaderboard esté oculto al inicio.
        if (leaderboardDisplayPanel != null)
        {
            leaderboardDisplayPanel.SetActive(false);
        }

    
    }

    // Muestra el panel del leaderboard y lo puebla con datos.
    public void Show()
    {
        if (leaderboardManager == null)
        {
            Debug.LogError("No se puede mostrar leaderboard: LeaderboardManager no disponible.");
            return;
        }
        if (leaderboardDisplayPanel == null)
        {
            Debug.LogError("No se puede mostrar leaderboard: leaderboardDisplayPanel no asignado.");
            return;
        }

        leaderboardDisplayPanel.SetActive(true);
        PopulateLeaderboard();
    }

    // Oculta el panel del leaderboard.
    public void Hide()
    {
        if (leaderboardDisplayPanel != null)
        {
            leaderboardDisplayPanel.SetActive(false);
        }
    }

    // Limpia las entradas existentes y crea nuevas filas con los datos del leaderboard.
    private void PopulateLeaderboard()
    {
        if (leaderboardContentParent == null || leaderboardRowPrefab == null)
        {
            Debug.LogError("LeaderboardContentParent o LeaderboardRowPrefab no asignados. No se puede poblar el leaderboard.");
            return;
        }

        // Limpiar filas antiguas.
        foreach (Transform child in leaderboardContentParent)
        {
            Destroy(child.gameObject);
        }

        List<LeaderboardEntry> topEntries = leaderboardManager.GetTopEntries();

        if (topEntries.Count == 0)
        {
            return;
        }

        // Crear una fila por cada entrada en el top.
        for (int i = 0; i < topEntries.Count; i++)
        {
            GameObject rowGO = Instantiate(leaderboardRowPrefab, leaderboardContentParent);
            LeaderboardRowUI rowUI = rowGO.GetComponent<LeaderboardRowUI>();
            if (rowUI != null)
            {
                rowUI.Setup(i + 1, topEntries[i].playerName, topEntries[i].score);
            }
            else
            {
                Debug.LogError("El Prefab de Fila del Leaderboard no tiene el script LeaderboardRowUI.", rowGO);
            }
        }
    }

    // Método público llamado por GameManager para mostrar el leaderboard.
    public void DisplayLeaderboard()
    {
        Show();
    }
}
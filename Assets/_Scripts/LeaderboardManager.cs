using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Para OrderByDescending y Take

public class LeaderboardManager : MonoBehaviour
{
    private const string LeaderboardFileName = "leaderboard.json";
    private const int MaxEntriesToShow = 8; // Cuántos scores mostrar en el leaderboard UI

    private LeaderboardData _leaderboardData;

    void Awake()
    {
        LoadLeaderboard();
    }

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, LeaderboardFileName);
    }

    public void LoadLeaderboard()
    {
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                _leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
                if (_leaderboardData == null || _leaderboardData.entries == null)
                {
                    // Archivo corrupto o vacío, inicializar
                    _leaderboardData = new LeaderboardData();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading leaderboard: {e.Message}. Initializing new leaderboard.");
                _leaderboardData = new LeaderboardData();
            }
        }
        else
        {
            _leaderboardData = new LeaderboardData();
        }

      
        SortEntries();
        Debug.Log("Leaderboard loaded. Entries: " + _leaderboardData.entries.Count);
    }

    public void SaveLeaderboard()
    {
        SortEntries(); // Asegurar que esté ordenado antes de guardar

        try
        {
            string json = JsonUtility.ToJson(_leaderboardData, true);
            File.WriteAllText(GetFilePath(), json);
            Debug.Log("Leaderboard saved.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving leaderboard: {e.Message}");
        }
    }

    public void AddEntry(string playerName, int score, int clicks, int time)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Anonymous"; // O un nombre por defecto más descriptivo
        }
      /*  playerName = playerName.ToUpper().Substring(0, Mathf.Min(playerName.Length, 3)); // Arcade style 3 letras
*/
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score, clicks, time);
        _leaderboardData.entries.Add(newEntry);
        SortEntries(); // Ordenar después de añadir
        SaveLeaderboard(); // Guardar cambios
    }

    private void SortEntries()
    {
        if (_leaderboardData != null && _leaderboardData.entries != null)
        {
            // Ordenar por score descendente, luego por tiempo ascendente (mejor tiempo), luego por clicks ascendente
            _leaderboardData.entries = _leaderboardData.entries
                .OrderByDescending(entry => entry.score)
                .ThenBy(entry => entry.total_time)
                .ThenBy(entry => entry.total_clicks)
                .ToList();
        }
    }

    public List<LeaderboardEntry> GetTopEntries()
    {
        if (_leaderboardData == null || _leaderboardData.entries == null)
        {
            return new List<LeaderboardEntry>(); // Devolver lista vacía si no hay datos
        }
        // Devuelve solo las MaxEntriesToShow mejores o todas si hay menos
        return _leaderboardData.entries.Take(MaxEntriesToShow).ToList();
    }

    public void ClearLeaderboard() // Función de utilidad para desarrollo/pruebas
    {
        _leaderboardData = new LeaderboardData();
        SaveLeaderboard();
        Debug.Log("Leaderboard cleared.");
    }
}
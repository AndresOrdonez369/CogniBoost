using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic; 
using System.Linq; 

public class ConfigLoader
{
    public IEnumerator LoadAndValidateConfigAsync(string fileName, CardRegistrySO cardRegistry, System.Action<GameConfig> callback)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        GameConfig gameConfig = null;
        string dataAsJson = null;

#if UNITY_WEBGL && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            dataAsJson = www.downloadHandler.text;
        }
        else
        {
            Debug.LogError($"ConfigLoader (WebGL): Failed to load '{fileName}'. Error: {www.error} from: {filePath}");
            callback?.Invoke(null);
            yield break;
        }
#else
        if (!File.Exists(filePath))
        {
            Debug.LogError($"ConfigLoader: Cannot find config file: {filePath}");
            callback?.Invoke(null);
            yield break;
        }
        try
        {
            dataAsJson = File.ReadAllText(filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ConfigLoader: Error reading '{fileName}'. Exception: {e.Message}");
            callback?.Invoke(null);
            yield break;
        }
#endif

        if (!string.IsNullOrEmpty(dataAsJson))
        {
            try
            {
                gameConfig = JsonUtility.FromJson<GameConfig>(dataAsJson);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ConfigLoader: Error parsing JSON from '{fileName}'. Exception: {e.Message}");
                // gameConfig permanecerá null
            }
        }
        else
        {
            Debug.LogError($"ConfigLoader: Loaded JSON data is null or empty for '{fileName}'.");
        }

        if (gameConfig != null && !IsValidConfig(gameConfig, cardRegistry))
        {
            Debug.LogError($"ConfigLoader: Config from '{fileName}' is invalid.");
            gameConfig = null;
        }

        callback?.Invoke(gameConfig);
    }

    public bool IsValidConfig(GameConfig config, CardRegistrySO cardRegistry)
    {
        if (config == null) { Debug.LogError("Validation: Config object is null."); return false; }
        if (config.blocks == null) { Debug.LogError("Validation: Config.blocks array is null."); return false; }
        if (config.blocks.Length == 0) { Debug.LogError("Validation: Config.blocks is empty."); return false; }
        if (config.blocks.Length % 2 != 0) { Debug.LogError($"Validation: Odd number of blocks ({config.blocks.Length})."); return false; }
        if (cardRegistry == null) { Debug.LogError("Validation: CardRegistrySO is null."); return false; }

        int maxR = 0;
        int maxC = 0;
        var valueCounts = new Dictionary<int, int>();
        var positions = new HashSet<Vector2Int>();

        foreach (var block in config.blocks)
        {
            if (block == null) { Debug.LogError("Validation: Null block entry found."); return false; }
            if (block.R <= 0 || block.C <= 0) { Debug.LogError($"Validation: Invalid R({block.R}) or C({block.C}). Must be 1-based positive."); return false; }
            if (block.R > maxR) maxR = block.R;
            if (block.C > maxC) maxC = block.C;
            if (cardRegistry.GetCardTypeById(block.number) == null) { Debug.LogError($"Validation: Block number {block.number} (R:{block.R},C:{block.C}) not in registry."); return false; }

            if (valueCounts.ContainsKey(block.number)) valueCounts[block.number]++;
            else valueCounts[block.number] = 1;

            Vector2Int currentPos = new Vector2Int(block.R, block.C);
            if (positions.Contains(currentPos)) { Debug.LogError($"Validation: Duplicate position (R:{block.R},C:{block.C})."); return false; }
            positions.Add(currentPos);
        }

        if (maxR < 2 || maxR > 8 || maxC < 2 || maxC > 8) { Debug.LogError($"Validation: Grid dimensions (R:{maxR},C:{maxC}) out of 2-8 bounds."); return false; }
        foreach (var pairCount in valueCounts)
        {
            if (pairCount.Value != 2) { Debug.LogError($"Validation: CardType ID {pairCount.Key} appears {pairCount.Value} times. Must be 2."); return false; }
        }
        // Debug.Log("Config validation successful."); // Mantenido o comentado según preferencia
        return true;
    }
}
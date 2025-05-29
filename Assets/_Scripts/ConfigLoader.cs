using UnityEngine;
using System.IO;
using System.Collections.Generic;
// System.Linq no se usa directamente en este script después de la pulida,
// pero no daña si se mantiene por si se añade alguna funcionalidad que lo requiera.
// Si quieres ser estricto, podrías eliminarlo. Por ahora lo dejaré.
using System.Linq;

public class ConfigLoader
{
    public GameConfig LoadAndValidateConfig(string fileName, CardRegistrySO cardRegistry)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        GameConfig gameConfig = null;

        if (!File.Exists(filePath))
        {
            Debug.LogError($"ConfigLoader: Cannot find config file at path: {filePath}");
            return null;
        }

        try
        {
            string dataAsJson = File.ReadAllText(filePath);
            gameConfig = JsonUtility.FromJson<GameConfig>(dataAsJson);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ConfigLoader: Error parsing JSON from file '{fileName}'. Exception: {e.Message}");
            return null;
        }

        if (!IsValidConfig(gameConfig, cardRegistry))
        {
            // IsValidConfig ya registra errores específicos.
            Debug.LogError($"ConfigLoader: Loaded configuration from '{fileName}' is invalid. Check previous errors for details.");
            return null;
        }
        return gameConfig;
    }

    private bool IsValidConfig(GameConfig config, CardRegistrySO cardRegistry)
    {
        if (config == null)
        {
            Debug.LogError("Validation Error: GameConfig object is null (JSON might be empty or malformed).");
            return false;
        }
        if (config.blocks == null)
        {
            Debug.LogError("Validation Error: GameConfig.blocks array is null (JSON 'blocks' field might be missing or malformed).");
            return false;
        }

        if (config.blocks.Length == 0)
        {
            Debug.LogError("Validation Error: config.blocks array is empty. No card data provided.");
            return false;
        }

        if (config.blocks.Length % 2 != 0)
        {
            Debug.LogError($"Validation Error: Number of blocks is {config.blocks.Length}, which is odd. Must be an even number to form pairs.");
            return false;
        }

        if (cardRegistry == null)
        {
            Debug.LogError("Validation Error: CardRegistrySO is null. Cannot validate block numbers.");
            return false;
        }

        int maxR = 0;
        int maxC = 0;
        var valueCounts = new Dictionary<int, int>();
        var positions = new HashSet<Vector2Int>();

        foreach (var block in config.blocks)
        {
            if (block == null)
            {
                Debug.LogError("Validation Error: Found a null entry within the config.blocks array.");
                return false;
            }

            if (block.R <= 0 || block.C <= 0)
            {
                Debug.LogError($"Validation Error: Invalid R ({block.R}) or C ({block.C}) for a block. Row and Column must be positive (1-based).");
                return false;
            }
            if (block.R > maxR) maxR = block.R;
            if (block.C > maxC) maxC = block.C;

            if (cardRegistry.GetCardTypeById(block.number) == null)
            {
                Debug.LogError($"Validation Error: Block 'number' {block.number} at (R:{block.R}, C:{block.C}) does not correspond to any registered CardTypeSO.");
                return false;
            }
            // Restricción de 0-9 para 'number' debe ser estricta
            if (block.number < 0 || block.number > 9) {
                Debug.LogError($"Validation Error: Block 'number' {block.number} at (R:{block.R}, C:{block.C}) is outside the allowed 0-9 range.");
                return false;
            }

            if (valueCounts.ContainsKey(block.number))
                valueCounts[block.number]++;
            else
                valueCounts[block.number] = 1;

            Vector2Int currentPos = new Vector2Int(block.R, block.C);
            if (positions.Contains(currentPos))
            {
                Debug.LogError($"Validation Error: Duplicate position (R:{block.R}, C:{block.C}) found in config.");
                return false;
            }
            positions.Add(currentPos);
        }

        // X (columnas) e Y (filas) no deben ser menores a 2 ni mayores a 8.
        if (maxR < 2 || maxR > 8 || maxC < 2 || maxC > 8)
        {
            Debug.LogError($"Validation Error: Effective grid dimensions (Rows: {maxR}, Columns: {maxC}) are out of bounds. Both must be between 2 and 8, inclusive.");
            return false;
        }

        foreach (var pairCount in valueCounts)
        {
            if (pairCount.Value != 2)
            {
                Debug.LogError($"Validation Error: CardType ID {pairCount.Key} appears {pairCount.Value} times. Each ID must appear exactly twice to form a pair.");
                return false;
            }
        }

        Debug.Log("Config validation successful.");
        return true;
    }
}
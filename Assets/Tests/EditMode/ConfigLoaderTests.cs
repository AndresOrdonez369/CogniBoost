// ConfigLoaderTests.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools; 
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ConfigLoaderTests
{
    private ConfigLoader _loader;
    private CardRegistrySO _mockRegistry;
    private List<CardTypeSO> _mockCardTypesList;

    [SetUp]
    public void SetupForEachTest()
    {
        _loader = new ConfigLoader();
        _mockRegistry = ScriptableObject.CreateInstance<CardRegistrySO>();
        _mockCardTypesList = new List<CardTypeSO>();

        for (int i = 0; i < 3; i++) // IDs 0, 1, 2
        {
            CardTypeSO cardType = ScriptableObject.CreateInstance<CardTypeSO>();
            cardType.id = i;
            _mockCardTypesList.Add(cardType);
        }
        _mockRegistry.cardTypes = _mockCardTypesList;
        _mockRegistry.InitializeLookup();
    }

    [TearDown]
    public void TeardownAfterEachTest()
    {
        Object.DestroyImmediate(_mockRegistry);
        foreach (var ct in _mockCardTypesList)
        {
            Object.DestroyImmediate(ct);
        }
        _mockCardTypesList.Clear();
        _loader = null;
    }

    [Test]
    public void IsValidConfig_WithValidMinimal2x2Config_ReturnsTrue()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 2, C = 1, number = 0 }, new BlockData { R = 2, C = 2, number = 1 },
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 1, C = 2, number = 1 }
            }
        };
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsTrue(isValid, "Una configuraci�n 2x2 v�lida deber�a ser reconocida como v�lida.");
    }

    [Test]
    public void IsValidConfig_WithOddNumberOfBlocks_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 1, C = 2, number = 1 },
                new BlockData { R = 2, C = 1, number = 0 }
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("Number of blocks is .* odd"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n con n�mero impar de bloques deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_WithNumberNotInRegistry_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 2, C = 1, number = 0 }, new BlockData { R = 2, C = 2, number = 5 }, // ID 5 no en mockRegistry
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 1, C = 2, number = 5 }
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("does not correspond to any registered CardTypeSO"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n con un 'number' no registrado deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_WithNumberAppearingOnce_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 2, C = 1, number = 0 }, new BlockData { R = 2, C = 2, number = 1 }, // Solo un 1
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 1, C = 2, number = 2 }  // Solo un 2
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("CardType ID .* appears 1 times"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n donde un 'number' aparece solo una vez deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_WithNumberAppearingThrice_ReturnsFalse()
    {
        GameConfig configThrice = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 1, C = 2, number = 0 },
                new BlockData { R = 1, C = 3, number = 0 }, // Tres 0s
                new BlockData { R = 2, C = 1, number = 1 }, new BlockData { R = 2, C = 2, number = 1 }, // Dos 1s para paridad de bloques
                new BlockData { R = 2, C = 3, number = 2 }  // Placeholder, para que no falle por "aparece una vez"
            }
        };
        // Necesitamos asegurar que el n�mero total de bloques sea par, y que otro n�mero no falle primero.
        // Mejor crear un config donde un n�mero aparece 3 veces y otro 1 vez, lo cual fallar� por el de 1 vez
        // o por el de 3 veces. La prueba de arriba ya cubre el de 1 vez.
        // Re-enfocando: Si un n�mero aparece 3 veces, otro DEBE aparecer un n�mero impar de veces tambi�n.
        // La validaci�n "Each ID must appear exactly twice" es la que saltar�.
        GameConfig configActualThrice = new GameConfig
        {
            blocks = new BlockData[] { // Total 4 bloques.
                new BlockData { R = 1, C = 1, number = 0 }, // 0 aparece 3 veces
                new BlockData { R = 1, C = 2, number = 0 },
                new BlockData { R = 2, C = 1, number = 0 },
                new BlockData { R = 2, C = 2, number = 1 }  // 1 aparece 1 vez
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("CardType ID .* appears .* times. Each ID must appear exactly twice"));
        bool isValid = _loader.IsValidConfig(configActualThrice, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n donde un 'number' no aparece dos veces deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_GridTooSmall_RowsLessThan2_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] { // 1 Fila, 2 Columnas
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 1, C = 2, number = 0 }
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("Effective grid dimensions .* are out of bounds"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n con menos de 2 filas deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_GridTooSmall_ColsLessThan2_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] { // 2 Filas, 1 Columna
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 2, C = 1, number = 0 }
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("Effective grid dimensions .* are out of bounds"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n con menos de 2 columnas deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_GridTooLarge_RowsGreaterThan8_ReturnsFalse()
    {
        var blocks = new List<BlockData>();
        for (int r = 1; r <= 9; r++)
        { // 9 filas
            blocks.Add(new BlockData { R = r, C = 1, number = (r - 1) % _mockCardTypesList.Count });
            blocks.Add(new BlockData { R = r, C = 2, number = (r - 1) % _mockCardTypesList.Count });
        }
        GameConfig config = new GameConfig { blocks = blocks.ToArray() };
        LogAssert.Expect(LogType.Error, new Regex("Effective grid dimensions .* are out of bounds"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n con m�s de 8 filas deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_DuplicatePositions_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 2, C = 1, number = 0 }, new BlockData { R = 2, C = 2, number = 1 },
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 2, C = 1, number = 1 } // Pos (2,1) duplicada
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("Duplicate position .* found in config"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n con posiciones (R,C) duplicadas deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_ConfigIsNull_ReturnsFalse()
    {
        LogAssert.Expect(LogType.Error, new Regex("GameConfig object is null"));
        bool isValid = _loader.IsValidConfig(null, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n nula deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_BlocksArrayIsNull_ReturnsFalse()
    {
        GameConfig config = new GameConfig { blocks = null };
        LogAssert.Expect(LogType.Error, new Regex("GameConfig.blocks array is null"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n con array de bloques nulo deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_BlocksArrayIsEmpty_ReturnsFalse()
    {
        GameConfig config = new GameConfig { blocks = new BlockData[0] };
        LogAssert.Expect(LogType.Error, new Regex("config.blocks array is empty"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n con array de bloques vac�o deber�a ser inv�lida.");
    }

    [Test]
    public void IsValidConfig_BlockEntryIsNull_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 2, C = 1, number = 0 }, null,
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 1, C = 2, number = 1 }
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("Found a null entry within the config.blocks array"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuraci�n con una entrada de bloque nula deber�a ser inv�lida.");
    }
}
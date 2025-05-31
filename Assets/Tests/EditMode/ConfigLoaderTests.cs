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
        if (_mockRegistry != null) Object.DestroyImmediate(_mockRegistry);
        if (_mockCardTypesList != null)
        {
            foreach (var ct in _mockCardTypesList)
            {
                if (ct != null) Object.DestroyImmediate(ct);
            }
            _mockCardTypesList.Clear();
        }
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
        // No se espera LogError para una config válida
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsTrue(isValid, "Una configuración 2x2 válida debería ser reconocida como válida.");
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
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Odd number of blocks \\(3\\)\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración con número impar de bloques debería ser inválida.");
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
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Block number 5 \\(R:2,C:2\\) not in registry\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración con un 'number' no registrado debería ser inválida.");
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
        // El error puede ser para el ID 1 o el ID 2, dependiendo del orden del diccionario.
        // Usamos una Regex que cubra ambos o aceptamos el primero que falle.
        // Si el ID 1 falla primero:
        LogAssert.Expect(LogType.Error, new Regex("^Validation: CardType ID (1|2) appears 1 times\\. Must be 2\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración donde un 'number' aparece solo una vez debería ser inválida.");
    }

    [Test]
    public void IsValidConfig_WithNumberAppearingThrice_ReturnsFalse()
    {
        GameConfig configActualThrice = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 1, C = 1, number = 0 },
                new BlockData { R = 1, C = 2, number = 0 },
                new BlockData { R = 2, C = 1, number = 0 }, // 0 aparece 3 veces
                new BlockData { R = 2, C = 2, number = 1 }  // 1 aparece 1 vez
            }
        };
        // El primer error detectado será que el ID 0 aparece 3 veces o que el ID 1 aparece 1 vez.
        LogAssert.Expect(LogType.Error, new Regex("^Validation: CardType ID (0|1) appears (1|3) times\\. Must be 2\\.$"));
        bool isValid = _loader.IsValidConfig(configActualThrice, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración donde un 'number' no aparece dos veces debería ser inválida.");
    }

    [Test]
    public void IsValidConfig_GridTooSmall_RowsLessThan2_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 1, C = 2, number = 0 }
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Grid dimensions \\(R:1,C:2\\) out of 2-8 bounds\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración con menos de 2 filas debería ser inválida.");
    }

    [Test]
    public void IsValidConfig_GridTooSmall_ColsLessThan2_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 2, C = 1, number = 0 }
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Grid dimensions \\(R:2,C:1\\) out of 2-8 bounds\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración con menos de 2 columnas debería ser inválida.");
    }

    [Test]
    public void IsValidConfig_GridTooLarge_RowsGreaterThan8_ReturnsFalse()
    {
        var blocks = new List<BlockData>();
        for (int r = 1; r <= 9; r++)
        {
            blocks.Add(new BlockData { R = r, C = 1, number = (r - 1) % _mockCardTypesList.Count });
            blocks.Add(new BlockData { R = r, C = 2, number = (r - 1) % _mockCardTypesList.Count });
        }
        GameConfig config = new GameConfig { blocks = blocks.ToArray() };
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Grid dimensions \\(R:9,C:2\\) out of 2-8 bounds\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración con más de 8 filas debería ser inválida.");
    }

    [Test]
    public void IsValidConfig_DuplicatePositions_ReturnsFalse()
    {
        GameConfig config = new GameConfig
        {
            blocks = new BlockData[] {
                new BlockData { R = 2, C = 1, number = 0 }, new BlockData { R = 2, C = 2, number = 1 },
                new BlockData { R = 1, C = 1, number = 0 }, new BlockData { R = 2, C = 1, number = 1 }
            }
        };
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Duplicate position \\(R:2,C:1\\)\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración con posiciones (R,C) duplicadas debería ser inválida.");
    }

    [Test]
    public void IsValidConfig_ConfigIsNull_ReturnsFalse()
    {
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Config object is null\\.$"));
        bool isValid = _loader.IsValidConfig(null, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración nula debería ser inválida.");
    }

    [Test]
    public void IsValidConfig_BlocksArrayIsNull_ReturnsFalse()
    {
        GameConfig config = new GameConfig { blocks = null };
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Config.blocks array is null\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración con array de bloques nulo debería ser inválida.");
    }

    [Test]
    public void IsValidConfig_BlocksArrayIsEmpty_ReturnsFalse()
    {
        GameConfig config = new GameConfig { blocks = new BlockData[0] };
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Config.blocks is empty\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración con array de bloques vacío debería ser inválida.");
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
        LogAssert.Expect(LogType.Error, new Regex("^Validation: Null block entry found\\.$"));
        bool isValid = _loader.IsValidConfig(config, _mockRegistry);
        Assert.IsFalse(isValid, "Configuración con una entrada de bloque nula debería ser inválida.");
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CardRegistry", menuName = "MemoryGame/Card Registry", order = 0)]
public class CardRegistrySO : ScriptableObject
{
    [Tooltip("Lista de todos los tipos de cartas disponibles en el juego.")]
    public List<CardTypeSO> cardTypes;

    private Dictionary<int, CardTypeSO> _typeLookup;

    
    private void OnEnable()
    {
        InitializeLookup();
    }

    public void InitializeLookup() // Puede ser llamado explícitamente si es necesario
    {
        if (cardTypes == null)
        {
            _typeLookup = new Dictionary<int, CardTypeSO>();
            Debug.LogError("CardRegistrySO: 'cardTypes' list is null!");
            return;
        }

        // Filtrar nulos antes de crear el diccionario para evitar errores
        var validCardTypes = cardTypes.Where(ct => ct != null).ToList();

        // Comprobar IDs duplicados
        var duplicateIds = validCardTypes.GroupBy(ct => ct.id)
                                   .Where(g => g.Count() > 1)
                                   .Select(g => g.Key);
        foreach (var id in duplicateIds)
        {
            Debug.LogError($"CardRegistrySO: Duplicate CardTypeSO ID '{id}' found. IDs must be unique.");
        }

        // Si hay duplicados, es mejor no inicializar o manejarlo de alguna formaa.
        // Por simplicidad, aquí se toma el primero que encuentre con un ID.
        _typeLookup = validCardTypes
            .GroupBy(ct => ct.id)
            .ToDictionary(g => g.Key, g => g.First());

        if (validCardTypes.Count != cardTypes.Count)
        {
            Debug.LogWarning("CardRegistrySO: Some CardTypeSO entries in the list were null and have been ignored.");
        }
    }

    public CardTypeSO GetCardTypeById(int id)
    {
        if (_typeLookup == null)
        {
            Debug.LogWarning("CardRegistrySO lookup not initialized. Initializing now.");
            InitializeLookup(); // Intento de inicialización tardía
        }

        if (_typeLookup != null && _typeLookup.TryGetValue(id, out CardTypeSO type))
        {
            return type;
        }
        Debug.LogWarning($"CardTypeSO with ID '{id}' not found in registry.");
        return null;
    }

    public int GetTotalRegisteredTypes()
    {
        if (_typeLookup == null) InitializeLookup();
        return _typeLookup?.Count ?? 0;
    }
}
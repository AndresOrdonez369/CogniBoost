using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CardRegistry", menuName = "MemoryGame/Card Registry", order = 0)]
public class CardRegistrySO : ScriptableObject
{
    [Tooltip("Lista de todos los tipos de cartas disponibles en el juego.")]
    public List<CardTypeSO> cardTypes;

    private Dictionary<int, CardTypeSO> _typeLookup;
    private bool _isLookupInitialized = false; // Flag para saber si ya se inicializó

    /*private void OnEnable()
    {
        InitializeLookup();
    }
*/
    public void InitializeLookup()
    {
        if (_typeLookup == null)
        {
            _typeLookup = new Dictionary<int, CardTypeSO>();
        }
        else
        {
            _typeLookup.Clear(); // Limpiar por si se llama de nuevo
        }

        if (cardTypes == null || cardTypes.Count == 0)
        {
            Debug.LogWarning("CardRegistrySO: 'cardTypes' list is null or empty during InitializeLookup. Lookup will be empty.");
            _isLookupInitialized = true; // Marcar como inicializado, aunque vacío
            return;
        }

        var validCardTypes = cardTypes.Where(ct => ct != null).ToList();
        if (validCardTypes.Count != cardTypes.Count)
        {
             Debug.LogWarning("CardRegistrySO: Some CardTypeSO entries in the 'cardTypes' list were null and have been ignored.");
        }

        // Comprobar IDs duplicados en los CardTypeSO válidos
        var duplicateIdGroups = validCardTypes.GroupBy(ct => ct.id).Where(g => g.Count() > 1);
        foreach (var group in duplicateIdGroups)
        {
            Debug.LogError($"CardRegistrySO: Duplicate CardTypeSO ID '{group.Key}' found. IDs must be unique. Occurrences: {group.Count()}");
        }

        // Crear el diccionario, tomando el primer CardTypeSO si hay IDs duplicados (aunque ya se logueó el error)
        _typeLookup = validCardTypes
            .GroupBy(ct => ct.id)
            .ToDictionary(g => g.Key, g => g.First());

        _isLookupInitialized = true; // Marcar que el lookup se ha intentado inicializar
        // Debug.Log("CardRegistrySO: Lookup initialized with " + _typeLookup.Count + " entries.");
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
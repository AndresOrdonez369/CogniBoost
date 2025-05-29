using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private CardRegistrySO cardRegistry; 

    private List<Card> _cards = new List<Card>();
    public List<Card> Cards => _cards;

    private System.Action<Card> _onCardSelectedCallback; // Para pasar al GameManager

    public bool CreateGrid(GameConfig config, System.Action<Card> onCardSelectedCallback)
    {
        _onCardSelectedCallback = onCardSelectedCallback;

        if (cardPrefab == null || gridParent == null || cardRegistry == null)
        {
            Debug.LogError("GridManager: Card Prefab, Grid Parent, or Card Registry not assigned.");
            return false;
        }
        if (config == null || config.blocks == null)
        {
            Debug.LogError("GridManager: GameConfig is null or has no blocks.");
            return false;
        }

        cardRegistry.InitializeLookup(); // Asegurarse de que el lookup esté listo

        ClearGrid();

        int numColumns = config.blocks.Max(b => b.C); // Asumiendo 1-based
        int numRows = config.blocks.Max(b => b.R);    // Asumiendo 1-based

        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            Debug.LogError("GridManager: Grid Parent does not have a GridLayoutGroup component.");
            return false;
        }
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = numColumns;
        // Podrías querer ajustar el tamaño de la celda dinámicamente aquí también

        var sortedBlocks = config.blocks.OrderBy(b => b.R).ThenBy(b => b.C).ToList();

        foreach (var blockData in sortedBlocks)
        {
            CardTypeSO type = cardRegistry.GetCardTypeById(blockData.number);
            if (type == null) // Esta validación ya está en ConfigLoader, pero una doble verificación no daña
            {
                Debug.LogError($"GridManager: Critical - No CardTypeSO found for number {blockData.number} during grid creation. Check CardTypeRegistry and JSON. Block (R:{blockData.R},C:{blockData.C})");
                ClearGrid(); // Limpiar lo que se haya creado
                return false; // Detener la creación de la grilla
            }

            GameObject cardGO = Instantiate(cardPrefab, gridParent);
            Card cardScript = cardGO.GetComponent<Card>();

            if (cardScript != null)
            {
                cardScript.Initialize(blockData, type, _onCardSelectedCallback);
                _cards.Add(cardScript);

                Button cardButton = cardGO.GetComponent<Button>();
                if (cardButton != null)
                {
                    cardButton.onClick.AddListener(cardScript.OnPointerClick);
                }
                else
                {
                    Debug.LogWarning($"Card prefab {cardPrefab.name} does not have a Button component. Clicks might not be detected via UI system.", cardGO);
                }
            }
            else
            {
                Debug.LogError($"Card prefab {cardPrefab.name} is missing the Card script component.", cardGO);
                Destroy(cardGO);
            }
        }
        Debug.Log($"Grid created with {_cards.Count} cards. Expected columns: {numColumns}, Expected rows: {numRows}.");
        return true;
    }

    public void ClearGrid()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        _cards.Clear();
    }

    public Card GetCardAt(int r, int c)
    {
        return _cards.FirstOrDefault(card => card.Row == r && card.Column == c);
    }
}
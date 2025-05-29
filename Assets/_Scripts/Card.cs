using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardTypeSO CardType { get; private set; }
    public int Row { get; private set; }
    public int Column { get; private set; }

    [Header("Card Visuals")]
    [Tooltip("La imagen que se muestra cuando la carta está boca abajo.")]
    [SerializeField] private Image cardBackImage;
    [Tooltip("La imagen que se muestra cuando la carta está boca arriba.")]
    [SerializeField] private Image cardFaceImage;
    [Tooltip("El color de tinte para la carta cuando se encuentra un par.")]
    [SerializeField] private Color matchedTintColor = new Color(195f / 255f, 254f / 255f, 1f, 1f); // Color por defecto (cian brillante)

    private bool _isRevealed = false;
    public bool IsRevealed => _isRevealed;
    private bool _isMatched = false;
    public bool IsMatched => _isMatched;

    private System.Action<Card> _onCardSelectedCallback;

    public void Initialize(BlockData data, CardTypeSO type, System.Action<Card> onCardSelectedCallback)
    {
        this.Row = data.R;
        this.Column = data.C;
        this.CardType = type;
        this._onCardSelectedCallback = onCardSelectedCallback;

        this.name = $"Card_R{Row}_C{Column}_Val_{(type != null ? type.id.ToString() : "NO_TYPE")}";

        if (cardFaceImage != null && CardType != null && CardType.faceSprite != null)
        {
            cardFaceImage.sprite = CardType.faceSprite;
            RectTransform faceImageRect = cardFaceImage.GetComponent<RectTransform>();
            if (faceImageRect != null)
            {
                faceImageRect.anchorMin = Vector2.zero;
                faceImageRect.anchorMax = Vector2.one;
                faceImageRect.offsetMin = Vector2.zero;
                faceImageRect.offsetMax = Vector2.zero;
            }
        }
        else
        {
            if (cardFaceImage == null) Debug.LogError($"Card {name}: cardFaceImage no está asignado en el Inspector.", this);
            if (CardType == null) Debug.LogError($"Card {name}: CardType es null (BlockData.number podría no corresponder a un CardTypeSO).", this);
            else if (CardType.faceSprite == null) Debug.LogError($"Card {name}: CardType '{CardType.name}' (ID: {CardType.id}) no tiene un faceSprite asignado.", this);
        }

        if (cardBackImage != null)
        {
            RectTransform backImageRect = cardBackImage.GetComponent<RectTransform>();
            if (backImageRect != null)
            {
                backImageRect.anchorMin = Vector2.zero;
                backImageRect.anchorMax = Vector2.one;
                backImageRect.offsetMin = Vector2.zero;
                backImageRect.offsetMax = Vector2.zero;
            }
        }
        else
        {
            Debug.LogError($"Card {name}: cardBackImage no está asignado en el Inspector.", this);
        }

        HideCard(true); // Inicialmente ocultas
    }

    public void RevealCard()
    {
        if (_isMatched || _isRevealed) return;
        _isRevealed = true;
        if (cardBackImage != null) cardBackImage.gameObject.SetActive(false);
        if (cardFaceImage != null) cardFaceImage.gameObject.SetActive(true);
    }

    public void HideCard(bool immediate = false)
    {
        if (_isMatched && !immediate) return;
        _isRevealed = false;
        if (cardBackImage != null) cardBackImage.gameObject.SetActive(true);
        if (cardFaceImage != null) cardFaceImage.gameObject.SetActive(false);
    }

    public void SetMatched()
    {
        _isMatched = true;
        _isRevealed = true;

        if (cardFaceImage != null)
        {
            cardFaceImage.gameObject.SetActive(true);
            cardFaceImage.color = matchedTintColor; // Aplicar tinte al encontrar par
        }
        if (cardBackImage != null)
        {
            cardBackImage.gameObject.SetActive(false);
        }

        Button button = GetComponent<Button>();
        if (button)
        {
            button.interactable = false;
        }

     
    }

    public void OnPointerClick()
    {
        // Prevenir acción si la carta ya está emparejada,
        // o si ya está revelada (GameManager previene seleccionar una carta ya revelada como la primera de un nuevo par),
        // o si el callback no está asignado.
        if (_isMatched || _isRevealed || _onCardSelectedCallback == null) return;

        _onCardSelectedCallback.Invoke(this);
    }
}
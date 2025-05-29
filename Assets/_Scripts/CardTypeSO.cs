using UnityEngine;


[CreateAssetMenu(fileName = "CardType_", menuName = "MemoryGame/Card Type", order = 1)]
public class CardTypeSO : ScriptableObject
{
    [Tooltip("El identificador único para este tipo de carta, debe coincidir con 'number' en el JSON.")]
    public int id; // Este será el 'number' del JSON (0-9)

    [Tooltip("Sprite que se mostrará cuando la carta esté boca arriba.")]
    public Sprite faceSprite;

}
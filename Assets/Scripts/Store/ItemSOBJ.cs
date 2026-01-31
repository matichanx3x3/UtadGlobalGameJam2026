using UnityEngine;

// Definimos los tipos de efectos posibles
public enum ItemEffectType
{
    ScoreMultiplier, // Aumenta el multiplicador de puntos
    HpBonus,         // Suma vida máxima
    AttackDamage     // Aumenta el daño
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/Item")]
public class ItemSOBJ : ScriptableObject
{
    [Header("Info de la Tienda")]
    public string itemName;
    [TextArea] public string description;
    public int cost;
    public Sprite icon; // Por si quieres mostrar una imagen en la UI

    [Header("Efecto del Ítem")]
    public ItemEffectType effectType;
    [Tooltip("Cantidad que aumentará (Ej: 1 para HP, 0.5 para daño, etc)")]
    public float amount;
}

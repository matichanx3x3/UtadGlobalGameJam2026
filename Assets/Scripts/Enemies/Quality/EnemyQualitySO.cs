using UnityEngine;

public enum QualityType
{
    None,           // Normal
    Exploder,       // Rojo: Explota
    Shielded,       // Rojo/Azul: Solo vulnerable por espalda o empuje
    Indestructible, // Verde: Solo empujable
    SpikesTop,      // Verde: Vulnerable a empuje, daño por salto
    SpikesSide,     // Azul: Vulnerable a salto, daño por lado
    Flying          // Neutral: Vuela, atraviesa plataformas
}

[CreateAssetMenu(fileName = "NewQuality", menuName = "Enemy/Quality")]
public class EnemyQualitySO : ScriptableObject
{
    public string qualityName;
    public QualityType type;
    

    [Header("Modificadores Base")]
    public float speedMultiplier = 1f;
    public int healthBonus = 0;

    [Header("Configuración Específica")]
    [Tooltip("Tiempo para explotar (0 = instantáneo)")]
    public float explosionDelay = 0.5f;
    [Tooltip("Radio de explosión")]
    public float explosionRadius = 2f;
}
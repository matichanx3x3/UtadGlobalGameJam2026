using UnityEngine;

[CreateAssetMenu(fileName = "NewQuality", menuName = "Enemy/Quality")]
public class EnemyQualitySO : ScriptableObject
{
    public string qualityName;
    [Header("Modificadores")]
    public float speedMultiplier = 1f;
    public int healthBonus = 0;
    public float damageMultiplier = 1f;
    public Color colorTint = Color.white; // identificador visual de la cualidad.
}
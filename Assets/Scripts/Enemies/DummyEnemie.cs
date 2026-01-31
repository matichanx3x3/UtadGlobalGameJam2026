using UnityEngine;
using DG.Tweening; // Usamos DOTween para feedback visual

public class DummyEnemy : MonoBehaviour, IDamageable, IPushable
{
    [Header("Configuración")]
    public float weightResistance = 1f; // 1 = normal, 2 = pesado, 0.5 = ligero
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // rojo o azul
    public void TakeDamage(int amount, string sourceName)
    {
        
        // feedback visual rojo
        spriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() => spriteRenderer.DOColor(originalColor, 0.1f));
        
        // feedback recibi daño
        transform.DOPunchScale(new Vector3(0.2f, -0.2f, 0), 0.2f);
    }

    // verde
    public void Push(Vector2 direction, float force)
    {
        // fuerza final restando el peso.
        float finalForce = force / weightResistance;

        // se aplica fuerza 
        if (rb != null)
        {
            // se resetea la velocidad para un empuje seco
            rb.velocity = Vector2.zero; 
            rb.AddForce(direction * finalForce, ForceMode2D.Impulse);
        }
    }
}
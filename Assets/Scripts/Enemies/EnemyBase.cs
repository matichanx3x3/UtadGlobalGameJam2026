using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class EnemyBase : MonoBehaviour, IDamageable, IPushable
{
    [Header("Estadísticas Base")]
    public float moveSpeed = 3f;
    public int maxHealth = 3;
    protected int currentHealth;
    
    [Header("Cualidades")]
    public List<EnemyQualitySO> qualities; //cualidades

    [Header("Detección")]
    public LayerMask groundLayer;
    public Transform groundCheck;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected bool isFacingRight = true;
    protected Transform playerTransform;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        ApplyQualities();
    }

    // modificadores de los sobj
    void ApplyQualities()
    {
        foreach (var quality in qualities)
        {
            moveSpeed *= quality.speedMultiplier;
            maxHealth += quality.healthBonus;
            currentHealth += quality.healthBonus;
            spriteRenderer.color = quality.colorTint;
        }
    }

    public virtual void TakeDamage(int amount, string sourceName)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió daño de {sourceName}. Vida: {currentHealth}");
        
        // feedback
        transform.DOShakeScale(0.2f, 0.2f);
        spriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() => {
            Color originalColor = qualities.Count > 0 ? qualities[0].colorTint : Color.white;
            spriteRenderer.color = originalColor;
        });

        if (currentHealth <= 0) Die();
    }

    public virtual void Push(Vector2 direction, float force)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    protected virtual void Die()
    {
        // anim de muerte o partículas
        transform.DOScale(0, 0.2f).OnComplete(() => Destroy(gameObject));
    }

    // girar el sprite
    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // hay suelo delante?
    protected bool CheckEdge()
    {
        Vector2 origin = groundCheck.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f, groundLayer);
        return hit.collider == null; // null = vacio
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
      if(collision.CompareTag("VoidZone") || collision.CompareTag("Damagable"))
      {
        Die();
      }  
    }
}
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
    public int scoreValue = 50;
    
    [Header("Cualidades")]
    public List<EnemyQualitySO> qualities;
    protected EnemyQualitySO activeQuality; // Referencia rápida a la cualidad activa

    [Header("Detección")]
    public LayerMask groundLayer;
    public Transform groundCheck;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected bool isFacingRight = true;
    protected Transform playerTransform;
    protected Collider2D myCollider;
    protected Animator anim;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        ApplyQualities();
    }
    protected virtual void Update()
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
    }
    void ApplyQualities()
    {
        if (qualities.Count > 0)
        {
            activeQuality = qualities[0]; // Asumimos una única cualidad como pediste

            moveSpeed *= activeQuality.speedMultiplier;
            maxHealth += activeQuality.healthBonus;
            currentHealth += activeQuality.healthBonus;
            spriteRenderer.color = activeQuality.colorTint;

            // Lógica de VUELO (Neutral)
            if (activeQuality.type == QualityType.Flying)
            {
                rb.gravityScale = 0; // Sin gravedad
                
                rb.mass = 100f; 
                
                rb.drag = 10f; 

                Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);
                
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }

    // recibe daño, segun el tipo que es
    public virtual void TakeDamage(int amount, string sourceName)
    {
        if (activeQuality != null)
        {
            // INDESTRUCTIBLE -> solo verde
            if (activeQuality.type == QualityType.Indestructible)
            {
                return;
            }

            // ESCUDADO
            if (activeQuality.type == QualityType.Shielded)
            {
                // Si el ataque viene de ARRIBA (Salto) -> Bloquear
                if (sourceName.Contains("Salto"))
                {
                    //feedback de bloqueo
                    transform.DOShakePosition(0.2f, 0.1f);
                    return;
                }
                
                // Si el ataque es frontal -> Bloquear
                Vector2 dirToPlayer = (playerTransform.position - transform.position).normalized;
                bool isAttackFromFront = (isFacingRight && dirToPlayer.x > 0) || (!isFacingRight && dirToPlayer.x < 0);

                if (isAttackFromFront)
                {
                    //feedback de bloqueo
                    transform.DOShakePosition(0.2f, 0.1f); 
                    return;
                }
            }

            // PUAS ARRIBA -> inmune a los saltos.
            if (activeQuality.type == QualityType.SpikesTop && sourceName.Contains("Salto"))
            {
                 // poner referencia para hacer daño al jugador
                 return;
            }
        }
        // recibe daño
        currentHealth -= amount;
        if (anim != null) anim.SetTrigger("Hit");
        // Feedback Visual
        spriteRenderer.DOColor(Color.red, 0.1f).OnComplete(() => {
            Color originalColor = activeQuality != null ? activeQuality.colorTint : Color.white;
            spriteRenderer.color = originalColor;
        });

        if (currentHealth <= 0) Die();
    }

    public virtual void Push(Vector2 direction, float force)
    {
        rb.velocity = Vector2.zero;
        if (activeQuality != null && activeQuality.type == QualityType.Flying)
        {
            // fuerza multiplicada con respecto al peso.
            rb.AddForce(direction * force * 50f, ForceMode2D.Impulse); 
        }
        else
        {
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }

    // PUAS -> Dañar al jugador al contacto
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (activeQuality == null) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            // posición del contacto
            Vector2 normal = collision.contacts[0].normal;
            
            // jugador aterrizó ENCIMA del enemigo
            bool hitFromTop = normal.y < -0.5f; 

            // puas laterales azul: daña al costado
            if (activeQuality.type == QualityType.SpikesSide && !hitFromTop)
            {
                Debug.Log("JUGADOR: Dañado por Púas Laterales");
                // collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(1);
            }
            // puas superiores verde: daña por encima
            else if (activeQuality.type == QualityType.SpikesTop && hitFromTop)
            {
                Debug.Log("JUGADOR: Dañado por Púas Superiores");
                // collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(1);
            }
        }
    }

    protected virtual void Die()
    {
        if (anim != null) 
        {
            anim.SetBool("Die", true);
            
            
            rb.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
        }
        PlayerController.OnPointsEarned?.Invoke(scoreValue);
        transform.DOScale(0, 0.2f).OnComplete(() => Destroy(gameObject));
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected bool CheckEdge()
    {
        // Si vuela, no le importan los bordes del suelo
        if (activeQuality != null && activeQuality.type == QualityType.Flying) return false;

        Vector2 origin = groundCheck.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f, groundLayer);
        return hit.collider == null; 
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
      if(collision.CompareTag("VoidZone") || collision.CompareTag("Damagable"))
      {
        Die();
      }  
    }
}
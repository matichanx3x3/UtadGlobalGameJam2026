using UnityEngine;
using System.Collections;
using DG.Tweening;
public class EnemyAttacker : EnemyBase
{
    [Header("Combate")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public int damage = 1;
    private float lastAttackTime;

    [Header("Tipo de Ataque")]
    public GameObject projectilePrefab; // Si es null -> MELEE. Sino -> RANGO.
    public Transform firePoint;
    public bool canFollowPlayer;

    private bool isExploding = false;

    protected override void Update()
    {
        if (playerTransform == null) return;
        if (isExploding) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
        // Gira hacia el jugador siempre
        if (playerTransform.position.x > transform.position.x && !isFacingRight) Flip();
        else if (playerTransform.position.x < transform.position.x && isFacingRight) Flip();

        if (distance <= attackRange)
        {
            // Detenerse para atacar
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (activeQuality != null && activeQuality.type == QualityType.Exploder)
            {
                // Iniciar secuencia de explosión (una sola vez)
                StartCoroutine(ExplodeSequence());
            }
            else if (Time.time > lastAttackTime + attackCooldown)
            {
                // Ataque Normal (Melee/Rango)
                Attack();
                lastAttackTime = Time.time;
            }

        }
        else
        {
            if (canFollowPlayer)
            {
                MoveTowardsPlayer();
            }
            else
            {
                // modo torreta
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }
    
    void MoveTowardsPlayer()
    {
        float dirX = (playerTransform.position.x > transform.position.x) ? 1 : -1;
        
        if (CheckEdge())
        {
             rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
             rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        } 
    }

    void Attack()
    {
        if (projectilePrefab != null) // RANGO
        {
            
            if (anim != null) anim.SetTrigger("Attack");
            // Instanciar
            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projScript = projObj.GetComponent<Projectile>();
            if (projScript != null)
            {
                // direccion segun donde mira
                Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
                projScript.Initialize(dir);
            }
        }
        else // MELEE
        {
            
            if (anim != null) anim.SetTrigger("Attack");
            Debug.Log("Enemigo golpea!");
            // deteccion del jugador
            if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
            {
                // falta hacerle daño al player
                //algo parecido a 
                // playerTransform.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    IEnumerator ExplodeSequence()
    {
        isExploding = true;
        float delay = activeQuality.explosionDelay;

        Debug.Log("ENEMIGO: ¡Secuencia de autodestrucción iniciada!");

        // Feedback visual: Parpadeo rápido o hinchazón
        transform.DOShakeScale(delay, 0.5f);
        spriteRenderer.DOColor(Color.red, delay);

        yield return new WaitForSeconds(delay);

        // EXPLOSIÓN
        Debug.Log("BOOM!");
        
        // Detectar si el jugador está en el radio
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, activeQuality.explosionRadius, LayerMask.GetMask("Player"));
        if (hitPlayer != null)
        {
            // Dañar al jugador
            Debug.Log("Jugador dañado por explosión");
            // hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(damage * 2); // Daño masivo
        }

        // Efecto visual de explosión (opcional instanciar particulas)
        // Instantiate(explosionParticles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (qualities != null && qualities.Count > 0 && qualities[0].type == QualityType.Exploder)
        {
            Gizmos.color = new Color(1, 0.5f, 0, 0.5f); // Naranja
            Gizmos.DrawWireSphere(transform.position, qualities[0].explosionRadius);
        }
    }
}
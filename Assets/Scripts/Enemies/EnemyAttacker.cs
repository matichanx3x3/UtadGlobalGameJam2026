using UnityEngine;

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

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // Gira hacia el jugador siempre
        if (playerTransform.position.x > transform.position.x && !isFacingRight) Flip();
        else if (playerTransform.position.x < transform.position.x && isFacingRight) Flip();

        if (distance <= attackRange)
        {
            // Detenerse para atacar
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (Time.time > lastAttackTime + attackCooldown)
            {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(firePoint.position, 0.5f);
        }
    }
}
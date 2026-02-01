using UnityEngine;

public class EnemyChaser : EnemyBase
{
    [Header("IA Chaser")]
    public float detectionRange = 5f;
    public float lossRange = 7.5f;
    public float stopDistance = 0.5f; // distancia para dejar de perseguir
    public float lostTargetWaitTime = 2f;
    private bool isChasing = false;
    private bool isWaiting = false;
    private float waitCounter;
    public float chasingSpeed;
    private int direction = 1; // direccion donde ver

    protected override void Start()
    {
        base.Start();
        waitCounter = lostTargetWaitTime;
        direction = Random.Range(0, 2) == 0 ? 1 : -1; 
    }

    protected override void Update()
    {
        base.Update();
        if (playerTransform == null) return;

        float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
        if (distToPlayer < detectionRange)
        {
            isChasing = true;
            isWaiting = false;
            waitCounter = lostTargetWaitTime; 
        }
        // Si el jugador se aleja y se perseguia -> ahora espera
        else if (distToPlayer > lossRange && isChasing)
        {
            isChasing = false;
            isWaiting = true; 
            
            rb.velocity = Vector2.zero; 
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else if (isWaiting)
        {
            waitCounter -= Time.deltaTime;

            // patrulla al pasar el tiempo de espera
            if (waitCounter <= 0)
            {
                isWaiting = false;

                direction = isFacingRight ? 1 : -1;
            }
        }
        else
        {
            // Si no persigue ni espera -> Patrulla
            Wander();
        }
    }

    void ChasePlayer()
    {
        // dirección hacia el jugador
        float dirX = (playerTransform.position.x > transform.position.x) ? 1 : -1;

        // comprobar si hay suelo
        if (CheckEdge())
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            // Moverse
            rb.velocity = new Vector2(dirX * chasingSpeed, rb.velocity.y);
            
            // Girar sprite
            if (dirX > 0 && !isFacingRight) Flip();
            else if (dirX < 0 && isFacingRight) Flip();
        }
    }

    void Wander()
    {   

        // Detectar si se va a caer
        if (CheckEdge())
        {
            direction *= -1;
            Flip();
        }

        // Aplicar movimiento
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // que el sprite siempre calce con el mov
        if (direction > 0 && !isFacingRight) Flip();
        else if (direction < 0 && isFacingRight) Flip();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        //rango de perdida
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, lossRange);
    }
}

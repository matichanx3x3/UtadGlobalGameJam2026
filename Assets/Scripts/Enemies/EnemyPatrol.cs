using UnityEngine;

public class EnemyPatrol : EnemyBase
{
    [Header("Patrulla")]
    public Transform[] waypoints;
    private Vector2[] patrolPoints;
    private int currentPointIndex = 0;
    public float waitTime = 1f;
    private float waitCounter = 0f;

    protected override void Start()
    {
        base.Start();
        patrolPoints = new Vector2[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
                patrolPoints[i] = waypoints[i].position;
        }
        
        waitCounter = waitTime;
    }

    protected override void Update()
    {
        
        base.Update();
        if (patrolPoints.Length == 0) return;

        Vector2 targetPosition = patrolPoints[currentPointIndex];
        
        // Calculamos la distancia
        float distance = Vector2.Distance(transform.position, targetPosition);


        if (distance > 0.5f) 
        {
            waitCounter = waitTime; 

            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            if (targetPosition.x > transform.position.x && !isFacingRight) Flip();
            else if (targetPosition.x < transform.position.x && isFacingRight) Flip();
        }
        else 
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            waitCounter -= Time.deltaTime;

            if (waitCounter <= 0)
            {
                currentPointIndex++;
                if (currentPointIndex >= patrolPoints.Length) 
                    currentPointIndex = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
                
                Transform nextPoint = waypoints[(i + 1) % waypoints.Length];
                if (nextPoint != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, nextPoint.position);
                }
            }
        }
    }
}
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerController playerController;
    public Transform attackPoint; // para debug visual

    [Header("Máscara Roja (Ataque)")]
    public Vector2 attackSize = new Vector2(1.5f, 1f); // area del puño
    public int attackDamage = 1;
    public float attackRate = 0.5f; // delay entre golpe
    private float nextAttackTime = 0f;

    [Header("Máscara Verde (Empuje)")]
    public Vector2 pushSize = new Vector2(3f, 2f); // area del viento
    public float pushForce = 10f; // fuerza de empuje del viento
    public float pushRate = 1f; // delay
    private float nextPushTime = 0f;

    [Header("Capas")]
    public LayerMask enemyLayers; // layer que afectará

    void Update()
    {
        if (playerController.pInput.ActionPressed)
        {
            HandleCombatAction();
        }
    }

    void HandleCombatAction()
    {
        // verifica el input y su tiempo
        switch (playerController.maskActual)
        {
            case 1: // ROJA
                if (Time.time >= nextAttackTime)
                {
                    PerformAttackRed();
                    nextAttackTime = Time.time + attackRate;
                }
                break;

            case 2: // VERDE
                if (Time.time >= nextPushTime)
                {
                    PerformPushGreen();
                    nextPushTime = Time.time + pushRate;
                }
                break;
        }
    }

    void PerformAttackRed()
    {
        // tods los enemigos en el area
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0f, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage, "Puño Rojo");
            }
        }
    }

    void PerformPushGreen()
    {
        // detecta objetos
        Collider2D[] hitObjects = Physics2D.OverlapBoxAll(attackPoint.position, pushSize, 0f, enemyLayers);

        // se calcula seguna donde mira el persona.
        Vector2 pushDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        foreach (Collider2D obj in hitObjects)
        {
            IPushable pushable = obj.GetComponent<IPushable>();
            if (pushable != null)
            {
                pushable.Push(pushDir, pushForce);
            }
        }
    }

    // draw gizmo
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        // Caja Roja (Ataque)
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);

        // Caja Verde (Empuje)
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(attackPoint.position, pushSize);
    }
}
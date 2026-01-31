using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfProjectile
{
    Normal,
    Parabol,
    Homming,

}

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour
{
    public TypeOfProjectile typeOfProjectile;

    [Header("Estadísticas")]
    public float speed = 10f;
    public int damage = 1;
    public float timeToDestroy = 3f;

    [Header("Configuración Parábola")]
    public float arcHeight = 5f; // Fuerza hacia arriba

    [Header("Configuración Homing")]
    public float rotateSpeed = 200f; // Qué tan rápido gira buscando al jugador

    private Rigidbody2D rb;
    private Transform target; // ref al jugador
    private Vector2 direction; // Dirección inicial

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 startDirection)
    {
        direction = startDirection.normalized;
        
        // Destrucción automática por tiempo
        Destroy(gameObject, timeToDestroy);

        // Configuración inicial según el tipo
        switch (typeOfProjectile)
        {
            case TypeOfProjectile.Normal:
                rb.gravityScale = 0; // Sin gravedad, vuela recto
                rb.velocity = direction * speed;
                break;

            case TypeOfProjectile.Parabol:
                rb.gravityScale = 1.5f; // Con gravedad para caer
                ParabolBehaviour(); // Impulso inicial
                break;

            case TypeOfProjectile.Homming:
                rb.gravityScale = 0; // Sin gravedad, flota persiguiendo
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
                break;
        }
    }

    void FixedUpdate()
    {
        //en fixed por recalculos constantes.
        if (typeOfProjectile == TypeOfProjectile.Homming)
        {
            HommingBehaviour();
        }
        else if (typeOfProjectile == TypeOfProjectile.Normal)
        {
            NormalBehaviour();
        }
    }

    private void NormalBehaviour()
    {
        rb.velocity = direction * speed;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void ParabolBehaviour()
    {
        // vector direccion.
        Vector2 arcDirection = (direction + Vector2.up).normalized;
        
        //impulso instantáneo
        rb.velocity = arcDirection * speed;
    }

    private void HommingBehaviour()
    {
        if (target == null) return; 

        // dirección hacia el jugador
        Vector2 directionToTarget = (Vector2)target.position - rb.position;
        directionToTarget.Normalize();

        
        // calculos de rotación
        float rotateAmount = Vector3.Cross(directionToTarget, transform.right).z;
        rb.angularVelocity = -rotateAmount * rotateSpeed;

        // se mueve hacia donde apunta.
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // se evita que choque consigo mismo
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) return; 

        // intenta hacer el daño
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, "Proyectil " + typeOfProjectile.ToString());
            Destroy(gameObject); 
        }
    }
}

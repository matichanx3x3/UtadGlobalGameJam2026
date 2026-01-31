using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//referencias: https://www.youtube.com/watch?v=KKGdDBFcu0Q
public class PlayerMovement : MonoBehaviour
{
    // Referencia al ScriptableObject con los datos del jugador
    //Referencia al Input System
    public PlayerController playerController;
    
    // Componente Rigidbody2D del jugador
    public Rigidbody2D RB;
    
    // Estado de dirección del jugador
    public bool isFacingRight;
    
    // Estado de salto del jugador
    public bool isJumping;
    private bool isJumpCut; // Indica si el jugador soltó el botón de salto antes de tiempo
    private bool isJumpFalling; // Indica si el jugador está cayendo después de un salto
    
    // Temporizadores para coyote time y buffering
    public float lastOnGroundTime; // Tiempo desde que el jugador tocó el suelo por última vez
    public float lastPressedJumpTime; // Tiempo desde que el jugador presionó saltar por última vez
    

    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint; // Punto de referencia para verificar si está en el suelo
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f); // Tamaño del box para verificar el suelo
    
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask groundLayer; // Layer que representa el suelo

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // la escala de gravedad inicial
        SetGravityScale(playerController.data.gravityScale);
        isFacingRight = true;
        
        RB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update se llama una vez por frame
    void Update()
    {
        // temporizadores
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        if (playerController.IsInputBlocked) //si el input se bloquea se detiene.
        {
            RB.velocity = new Vector2(0, RB.velocity.y); 
            return;
        }

        if (playerController.pInput.HasHorizontalInput())
            CheckDirectionToFace(playerController.pInput.GetHorizontalInput() > 0);

        if(playerController.maskActual == 3)
        { 
            if (playerController.pInput.ActionPressed)
            {
                OnJumpInput();
            }

            // cuando se suelta el botón de salto
            if (playerController.pInput.ActionReleased)
            {
                OnJumpUpInput();
            }
        }

        #region GROUND CHECK
        // el jugador está tocando el suelo?
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer) && !isJumping)
        {
            lastOnGroundTime = playerController.data.coyoteTime;
        }
        #endregion

        #region JUMP CHECKS
        // si esta en el salto, esta cayendo pues ya no salta
        if (isJumping && RB.velocity.y < 0)
        {
            isJumping = false;
            
            if (!isJumpFalling)
            {
                isJumpFalling = true;
            }
        }

        // verificaciones del coyote time
        if (CanJump() && lastPressedJumpTime > 0)
        {
            isJumping = true;
            isJumpCut = false;
            isJumpFalling = false;
            Jump();
        }
        #endregion

        #region GRAVITY
        // mientras cae, mas gravedad le afecta
        if (RB.velocity.y < 0 && playerController.pInput.GetVerticalInput() < 0)
        {
            // efecto de incrementar gravedad al usar flechaabajo
            SetGravityScale(playerController.data.gravityScale * playerController.data.fastFallGravityMult);
            // lo limita para que no llegue al infinit
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -playerController.data.maxFastFallSpeed));
        }
        else if (isJumpCut)
        {
            // si se suelta antes, aplica un poco mas de gravedad
            SetGravityScale(playerController.data.gravityScale * playerController.data.jumpCutGravityMult);
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -playerController.data.maxFallSpeed));
        }
        else if ((isJumping || isJumpFalling) && Mathf.Abs(RB.velocity.y) < playerController.data.jumpHangTimeThreshold)
        {
            // redce cuando llega al punto maximo -> "sensacion de flotar"
            SetGravityScale(playerController.data.gravityScale * playerController.data.jumpHangGravityMult);
        }
        else if (RB.velocity.y < 0)
        {
            // incrementa gravedad al caer (de forma normal)
            SetGravityScale(playerController.data.gravityScale * playerController.data.fallGravityMult);
            // lo limita para que no llegue al infinit
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -playerController.data.maxFallSpeed));
        }
        else
        {
            // gravedad por defecto si está en el suelo o moviéndose hacia arriba
            SetGravityScale(playerController.data.gravityScale);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        //movimiento
        Run();
    }

    #region INPUT CALLBACKS
    public void OnJumpInput()
    {
        // tiempo de la ultima vez que se presion saltar
        lastPressedJumpTime = playerController.data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        // marca el corte del salto
        if (CanJumpCut())
            isJumpCut = true;
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }
    #endregion

    #region RUN METHODS
    private void Run()
    {
        // velocidad objetivo (Input * Velocidad Máxima)
        float targetSpeed = playerController.pInput.GetHorizontalInput() * playerController.data.runMaxSpeed;

        // diferencia entre la velocidad actual y la deseada
        float speedDif = targetSpeed - RB.velocity.x;

        // qué tasa de aceleración usar
        // Si targetSpeed es casi 0, frena, sino esta corriendo.
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerController.data.runAccelAmount : playerController.data.runDeccelAmount;

        // ultiplicadores aéreos si no estamos en el suelo
        if (lastOnGroundTime <= 0)
        {
            accelRate *= (Mathf.Abs(targetSpeed) > 0.01f) ? playerController.data.accelInAir : playerController.data.deccelInAir;
        }
        // se aplicar la fuerza final
        // F = m * a , basada en la diferencia de velocidad
        float movement = speedDif * accelRate;

        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }
    #endregion

    #region TURN METHODS
    private void Turn()
    {
        // Almacena la escala y voltear el jugador en el eje x
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        //no se puede llamar Jump múltiples veces en una pulsación
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        // Aumenta la fuerza aplicada si esta cayendo
        float force = playerController.data.jumpForce;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        // Aplicar la fuerza de salto como impulso
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerController.maskActual == 3)
        {
            // Debemos estar cayendo y el objeto debe estar abajo
            if (RB.velocity.y < 0 && collision.contacts[0].normal.y > 0.5f)
            {
                // si es dañable
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                
                if (damageable != null)
                {
                    damageable.TakeDamage(1, "Salto Azul");
                    //rebote adicional
                    Jump();
                }
            }
        }
    }
    #endregion

    #region CHECK METHODS
    // donde mira el jugador
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
            Turn();
    }

    // puede saltar?
    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;
    }

    // puedo cortar el salto?
    private bool CanJumpCut()
    {
        return isJumping && RB.velocity.y > 0;
    }
    #endregion

    #region EDITOR METHODS
    // gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }
    #endregion

    
}
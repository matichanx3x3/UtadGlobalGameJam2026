using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Player Config")]
    public int hp = 3;
    public int scoreMultiplier = 1;
    public int hpBonus = 0;
    public int scoreSaved = 0;
    
    [Header("PlayerCombat")]
    public int attackDamage = 1;
    public float attackRate = 0.5f; // delay entre golpe
    public float pushForce = 10f; // fuerza de empuje del viento
    public float pushRate = 1f; // delay

    [Header("Gravity")]
    [Tooltip("Fuerza de gravedad con la que caerá (calculada automáticamente)")]
	[HideInInspector] public float gravityStrength;
    [Tooltip("Escala de gravedad relativa a la gravedad de Unity ")] 
	[HideInInspector] public float gravityScale; 
    [Tooltip("Multiplicador de gravedad al caer (hace que caiga más rápido que subir)")]
    public float fallGravityMult;
    [Tooltip("Velocidad máxima de caída ")]
	public float maxFallSpeed;
    [Tooltip("Multiplicador de gravedad para caída rápida (cuando presionas abajo)")]
	public float fastFallGravityMult;
    [Tooltip("Velocidad máxima de caída rápida")]
	public float maxFastFallSpeed; 

    [Header("Run")]
    [Tooltip("Velocidad máxima que queremos que el jugador alcance")]
	public float runMaxSpeed;
    [Tooltip("Velocidad a la que el jugador acelera hasta la velocidad máxima")] 
	public float runAcceleration;
    [Tooltip("Fuerza real de aceleracion (multiplicada con speedDiff) aplicada al jugador")]
	[HideInInspector] public float runAccelAmount;
    [Tooltip("Velocidad a la que el jugador desacelera desde su velocidad actual")]
	public float runDecceleration;
    [Tooltip("Fuerza real de desaceleracion(multiplicada con speedDiff) aplicada al jugador")]
	[HideInInspector] public float runDeccelAmount;
	[Tooltip("Multiplicador de aceleración cuando está en el aire")]
	[Range(0f, 1)] public float accelInAir;
    [Tooltip("Multiplicador de desaceleración cuando está en el aire")]
	[Range(0f, 1)] public float deccelInAir;

    [Header("Jump")]
    [Tooltip("Altura que alcanzará el salto (en unidades de Unity)")]
    public float jumpHeight;
    [Tooltip("Tiempo para llegar al punto máximo de salto (en segundos)")]
	public float jumpTimeToApex;
    [Tooltip("Fuerza de salto calculada automáticamente")]
	[HideInInspector] public float jumpForce;

    [Header("Both Jumps")]
    [Tooltip("Multiplicador de gravedad si el jugador suelta el botón de salto antes de tiempo")]
	public float jumpCutGravityMult;
    [Tooltip("Reduce la gravedad en el punto máximo del salto para dar sensación de flotar")]
	[Range(0f, 1)] public float jumpHangGravityMult;
    [Tooltip("Umbral de velocidad vertical para activar el hang time")]
	public float jumpHangTimeThreshold;
	[Tooltip("Multiplicador de aceleración horizontal durante el hang time")]
    public float jumpHangAccelerationMult;
    [Tooltip("Multiplicador de aceleración horizontal durante el hang time")]
	public float jumpHangMaxSpeedMult;

    [Header("Assist")]
    [Tooltip("Tiempo después de dejar una plataforma en el que aún puedes saltar")]
    [Range(0.01f, 0.5f)] public float coyoteTime;
    [Tooltip("Tiempo antes de tocar el suelo en el que se registra el input de salto")]
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime;

    // Callback de Unity, llama  cuando el inspector se actualiza
    private void OnValidate()
    {
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
		gravityScale = gravityStrength / Physics2D.gravity.y;
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
		
	}
}

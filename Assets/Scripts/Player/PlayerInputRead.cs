using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputRead : MonoBehaviour
{
    private PlayerInput playerInput;
    
    // Inputs
    private Vector2 moveInput;
    
    // Flags públicos para leer desde otros scripts
    public Vector2 MoveInput => moveInput;
    public bool MaskInputPos { get; private set; }
    public bool MaskInputNeg { get; private set; }
    public bool MaskInputAcept { get; private set; }
    public bool NormalPressed { get; private set; }
    public bool ActionPressed { get; private set; }     // Salto (Down)
    public bool ActionReleased { get; private set; }    // Salto (Up)
    public bool ActionHeld { get; private set; }        // Salto (Hold)
    public bool ActionBuy {get; private set;}
    void Awake()
    {
        playerInput = new PlayerInput();  
    }

    void OnEnable()
    {
        playerInput.Player.Enable();
        SubscribeToInputActions();  
    }

    private void OnDisable()
    {
        UnsubscribeFromInputActions();
        playerInput.Player.Disable();
    }

    private void OnDestroy()
    {
        playerInput?.Dispose();
    }

    private void Update()
    {
        // lee movimiento constante
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        // refresh de flags
        ResetOneShotInputs();
    }

    private void SubscribeToInputActions()
    {
        // Salto
        playerInput.Player.ActionBtn.started += OnJumpStarted;
        playerInput.Player.ActionBtn.canceled += OnJumpCanceled;
        
        // Máscaras
        playerInput.Player.ChangeMaskNeg.performed += OnChangeMaskNeg;
        playerInput.Player.ChangeMaskPos.performed += OnChangeMaskPos;
        playerInput.Player.ChangeMaskAccept.performed += OnChangeMaskAcepted;
        playerInput.Player.TakeOffMask.performed += OnResetMask; 

        playerInput.Player.BuyAction.performed += OnBuyAction;
    }

    private void UnsubscribeFromInputActions()
    {
        playerInput.Player.ActionBtn.started -= OnJumpStarted;
        playerInput.Player.ActionBtn.canceled -= OnJumpCanceled;
        playerInput.Player.ChangeMaskNeg.performed -= OnChangeMaskNeg;
        playerInput.Player.ChangeMaskPos.performed -= OnChangeMaskPos;
        playerInput.Player.ChangeMaskAccept.performed -= OnChangeMaskAcepted;
        playerInput.Player.TakeOffMask.performed -= OnResetMask; 
        playerInput.Player.BuyAction.performed -= OnBuyAction;
    }


    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        ActionPressed = true;
        ActionHeld = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        ActionReleased = true;
        ActionHeld = false;
    }

    private void OnChangeMaskNeg(InputAction.CallbackContext context)
    {   
        MaskInputNeg = true;
    }

    private void OnChangeMaskPos(InputAction.CallbackContext context)
    {
        MaskInputPos = true;
    }

    private void OnChangeMaskAcepted(InputAction.CallbackContext context)
    {
        MaskInputAcept = true;
    }

    private void OnResetMask(InputAction.CallbackContext context)
    {
        NormalPressed = true;
    }

    private void OnBuyAction(InputAction.CallbackContext context)
    {
        ActionBuy = true;
    }

    // lLimpia los inputs que solo deben durar un frame
    private void ResetOneShotInputs()
    {
        ActionPressed = false;
        ActionReleased = false;
        MaskInputAcept = false;
        MaskInputPos = false;
        MaskInputNeg = false;
        NormalPressed = false;
        ActionBuy = false;
    }

    public float GetHorizontalInput() => moveInput.x;
    public float GetVerticalInput() => moveInput.y;
    public bool HasHorizontalInput() => Mathf.Abs(moveInput.x) > 0.01f;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMask : MonoBehaviour
{
    public PlayerController playerController;
    public GameCanva maskUI;
    private int temporaryMaskIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.actualPlayerState == ActualPlayerState.NoActive) return;

        // 1. Navegar entre máscaras (Q/E o L1/R1)
        if (playerController.pInput.MaskInputPos) ChangeSelection(1);
        if (playerController.pInput.MaskInputNeg) ChangeSelection(-1);

        // 2. Confirmar cambio (Enter / R2)
        if (playerController.pInput.MaskInputAcept) ConfirmMask();

        // 3. Quitar máscara (Ctrl / L2)
        if (playerController.pInput.NormalPressed) RemoveMask();

    }

    void ChangeSelection(int direction)
    {
        temporaryMaskIndex += direction;
        // Ciclar entre 1 y 3 (las máscaras disponibles)
        if (temporaryMaskIndex > 3) temporaryMaskIndex = 1;
        if (temporaryMaskIndex < 1) temporaryMaskIndex = 3;

        maskUI.UpdatePreview(temporaryMaskIndex);
    }

    void ConfirmMask()
    {
        // Bloqueo rápido con DOTween
        playerController.ChangePlayerState(ActualPlayerState.WaitingForMask);
        
        // Simulación de tiempo de cambio rápido (0.2s)
        DOVirtual.DelayedCall(0.2f, () => {
            playerController.ChangeMaskType((Masktype)temporaryMaskIndex);
            playerController.ChangePlayerState(ActualPlayerState.Active);
            maskUI.UpdateFinalMask(temporaryMaskIndex);
        });
    }

    void RemoveMask()
    {
        temporaryMaskIndex = 0;
        playerController.ChangeMaskType(Masktype.NoMask);
        maskUI.UpdateFinalMask(0);
        maskUI.UpdatePreview(0);
    }
}

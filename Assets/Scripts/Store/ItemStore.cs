using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ItemStore : MonoBehaviour
{
    [Header("Configuración")]
    public ItemSOBJ itemData; 
    public StoreManager manager;

    [Header("Visuales Mundo")]
    public SpriteRenderer itemSprite;
    public TMP_Text costText;
    public GameObject buyPrompt;

    [Header("Estado")]
    private bool isPlayerInRange;
    private bool isSold;
    private PlayerController currentPlayer;

    void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        if (buyPrompt != null) buyPrompt.SetActive(false);
        
        if (itemData != null) UpdateVisuals();
    }

    void Update()
    {
        if (isPlayerInRange && !isSold && currentPlayer != null && itemData != null)
        {
            if (currentPlayer.pInput.ActionBuy)
            {
                AttemptBuy();
            }
        }
    }

    public void Setup(ItemSOBJ newItemData, StoreManager newManager)
    {
        itemData = newItemData;
        manager = newManager;

        // Actualizamos las visuales inmediatamente
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (itemData != null)
        {
            if (itemSprite != null) itemSprite.sprite = itemData.icon;
            if (costText != null) costText.text = itemData.cost.ToString();
        }
    }

    void AttemptBuy()
    {
        // Llamamos al manager (que ya tiene la lógica de verificar dinero)
        bool success = manager.TryBuyItem(itemData);

        if (success)
        {
            MarkAsSold();
        }
        else
        {
            if(costText != null) 
                costText.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
        }
    }

    void MarkAsSold()
    {
        isSold = true;
        
        if (itemSprite != null) 
        {
            itemSprite.color = Color.gray; 
            itemSprite.transform.DOScale(0, 0.3f); 
        }

        if (costText != null) costText.text = "VENDIDO";
        if (buyPrompt != null) buyPrompt.SetActive(false);

        transform.DOPunchScale(new Vector3(0.2f, -0.2f, 0), 0.2f);
    }

    // --- DETECCIÓN DEL JUGADOR ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isSold) return;

        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            currentPlayer = collision.GetComponent<PlayerController>();

            if (buyPrompt != null) buyPrompt.SetActive(true);
            
            transform.DOShakeScale(0.2f, 0.1f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            currentPlayer = null;

            if (buyPrompt != null) buyPrompt.SetActive(false);
        }
    }
}

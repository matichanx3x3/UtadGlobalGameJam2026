using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoreManager : MonoBehaviour
{
    [Header("Generación de Ítems")]
    [Tooltip("Arrastra aquí TODOS los ScriptableObjects de ítems posibles")]
    public List<ItemSOBJ> allItemsPool; 
    
    [Tooltip("Los 3 puntos (Transforms) donde aparecerán los objetos")]
    public List<Transform> spawnPoints; 
    
    [Tooltip("El Prefab genérico del ítem (debe tener el script ItemStoreWorld)")]
    public GameObject itemWorldPrefab;

    [Header("Referencias")]
    public PlayerController playerController;
    public PlayerData playerData;

    void Start()
    {
        SpawnStoreItems();
    }

    void SpawnStoreItems()
    {
        if (itemWorldPrefab == null || spawnPoints.Count == 0 || allItemsPool.Count == 0)
        {
            Debug.LogWarning("StoreManager: Falta configurar prefabs, puntos o ítems.");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            // item al azar
            int randomIndex = Random.Range(0, allItemsPool.Count);
            ItemSOBJ randomItem = allItemsPool[randomIndex];

            GameObject newItemObject = Instantiate(itemWorldPrefab, spawnPoint.position, Quaternion.identity);

            // setup del item
            ItemStore itemScript = newItemObject.GetComponent<ItemStore>();
            if (itemScript != null)
            {
                itemScript.Setup(randomItem, this);
            }
        }
    }
    // buy
    public bool TryBuyItem(ItemSOBJ item)
    {
        // alcanza el dinero?
        if (playerData.scoreSaved >= item.cost)
        {
            playerController.UseScore(item.cost);

            ApplyItemEffect(item);

            return true; 
        }
        else
        {
            return false; 
        }
    }

    private void ApplyItemEffect(ItemSOBJ item)
    {
        switch (item.effectType)
        {
            case ItemEffectType.ScoreMultiplier:
                
                playerData.scoreMultiplier += Mathf.RoundToInt(item.amount);
                break;

            case ItemEffectType.HpBonus:
                
                playerData.hpBonus += Mathf.RoundToInt(item.amount);
                
                break;

            case ItemEffectType.AttackDamage:
                playerData.attackDamage += Mathf.RoundToInt(item.amount);
                break;
        }
    }
}
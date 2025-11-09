using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [System.Serializable]
    public class DropItem
    {
        public GameObject itemPrefab;
        public float dropChance;
    }
    public List<DropItem> dropItems;
    public float dropForce = 2f;
    public float dropRadius = 0.5f;
    public void DropLoot()
    {
        foreach (var dropItem in dropItems)
        {
            if (Random.value <= dropItem.dropChance)
            {
                Vector2 dropPosition = transform.position + Random.insideUnitSphere * dropRadius;
                GameObject droppedItem = Instantiate(dropItem.itemPrefab, dropPosition, Quaternion.identity);
                        
            }
        }
    }
}

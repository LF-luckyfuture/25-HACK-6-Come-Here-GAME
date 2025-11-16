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
    [Header("经验值设置")]
    public int expReward = 10;

    public void DropLoot()
    {
        // 掉落物品
        foreach (var dropItem in dropItems)
        {
            if (Random.value <= dropItem.dropChance)
            {
                Vector3 dropPosition = transform.position;
                dropPosition.y=transform.position.y;
                GameObject droppedItem = Instantiate(dropItem.itemPrefab, dropPosition, Quaternion.identity);
            }
        }

        // 添加经验值掉落
        GiveExperience();
    }

    private void GiveExperience()
    {
        if (Character.instance != null)
        {
            Character.instance.AddExp(expReward);
            Debug.Log($"敌人 {gameObject.name} 掉落 {expReward} 经验值");
        }
    }
}
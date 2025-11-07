using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public enum ItemType
    {
        EXP
    }
    public ItemType itemType;
    public float EXPAmount=1f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            Character character= other.GetComponent<Character>();
            if (character !=null )
            {
                switch (itemType)
                { 
                    case ItemType.EXP:
                        character.currentExp += (int)EXPAmount;
                        Destroy(gameObject);
                        break;
                }
            }
        }
    }
}

using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    public Character character;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            
        }
    }
}

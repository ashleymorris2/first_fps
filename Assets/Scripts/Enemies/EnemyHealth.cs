using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int currentHealth = 100;

    public void TakeDamage(int damageAmout)
    {
        currentHealth -= damageAmout;

        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}

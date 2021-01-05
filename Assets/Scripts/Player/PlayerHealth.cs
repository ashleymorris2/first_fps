using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int maxHealth;

    private int currentHealth;

    private float damageDelay = 1f;
    private float damageDelayCounter;

    private static PlayerHealth instance;


    public static PlayerHealth Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (damageDelayCounter > 0)
        {
            damageDelayCounter -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (damageDelayCounter <= 0)
        {
            currentHealth -= damageAmount;

            if (currentHealth <= 0)
            {
                gameObject.SetActive(false);
            }

            damageDelayCounter = damageDelay;
        }

    }
}

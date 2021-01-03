using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour

{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate, timeBetweenShots = 2f, timeToShoot = 1f;

    public float fireCount { private get; set; }
    public float shotWaitCounter { private get; set; }
    public float shootTimeCounter { private get; set; }


    void Start()
    {
        shootTimeCounter = timeToShoot;
        shotWaitCounter = timeBetweenShots;

        var bulletController = bullet.GetComponentInChildren<BulletController>();
        bulletController.isEnemyBullet = true;
    }

    internal void StartAttackCountDown()
    {
        shootTimeCounter = timeToShoot;
        shotWaitCounter = timeBetweenShots;
    }

    internal void FireAtPlayer()
    {
        if (shotWaitCounter > 0)
        {
            shotWaitCounter -= Time.deltaTime;

            if (shotWaitCounter <= 0)
            {
                shootTimeCounter = timeToShoot;
            }
        }
        else
        {
            shootTimeCounter -= Time.deltaTime;

            if (shootTimeCounter > 0)
            {
                fireCount -= Time.deltaTime;

                if (fireCount <= 0)
                {
                    fireCount = fireRate;

                    firePoint.LookAt(Player.instance.transform.position + new Vector3(0f, 1.5f, 0f));

                    Vector3 targetDirection = Player.instance.transform.position - transform.position;
                    float angle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

                    if (Mathf.Abs(angle) < 30f)
                    {
                        Instantiate(bullet, firePoint.position, firePoint.rotation);
                    }
                    else
                    {
                        shotWaitCounter = timeBetweenShots;
                    }
                }

                GetComponent<NavMeshAgent>().destination = transform.position;
            }
            else
            {
                shotWaitCounter = timeBetweenShots;
            }
        }
    }
}

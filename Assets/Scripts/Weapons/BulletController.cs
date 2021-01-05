using UnityEngine;

public class BulletController : MonoBehaviour
{

    [SerializeField] float meshSkinWidth = 0.1f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float bulletVelocity = 90;
    [SerializeField] float lifeTime = 8;
    [SerializeField] GameObject collisionEffect;
    [SerializeField] Transform bulletDirection;

    private float minimumMoveExtent;
    private float partialMoveExtent;
    private float sqrMinimumMoveExtent;
    private Vector3 previousPosition;

    private GameObject bullet;
    private Rigidbody bulletModel;
    private Collider bulletCollider;

    public bool isEnemyBullet { get; set; }
    public bool isPlayerBullet { get; set; }

    void Start()
    {
        bullet = transform.parent.gameObject;
        bulletModel = GetComponent<Rigidbody>();
        bulletCollider = bulletModel.GetComponent<Collider>();

        previousPosition = bulletModel.position;

        minimumMoveExtent = Mathf.Min(Mathf.Min(bulletCollider.bounds.extents.x, bulletCollider.bounds.extents.y), bulletCollider.bounds.extents.z);
        partialMoveExtent = minimumMoveExtent * (1.0f - meshSkinWidth);
        sqrMinimumMoveExtent = minimumMoveExtent * partialMoveExtent;
    }

    void FixedUpdate()
    {
        CheckForCollision();
        Move();
        Expire();
    }

    private void CheckForCollision()
    {
        Vector3 movementThisUpdate = bulletModel.position - previousPosition;
        float movementSqrMagnitude = movementThisUpdate.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumMoveExtent)
        {
            var movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);

            //Use a raycast to see what is infront of the bullet that isn't on this layermask
            if (Physics.Raycast(previousPosition, movementThisUpdate, out var hitInfo, movementMagnitude, ~layerMask))
            {
                if (!hitInfo.collider)
                    return;

                if (hitInfo.collider.isTrigger)
                    hitInfo.collider.SendMessage("OnTriggerEnter", bulletCollider); //Collider is a trigger let them handle OnTriggerEnter

                if (!hitInfo.collider.isTrigger)
                {
                    OnTriggerEnter(hitInfo.collider);//Collider doesn't have a trigger call our own OnTriggerEnter..
                    bulletModel.position = hitInfo.point - (movementThisUpdate / movementMagnitude) * partialMoveExtent;
                }
            }
        }

        previousPosition = bulletModel.position;
    }

    private void Move()
    {
        bulletModel.AddForce(bulletDirection.forward * bulletVelocity, ForceMode.VelocityChange);
        bullet.transform.position = bulletModel.position;
    }

    private void Expire()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(bullet);
            Debug.Log("Expired");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var damage = 25;

        if (other.gameObject.tag == "Enemy" && !isEnemyBullet)
        {
            other.gameObject.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            Debug.Log($"I {gameObject} isEnemyBullet = {isEnemyBullet} isPlayerBullet = {isPlayerBullet} Hit {other}");
        }

        if (other.gameObject.tag == "Headshot" && !isEnemyBullet)
        {

            other.transform.parent.GetComponent<EnemyHealth>().TakeDamage(damage * 2);
            Debug.Log($"I {gameObject} isEnemyBullet = {isEnemyBullet} isPlayerBullet = {isPlayerBullet} Hit {other} - HEADSHOT");
        }

        if (other.gameObject.tag == "Player" && !isPlayerBullet)
        {
            Debug.Log($"Hit player at {transform.position}");
            PlayerHealth.Instance.TakeDamage(damage);
        }

        Instantiate(collisionEffect, bullet.transform.position, bullet.transform.rotation);
        Destroy(bullet);
    }

}

using UnityEngine;

public class BulletController : MonoBehaviour
{

    [SerializeField] float meshSkinWidth = 0.1f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float bulletVelocity;
    [SerializeField] float lifeTime;
    [SerializeField] GameObject collisionEffect;
    [SerializeField] Transform bulletDirection;

    private float minimumMoveExtent;
    private float partialMoveExtent;
    private float sqrMinimumMoveExtent;
    private Vector3 previousPosition;

    private GameObject bullet;
    private Rigidbody bulletModel;
    private Collider bulletCollider;


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

                if (hitInfo.collider.isTrigger && hitInfo.distance < 0.5f)
                    hitInfo.collider.SendMessage("OnTriggerEnter", bulletCollider); //Collider is a trigger let them handle OnTriggerEnter

                if (!hitInfo.collider.isTrigger && hitInfo.distance < 0.1f)
                    OnTriggerEnter(hitInfo.collider); //Collider doesn't have a trigger and we've almost hit the collider, call out own OnTriggerEnter..

                if (!hitInfo.collider.isTrigger)
                    bulletModel.position = hitInfo.point - (movementThisUpdate / movementMagnitude) * partialMoveExtent;
            }
        }

        previousPosition = bulletModel.position;

        bulletModel.AddForce(bulletDirection.forward * bulletVelocity, ForceMode.VelocityChange);
        bullet.transform.position = bulletModel.position;

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(bullet);
            Debug.Log("Expired");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
            if(other.gameObject.tag == "Enemy")
            {
                Destroy(other.gameObject);
            }
            Instantiate(collisionEffect, bullet.transform.position + (bullet.transform.forward * ((-bulletVelocity * .015f) * Time.deltaTime)), bullet.transform.rotation);
            Destroy(bullet);
            Debug.Log($"Destroyed ----- I {gameObject} Hit {other}");
        
    }
}

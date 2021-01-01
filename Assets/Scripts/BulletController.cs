using UnityEngine;

public class BulletController : MonoBehaviour
{

    [SerializeField] float movementVelocity;
    [SerializeField] float lifeTime;

    [SerializeField] GameObject collisionEffect;

    [SerializeField] Transform bulletDirection;
    
    private Rigidbody bulletBody;
    private GameObject parentObject;


    void Start()
    {
        bulletBody = GetComponent<Rigidbody>();
        parentObject = transform.parent.gameObject;
    }


    void FixedUpdate()
    {
        bulletBody.AddForce(bulletDirection.forward * movementVelocity, ForceMode.VelocityChange);
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(parentObject);
            Debug.Log("Expired");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(collisionEffect, transform.position, transform.rotation);
        Destroy(parentObject);
        Debug.Log("Destroyed");
    }
}

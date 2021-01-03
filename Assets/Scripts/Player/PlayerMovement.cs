using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform playerCameraTransform;
    [SerializeField] Transform playerFeet;
    [SerializeField] LayerMask jumpableTerrain;

    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float gravityMultiplier = 2f;
    [SerializeField] float jumpPower;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float runSpeed = 12f;

    [SerializeField] [Range(0f, 0.5f)] float moveSmoothTime = 0.105f;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePoint;

    private float cameraPitch = 0.0f;
    private float standingHeight;

    private Vector3 playerFallVelocity;
    private Vector2 currentDirection = Vector2.zero;
    private Vector2 currentDirectionVelocity = Vector2.zero;

    private CharacterController playerController;

    private bool canJump, canDoubleJump;

    void Start()
    {
        playerController = GetComponent<CharacterController>();
        standingHeight = playerController.height;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleJumping();
        ApplyGravity();

        HandleShoot();
    }

    private void HandleMovement()
    {
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();

        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentDirectionVelocity, moveSmoothTime);

        var speedModifier = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            speedModifier = runSpeed;

        Vector3 movementVelocity = (transform.right * currentDirection.x + transform.forward * currentDirection.y) * speedModifier;
        playerController.Move(movementVelocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        cameraPitch -= mouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerCameraTransform.localEulerAngles = Vector3.right * cameraPitch;
        playerController.transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
    }

    private void HandleJumping()
    {

        canJump = Physics.OverlapSphere(playerFeet.position, .15f, jumpableTerrain).Length > 0;

        if (canJump)
        {
            canDoubleJump = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            playerFallVelocity.y = jumpPower;
            canDoubleJump = true;
        }

        else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            playerFallVelocity.y = jumpPower;
            canDoubleJump = false;
        }

        playerController.Move(playerFallVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        playerFallVelocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        if (playerController.isGrounded)
            playerFallVelocity.y = Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        playerController.Move(playerFallVelocity * Time.deltaTime);
    }

    private void HandleShoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Check to see if there is anything to shoot at and direct our bullets towards it
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out var hit, 50f))
            {
                if (Vector3.Distance(playerCameraTransform.position, hit.point) > 1f)
                {
                    firePoint.LookAt(hit.point);
                }
            }
            else
            {
                firePoint.LookAt(playerCameraTransform.position + (playerCameraTransform.forward * 30));
            }

            var bulletController = bullet.GetComponentInChildren<BulletController>();
            bulletController.isPlayerBullet = true;

            Instantiate(bullet, firePoint.position, firePoint.rotation);
        }
    }

}

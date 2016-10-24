using UnityEngine;

public class bl_SimplePlayerController : MonoBehaviour
{
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public Transform Head = null;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 LastImpactDirection = Vector3.forward;
    Vector3 lookat;

    void OnEnable()
    {
        bl_DamageDelegate.OnIndicator += OnImpact;
    }

    void OnDisable()
    {
        bl_DamageDelegate.OnIndicator -= OnImpact;
    }

    void OnImpact(bl_IndicatorInfo info)
    {
        LastImpactDirection = info.Direction;
    }

    void OnControllerColliderHit(ControllerColliderHit c)
    {
        if(c.transform.GetComponent<Rigidbody>() != null)
        {
            c.transform.GetComponent<Rigidbody>().AddForce(transform.forward * 10, ForceMode.Impulse);
        }
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            Vector3 stickDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Get camera rotation.    

            Vector3 CameraDirection = Camera.main.transform.forward;
            CameraDirection.y = 0.0f; // kill Y
            Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);

            // Convert joystick input in Worldspace coordinates
            moveDirection = referentialShift * stickDirection;

            if (Head != null)
            {
                 lookat = Vector3.Lerp(lookat, LastImpactDirection, 2 * Time.deltaTime);
                Head.LookAt(lookat);
            }

            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f; // Kecepatan gerakan karakter
    public float rotationSpeed = 500f; // Kecepatan rotasi karakter

    public Rigidbody rb;

    Vector3 movement;
    public Animator animator;

    [SerializeField] private FixedJoystick joystick;

    [SerializeField] private Transform cameraTransform;

    private bool isJumping = false;

    public LayerMask groundLayer;
    Collider ground;
    Bounds bounds;
    Vector3 positionBeforeFalling;
    public float checkDistance = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (PlayerPrefs.HasKey("PlayerX") && PlayerPrefs.HasKey("PlayerY") && PlayerPrefs.HasKey("PlayerZ"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");
            transform.position = new Vector3(x, y, z);
        }
    }

    private void FixedUpdate()
    {
        // float joystickHorizontalInput = Input.GetAxis("Horizontal");
        // float joystickVerticalInput = Input.GetAxis("Vertical");

        float joystickHorizontalInput = joystick != null ? joystick.Horizontal : 0f;
        float joystickVerticalInput = joystick != null ? joystick.Vertical : 0f;

        if (!isJumping)
        {
            // Play animation
            bool isWalking = !Mathf.Approximately(joystickVerticalInput, 0) || !Mathf.Approximately(joystickHorizontalInput, 0);
            if (animator != null) animator.SetBool("isWalking", isWalking);

            // Move and rotate player based on input
            MovePlayerFree(joystickHorizontalInput, joystickVerticalInput);
        }

    }

    private void MovePlayerFree(float hInput, float vInput)
    {
        if (!IsGrounded())
        {
            Debug.Log("is not grounded");
            rb.velocity = Vector3.zero;
            transform.position = positionBeforeFalling;
        }
        else{
            GetGroundCollider();
            positionBeforeFalling = transform.position;
        }
        // Mengatur arah rotasi berdasarkan input horizontal
        Vector3 rotationDirection = new Vector3(0, hInput, 0);
        if (rotationDirection.y != 0)
        {
            Quaternion targetRotation = Quaternion.Euler(0, rotationDirection.y, 0) * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Mengatur gerakan maju atau mundur berdasarkan input vertikal
        if (vInput != 0)
        {
            Vector3 moveDirection = transform.forward * vInput;
            float magnitude = Mathf.Clamp01(moveDirection.magnitude) * moveSpeed;
            moveDirection.Normalize();

            transform.Translate(moveDirection * magnitude * Time.fixedDeltaTime, Space.World);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Object"))
        {
            rb.velocity = Vector3.zero;
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance, groundLayer))
        {
            return true;
        }
        return false;
    }

    void GetGroundCollider()
    {
        RaycastHit hit;
        // Raycast ke bawah dari posisi player
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance, groundLayer))
        {
            ground = hit.collider; // Mengembalikan collider yang terkena raycast
        }
        // bounds = null; // Tidak ada ground di bawah player
    }
}

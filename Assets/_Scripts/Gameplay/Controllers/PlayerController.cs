using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f; // Kecepatan gerakan karakter
    public float rotationSpeed = 500f; // Kecepatan rotasi karakter

    private Rigidbody rb;

    Vector3 movement;
    public Animator animator;

    [SerializeField] private FixedJoystick joystick;

    [SerializeField] private Transform cameraTransform;

    private bool isJumping = false;
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
}

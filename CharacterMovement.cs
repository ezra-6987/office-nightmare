using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    public bool IsMoving { get; private set; } = false; // Tracks if the character is moving
    public Vector2 MoveDirection { get; private set; } = Vector2.zero; // Tracks movement direction
    public Boss bossScript; // Reference to the Boss script

    private Vector2 inputDirection = Vector2.zero; // Stores player input

    void Start()
    {
        // Ensure Rigidbody2D is attached
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing! Please attach a Rigidbody2D component.");
        }
        else
        {
            rb.freezeRotation = true; // Prevent unwanted rotation
        }

        // Ensure Boss script is assigned
        if (bossScript == null)
        {
            Debug.LogError("Boss script is not assigned! Please reference it in the Inspector.");
        }
    }

    void Update()
    {
        // Capture input in Update for responsiveness
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Update movement state
        IsMoving = inputDirection.magnitude > 0.01f;
        MoveDirection = inputDirection;

        // Pass the movement direction to the Boss script
        if (bossScript != null)
        {
            bossScript.raycastDirection = inputDirection;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Apply movement in FixedUpdate for physics consistency
        if (rb != null)
        {
            rb.velocity = inputDirection * moveSpeed;
        }
    }
}

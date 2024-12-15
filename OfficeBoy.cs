using UnityEngine;

public class OfficeBoy : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public GameObject character; // Reference to the character (if needed)

    private CharacterMovement characterMovement; // Reference to the CharacterMovement script
    private Rigidbody2D rb; // Reference to Rigidbody2D

    void Start()
    {
        // Try to get the CharacterMovement component
        characterMovement = GetComponent<CharacterMovement>();
        if (characterMovement == null)
        {
            Debug.LogError("CharacterMovement component is missing! Please attach a CharacterMovement component.");
        }

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("RigidBody2D is missing! Please attach a Rigidbody2D component.");
        }
    }

    void FixedUpdate()
    {
        // Ensure GameManager instance is available
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is missing!");
            return;
        }

        // Check if the office boy is allowed to move
        if (GameManager.Instance.isOfficeBoyMoving)
        {
            MovePlayer(); // Allow movement
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop movement if not allowed
        }
    }

    // Method to move the player based on input
    private void MovePlayer()
    {
        // Get input for horizontal and vertical movement
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        // Create a normalized movement vector
        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;

        // Apply velocity to the Rigidbody2D for movement
        rb.velocity = movement * moveSpeed;
    }
}

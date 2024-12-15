using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public GameObject[] waypoints; // Array of waypoints
    public float moveSpeed = 5f; // Movement speed
    public float stopDistance = 0.1f; // Distance to stop at a waypoint
    private int currentWaypointIndex = 0; // Index of the current waypoint

    public GameObject officeBoy; // Reference to the office boy object
    public LayerMask raycastLayerMask; // Layer mask for raycasting
    public float raycastDistance = 100f; // Raycast distance

    private Rigidbody2D rb; // Rigidbody for velocity-based movement
    public bool isTriggered = false; // Flag to check if event is already triggered
    public Vector2 raycastDirection; // Direction for raycasting

    public Transform head; // Transform for the head object
    public Transform body; // Transform for the body object

    public AudioSource audioSource; // AudioSource for playing sounds
    public AudioClip moveSound; // Sound when the boss is moving
    public AudioClip triggerSound; // Sound when the boss meets the office boy
    public AudioClip headTurnSound; // Sound when the head turns to look at an object

    private void Start()
    {
        // Ensure waypoints are assigned
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned to the boss character.");
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on the Boss object.");
            enabled = false;
            return;
        }

        // Ensure AudioSource is assigned
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned. Please attach an AudioSource component to the Boss object.");
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isBossMoving && !isTriggered)
        {
            MoveToWaypoint();
        }

        PerformRaycast();
    }

    private void MoveToWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        // Get the direction to the current waypoint
        Vector3 direction = (waypoints[currentWaypointIndex].transform.position - transform.position).normalized;

        // Rotate body towards movement direction
        RotateBody(direction);

        // Play movement sound
        PlaySound(moveSound);

        // Move the boss using velocity
        rb.velocity = new Vector2(direction.x, direction.y) * moveSpeed;

        // Check if the boss has reached the waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < stopDistance)
        {
            rb.velocity = Vector2.zero; // Stop the boss
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop to the next waypoint
        }
    }

    private void RotateBody(Vector3 direction)
    {
        // Update body rotation based on movement direction
        if (body != null && direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            body.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void PerformRaycast()
    {
        if (officeBoy == null) return;

        // Perform raycast in the specified direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, raycastDistance, raycastLayerMask);

        if (hit.collider != null)
        {
            // Play head turn sound
            PlaySound(headTurnSound);

            // Update head rotation to look at the detected object
            RotateHead(hit.point);

            if (hit.collider.gameObject == officeBoy && !isTriggered)
            {
                TriggerEventOnMeeting();
            }
        }
    }

    private void RotateHead(Vector3 lookAtPoint)
    {
        if (head != null)
        {
            Vector3 direction = (lookAtPoint - head.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            head.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void TriggerEventOnMeeting()
    {
        isTriggered = true; // Prevent multiple triggers
        rb.velocity = Vector2.zero; // Stop the boss when triggered
        Debug.Log("The boss has met the office boy!");

        // Stop boss movement and allow office boy to move (update GameManager state)
        GameManager.Instance.isBossMoving = false;
        GameManager.Instance.isOfficeBoyMoving = true;

       // Play trigger sound only when the event is triggered
       PlaySound(triggerSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the raycast in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)raycastDirection * raycastDistance);

        // Visualize waypoints
        if (waypoints != null)
        {
            Gizmos.color = Color.blue;
            foreach (var waypoint in waypoints)
            {
                Gizmos.DrawSphere(waypoint.transform.position, 0.2f);
            }
        }
    }

    private IEnumerator WaitAtWaypoint(float waitTime)
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitTime);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
}

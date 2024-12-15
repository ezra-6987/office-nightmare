using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public GameObject[] waypoints; // Array of waypoints
    public float moveSpeed = 2f; // Movement speed
    public float stopDistance = 0.1f; // Distance to stop at a waypoint
    private int currentWaypointIndex = 0; // Index of the current waypoint

    public GameObject officeBoy; // Reference to the office boy object
    public LayerMask raycastLayerMask; // Layer mask for raycasting
    public float raycastDistance = 100f; // Raycast distance

    private Rigidbody2D rb; // Rigidbody for velocity-based movement
    public bool isTriggered = false; // Flag to check if event is already triggered
    public Vector2 raycastDirection; // Direction for raycasting

    private void Start()
    {
        // Ensure waypoints are assigned
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned to the boss character.");
            enabled = false;
            return;
        }

        Debug.Log("Number of waypoints: " + waypoints.Length);

        // Initialize the Rigidbody
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on the Boss object.");
            enabled = false;
            return;
        }

        Debug.Log("isTriggered: " + isTriggered);

        Debug.Log("Boss script enabled: " + enabled);

    }

    private void FixedUpdate()
    {
        Debug.Log("isBossMoving: " + GameManager.Instance.isBossMoving);
        if (GameManager.Instance.isBossMoving && !isTriggered)
        {
             MoveToWaypoint();
        }

        Debug.Log("FixedUpdate Called"); // Add this to confirm the method is running
        if (!isTriggered)
        {
            MoveToWaypoint(); // Move between waypoints
            Debug.Log($"Boss moving towards waypoint {currentWaypointIndex}");

        }

        PerformRaycast(); // Perform raycasting to detect objects
    }

    private void MoveToWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        // Get the direction to the current waypoint
        Vector3 direction = (waypoints[currentWaypointIndex].transform.position - transform.position).normalized;
        Debug.Log($"Moving towards: {waypoints[currentWaypointIndex].name}, Direction: {direction}");

        // Move the boss using velocity
        rb.velocity = new Vector2(direction.x, direction.y) * moveSpeed;

        // Check distance
        float distance = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position);
        Debug.Log($"Distance to waypoint: {distance}");

        // Check if the boss has reached the waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < stopDistance)
        {
            rb.velocity = Vector2.zero; // Stop the boss
            Debug.Log("Reached waypoint: " + currentWaypointIndex);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop to the next waypoint
        }
    }

    private void PerformRaycast()
    {
        if (officeBoy == null) return;

        // Perform raycast in the specified direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, raycastDistance, raycastLayerMask);

        if (hit.collider != null)
        {
            Debug.Log("Boss sees: " + hit.collider.name);

            // Trigger event if the office boy is detected
            if (hit.collider.gameObject == officeBoy && !isTriggered)
            {
                TriggerEventOnMeeting();
            }
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

        // Handle further actions like animations, sounds, or UI updates here
    }

    private void OnDrawGizmos()
    {
        // Visualize the raycast in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)raycastDirection * raycastDistance);

        // Visualize waypoints
        if (waypoints != null)
        Debug.Log($"Waypoints Count: {waypoints.Length}");
        {
            Gizmos.color = Color.blue;
            foreach (var waypoint in waypoints)
            {
                if (waypoint != null)
                Debug.Log("Waypoint Position: " + waypoint.transform.position);
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

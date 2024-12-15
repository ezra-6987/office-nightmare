using UnityEngine;

public class InputHandler : MonoBehaviour
{
    void Update()
    {
        // Toggle movement states with the "T" key
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Instance.isBossMoving = !GameManager.Instance.isBossMoving;
            GameManager.Instance.isOfficeBoyMoving = !GameManager.Instance.isOfficeBoyMoving;
        }
    }
}

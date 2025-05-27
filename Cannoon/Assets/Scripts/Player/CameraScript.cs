using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public Vector3 positionAdjustments;

    private void LateUpdate()
    {
        // Follows Player
        transform.position = new Vector3(player.transform.position.x + positionAdjustments.x,
            player.transform.position.y + positionAdjustments.y,
            player.transform.position.z + positionAdjustments.z);
    }
}

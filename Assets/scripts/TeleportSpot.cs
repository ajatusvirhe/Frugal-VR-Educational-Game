using UnityEngine;

public class TeleportSpot : Interactive
{
    public new void Interact()
    {
        //RaycastHit hit = CameraInteract.GetLatestHit();
        CameraInteract.GetPlayerTransform().position =
            transform.position * CameraInteract.GetPlayerHeight();
    }
}

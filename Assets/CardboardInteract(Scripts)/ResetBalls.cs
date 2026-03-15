using UnityEngine;

public class ResetBalls : Interactive
{
    public new void Interact()
    {
        base.Interact(); // optional: logs "Interacted with ..."
        
        Ball[] balls = FindObjectsOfType<Ball>();
        foreach (Ball ball in balls)
        {
            ball.ResetPosition();
        }

        Debug.Log("All balls have been reset!");
    }
}
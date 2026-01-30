using UnityEngine;

public class ObjectInteractionTask : MonoBehaviour
{
    public LocomotionBase Player;

    private void OnTriggerEnter(Collider other)
    {
        if (other && other.CompareTag("Player"))
        {
            Player.CollideWithOIT(this.gameObject);
        }
    }
}

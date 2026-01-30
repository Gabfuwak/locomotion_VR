using UnityEngine;

public class Banner : MonoBehaviour
{
    public LocomotionBase Player;

    private void OnTriggerEnter(Collider other)
    {
        if (other && other.CompareTag("Player"))
        {
            Player.CollideWithBanner(this.gameObject);
        }
    }

}

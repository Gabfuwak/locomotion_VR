using UnityEngine;

public class Coin : MonoBehaviour
{
    public LocomotionBase Player;
    private void OnTriggerEnter(Collider other)
    {
        if (other && other.CompareTag("Player"))
        {
            Player.CollideWithCoin(this.gameObject);
        }
    }
}

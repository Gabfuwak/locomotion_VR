using UnityEngine.AI;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform player;

    NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        if (player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
}

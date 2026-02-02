using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBallCameraController : MonoBehaviour
{
    [SerializeField] Vector3 offset;

    RollingBallPlayerController player;

    private void Awake()
    {
        player = FindAnyObjectByType<RollingBallPlayerController>();
    }

    private void Update()
    {
        gameObject.transform.position = player.transform.position + offset;
    }
}

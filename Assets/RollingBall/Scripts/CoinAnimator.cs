using UnityEngine;

public class CoinAnimator : MonoBehaviour
{
    float time = 0f;
    readonly float baseY = 1f;

    private void Update()
    {
        time += Time.deltaTime;

        Vector3 newPosition = transform.position;
        newPosition.y = baseY + 0.3f * Mathf.Cos(2f * time);
        transform.position = newPosition;

        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }
}

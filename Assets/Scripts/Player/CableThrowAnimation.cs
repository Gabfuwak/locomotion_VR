using System.Collections;
using UnityEngine;
using GogoGaga.OptimizedRopesAndCables;

public class CableThrowAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private float cableLengthMultiplier = 1.5f;

    [Header("Rope Settings")]
    [SerializeField] private Material ropeMaterial; // Optional: Material for the rope
    [SerializeField] private float ropeWidth = 0.2f;

    private bool isAnimating = false;

    /// <summary>
    /// Triggers the cable throw animation from origin to target
    /// </summary>
    /// <param name="origin">Starting point of the cable</param>
    /// <param name="target">End point (where raycast hit)</param>
    /// <param name="onComplete">Callback when animation completes</param>
    public void PlayThrowAnimation(Vector3 origin, Vector3 target, System.Action onComplete = null)
    {
        Debug.Log($"PlayThrowAnimation called - origin: {origin}, target: {target}");
        if (isAnimating)
        {
            Debug.LogWarning("Animation already in progress, ignoring");
            return;
        }
        StartCoroutine(ThrowAnimationCoroutine(origin, target, onComplete));
    }

    private IEnumerator ThrowAnimationCoroutine(Vector3 origin, Vector3 target, System.Action onComplete)
    {
        isAnimating = true;

        // Calculate actual distance
        float actualDistance = Vector3.Distance(origin, target);
        float extendedLength = actualDistance * cableLengthMultiplier;

        // Create rope GameObject dynamically
        GameObject ropeObj = new GameObject("CableThrow_Temp");
        ropeObj.transform.position = origin;

        // Create start and end point transforms
        GameObject startObj = new GameObject("StartPoint");
        startObj.transform.SetParent(ropeObj.transform);
        startObj.transform.position = origin;

        GameObject endObj = new GameObject("EndPoint");
        endObj.transform.SetParent(ropeObj.transform);
        endObj.transform.position = target;

        // Add and configure the Rope component
        Rope ropeComponent = ropeObj.AddComponent<Rope>();
        ropeComponent.SetStartPoint(startObj.transform, true);
        ropeComponent.SetEndPoint(endObj.transform, true);
        ropeComponent.ropeLength = extendedLength;
        ropeComponent.ropeWidth = ropeWidth;
        ropeComponent.linePoints = 30; // More segments for smoother serpentine look
        ropeComponent.stiffness = 100f; // Lower stiffness for more flexible rope
        ropeComponent.damping = 8f;
        ropeComponent.midPointWeight = 3f; // Adds more sag/bunching in the middle

        // Configure LineRenderer (automatically added by Rope component)
        LineRenderer lineRenderer = ropeObj.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            if (ropeMaterial != null)
            {
                lineRenderer.material = ropeMaterial;
            }
            // Make sure it's visible
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
        }

        Debug.Log($"Cable throw animation started: {extendedLength}m rope from {origin} to {target}");

        // Wait for animation duration
        yield return new WaitForSeconds(animationDuration);

        // Clean up
        Destroy(ropeObj);
        isAnimating = false;

        Debug.Log("Animation complete (with rope), calling onComplete callback");
        // Notify completion
        onComplete?.Invoke();
    }

    public bool IsAnimating => isAnimating;
}

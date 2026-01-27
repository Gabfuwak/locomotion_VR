using UnityEngine;

public class AnchorThrowing : MonoBehaviour
{
    [SerializeField] GameObject anchorPointA;
    [SerializeField] GameObject anchorPointB;
    [SerializeField] LayerMask building;
    [SerializeField] float maxDistance;
    [SerializeField] LayerMask ground;
    LayerMask combinedMask;

    Camera cam;
    public bool Attached = false;
    public Vector3 AnchorPos => anchorPointA.transform.position;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        combinedMask = building | ground;
    }

    private void Update()
    {
        if (!Attached && Input.GetButtonDown("Fire1"))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, combinedMask))
            {
                anchorPointA.transform.position = hit.point;
                anchorPointA.SetActive(true);
                Attached = true;
            }
        } else if (Input.GetButtonDown("Fire2"))
        {
            anchorPointA.SetActive(false);
            Attached = false;
        }
    }
}

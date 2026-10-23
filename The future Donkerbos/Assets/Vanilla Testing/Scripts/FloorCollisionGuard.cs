
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class FloorCollisionGuard : MonoBehaviour
{
    [Header("Floor Settings")]
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private float floorClearance = 0.02f;

    private Collider[] chairColliders;

    private void Awake()
    {
        chairColliders = GetComponentsInChildren<Collider>(true);
    }

    private void LateUpdate()
    {
        float lowestPoint = float.PositiveInfinity;

        foreach (Collider col in chairColliders)
        {
            if (col == null || !col.enabled || col.isTrigger)
                continue;

            lowestPoint = Mathf.Min(
                lowestPoint,
                col.bounds.min.y
            );
        }

        if (float.IsPositiveInfinity(lowestPoint))
            return;

        // Find the floor directly beneath the chair
        Vector3 rayOrigin = transform.position + Vector3.up * 2f;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            10f,
            floorLayer))
        {
            float minimumY = hit.point.y + floorClearance;

            if (lowestPoint < minimumY)
            {
                transform.position += Vector3.up *
                    (minimumY - lowestPoint);
            }
        }
    }
}

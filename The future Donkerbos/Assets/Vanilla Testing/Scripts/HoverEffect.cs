
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField] private float hoverHeight = 0.15f;
    [SerializeField] private float hoverSpeed = 2f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 45f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        // Hover up and down
        float offset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.localPosition = startPosition + Vector3.up * offset;

        // Rotate around the Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}

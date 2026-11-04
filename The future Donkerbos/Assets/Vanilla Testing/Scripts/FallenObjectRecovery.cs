
using UnityEngine;

public class FallenObjectRecovery : MonoBehaviour
{
    [Header("Recovery Settings")]
    [SerializeField] private float fallDistance = 1.5f;
    [SerializeField] private float returnHeight = 0.2f;

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Rigidbody rb;

    private void Start()
    {
        // Remember the original position and rotation
        startingPosition = transform.position;
        startingRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Check if the object has fallen too far
        if (transform.position.y <
            startingPosition.y - fallDistance)
        {
            RecoverObject();
        }
    }

    private void RecoverObject()
    {
        Debug.Log(gameObject.name + " has been recovered!");

        if (rb != null)
        {
            // Stop movement before repositioning
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = startingPosition +
                          Vector3.up * returnHeight;

            rb.rotation = startingRotation;
        }
        else
        {
            transform.position = startingPosition +
                                 Vector3.up * returnHeight;

            transform.rotation = startingRotation;
        }
    }
}

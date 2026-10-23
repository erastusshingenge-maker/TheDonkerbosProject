using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField]
    private ConstructionStageManager stageManager;

    [Header("Placement Settings")]
    [SerializeField] private string requiredTag;
    [SerializeField] private GameObject placedObject;

    private bool isPlaced = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlaced) return;

        GameObject incomingObject =
            other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (!incomingObject.CompareTag(requiredTag))
            return;

        PlaceObject(incomingObject);
    }

    private void PlaceObject(GameObject incomingObject)
    {
        isPlaced = true;

        // Show the correctly positioned object
        placedObject.SetActive(true);

        // Disable the placement trigger
        GetComponent<Collider>().enabled = false;

        // Notify construction manager
        if (stageManager != null)
        {
            stageManager.ObjectPlaced();
        }

        // Remove the original object
        SafeDestroy(incomingObject);

        Debug.Log("Object placed successfully!");
    }

    private void SafeDestroy(GameObject incomingObject)
    {
        if (incomingObject != null)
        {
            Destroy(incomingObject);
        }

        // Hide the placement trigger without destroying it
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }
}


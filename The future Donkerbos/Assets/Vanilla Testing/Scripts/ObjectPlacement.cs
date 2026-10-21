
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

        // Find the object entering the trigger
        GameObject incomingObject =
            other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        // Check if it is the correct object
        if (!incomingObject.CompareTag(requiredTag))
            return;

        PlaceObject(incomingObject);
    }

    private void PlaceObject(GameObject incomingObject)
    {
        isPlaced = true;

        // Enable the correctly positioned object
        placedObject.SetActive(true);

        // Disable the placement trigger
        GetComponent<Collider>().enabled = false;

        // Remove the original object
        incomingObject.SetActive(false);
        Destroy(incomingObject);

        stageManager.ObjectPlaced();
        Destroy(gameObject);

        Debug.Log("Object placed successfully!");
    }
}



using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PolePlacement : MonoBehaviour
{
    [Header("Placement Settings")]
    public GameObject fixedPole;
    public string poleTag = "Pole";

    private bool isPlaced = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlaced) return;

        // Find the pole object
        XRGrabInteractable grab =
            other.GetComponentInParent<XRGrabInteractable>();

        if (grab == null) return;

        // Check if it is a pole
        if (!grab.CompareTag(poleTag)) return;

        // Place immediately
        PlacePole(grab.gameObject);
    }

    private void PlacePole(GameObject pole)
    {
        isPlaced = true;

        // Enable the pre-positioned pole
        fixedPole.SetActive(true);

        // Disable the trigger
        GetComponent<Collider>().enabled = false;

        // Remove the original pole
        pole.SetActive(false);
        Destroy(pole);

        Debug.Log("Pole placed!");
    }
}

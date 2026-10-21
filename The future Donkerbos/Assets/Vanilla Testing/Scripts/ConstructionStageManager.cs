
using UnityEngine;

public class ConstructionStageManager : MonoBehaviour
{
    [Header("Current Stage")]
    [SerializeField] private int requiredPlacements;

    [Header("Next Stage")]
    [SerializeField] private GameObject[] nextStageTriggers;

    private int completedPlacements = 0;

    private void Start()
    {
        // Disable the next stage initially
        foreach (GameObject trigger in nextStageTriggers)
        {
            trigger.SetActive(false);
        }
    }

    public void ObjectPlaced()
    {
        completedPlacements++;

        Debug.Log("Construction progress: " +
            completedPlacements + "/" + requiredPlacements);

        if (completedPlacements >= requiredPlacements)
        {
            UnlockNextStage();
        }
    }

    private void UnlockNextStage()
    {
        foreach (GameObject trigger in nextStageTriggers)
        {
            trigger.SetActive(true);
        }

        Debug.Log("Next construction stage unlocked!");
    }
}


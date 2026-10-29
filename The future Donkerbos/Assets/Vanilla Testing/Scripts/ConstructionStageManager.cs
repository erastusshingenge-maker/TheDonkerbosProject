using UnityEngine;

public class ConstructionStageManager : MonoBehaviour
{
    [Header("Current Stage")]
    [SerializeField] private int requiredPlacements;
    [SerializeField] private bool isFinalStage = false;

    [Header("Next Stage")]
    [SerializeField] private GameObject[] nextStageTriggers;

    [Header("Scene Transition")]
    [SerializeField] private ConstructionSceneTransition sceneTransition;

    public bool isHutFinished { get; private set; } = false;

    private int completedPlacements = 0;

    private void Start()
    {
        foreach (GameObject trigger in nextStageTriggers)
        {
            trigger.SetActive(false);
        }
    }

    public void ObjectPlaced()
    {
        if (completedPlacements >= requiredPlacements)
            return;

        completedPlacements++;

        Debug.Log("Construction progress: " +
            completedPlacements + "/" + requiredPlacements);


        if (completedPlacements >= requiredPlacements)
        {
            if (isFinalStage)
            {
                isHutFinished = true;

                Debug.Log("Construction completed!");

                // Start the scene transition
                if (sceneTransition != null)
                {
                    sceneTransition.StartSceneTransition();
                }
            }
            else
            {
                UnlockNextStage();
            }
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
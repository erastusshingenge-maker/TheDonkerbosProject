
using System.Collections;
using UnityEngine;

public class VRPlayerSpawn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private OVRCameraRig cameraRig;
    [SerializeField] private MonoBehaviour firstPersonLocomotor;
    [SerializeField] private Transform playerController;

    [Header("Settings")]
    [SerializeField] private float spawnDelay = 0.3f;

    private IEnumerator Start()
    {
        if (playerSpawnPoint == null ||
            cameraRig == null ||
            playerController == null ||
            firstPersonLocomotor == null)
        {
            Debug.LogError("VRPlayerSpawn: Missing references!");
            yield break;
        }

        // Disable locomotion during spawning.
        firstPersonLocomotor.enabled = false;

        yield return new WaitForSeconds(spawnDelay);

        Transform head = cameraRig.centerEyeAnchor;

        if (head == null)
        {
            Debug.LogError("VRPlayerSpawn: Head missing!");
            firstPersonLocomotor.enabled = true;
            yield break;
        }

        // Rotate the rig to match the spawn direction.
        float yawDifference = Mathf.DeltaAngle(
            head.eulerAngles.y,
            playerSpawnPoint.eulerAngles.y
        );

        cameraRig.transform.Rotate(
            0f, yawDifference, 0f, Space.World
        );

        yield return null;

        // Move the headset horizontally to the spawn point.
        Vector3 difference =
            playerSpawnPoint.position - head.position;

        difference.y = 0f;

        cameraRig.transform.position += difference;

        // Align the locomotion controller horizontally.
        // It is a child of the rig, so use its local position.
        Vector3 controllerLocalPosition =
            playerController.localPosition;

        Vector3 headLocalPosition =
            playerController.parent.InverseTransformPoint(
                head.position
            );

        controllerLocalPosition.x = headLocalPosition.x;
        controllerLocalPosition.z = headLocalPosition.z;

        playerController.localPosition = controllerLocalPosition;

        Physics.SyncTransforms();

        yield return new WaitForFixedUpdate();

        // Re-enable ground following and joystick movement.
        firstPersonLocomotor.enabled = true;

        Debug.Log("VRPlayerSpawn: Spawn completed!");
    }
}

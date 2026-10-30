using UnityEngine;

// Put this on the Nurse (same object as the Signal Receiver).
// In each Signal Receiver reaction, pick  NurseAnimationSignals -> PlayWalking / PlayStanding / PlayHealing.
public class NurseAnimationSignals : MonoBehaviour
{
    public Animator animator;
    public float blend = 0.2f;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Go(string state)
    {
        if (animator == null) { Debug.LogWarning("NurseAnimationSignals: no Animator found."); return; }
        animator.CrossFadeInFixedTime(state, blend);
    }

    public void PlayStanding() { Go("Standing"); }
    public void PlayWalking()  { Go("Walking"); }
    public void PlayHealing()  { Go("Healing"); }
}

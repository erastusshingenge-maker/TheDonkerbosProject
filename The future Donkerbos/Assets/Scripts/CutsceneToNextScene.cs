// Put this on the GameObject that has the Video Player.
// When the video finishes, the next scene loads by itself.
// If the video fails to play, it moves on after a safety timeout so no child is left in the dark.
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class CutsceneToNextScene : MonoBehaviour
{
    [Tooltip("Exact name of the scene to load when the video ends, e.g. Present Dorm")]
    public string nextScene = "Present Dorm";
    [Tooltip("Seconds to wait before starting, so the headset has settled")]
    public float startDelay = 1.0f;
    [Tooltip("Move on after this many seconds even if the video never finishes")]
    public float safetyTimeout = 60f;

    VideoPlayer vp;
    bool loading;
    float timer;

    void Start()
    {
        vp = GetComponent<VideoPlayer>();
        vp.playOnAwake = false;
        vp.isLooping = false;
        vp.loopPointReached += _ => Next();
        vp.errorReceived += (_, msg) => { Debug.LogWarning("Cutscene video error: " + msg); Next(); };
        vp.Prepare();
        Invoke(nameof(Play), startDelay);
    }

    void Play() { if (!loading) vp.Play(); }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > safetyTimeout) Next();
    }

    void Next()
    {
        if (loading) return;
        loading = true;
        SceneManager.LoadScene(nextScene);
    }
}

using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text counter;
    [SerializeField] private int targetFps;

    public float updateInterval = 0.5f;

    private float accum = 0f;
    private int frames = 0;
    private float timeleft;

    void Start()
    {
        timeleft = updateInterval;
        if (targetFps>0)
        {
            Application.targetFrameRate = targetFps;
        }
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeleft <= 0.0)
        {
            float fps = accum / frames;
            counter.text = "FPS:" + Mathf.Round(fps);

            accum = 0.0f;
            frames = 0;
            timeleft = updateInterval;
        }
    }
}
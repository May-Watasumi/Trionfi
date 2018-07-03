using UnityEngine;
using System.Collections;

public class FPSCounter: SingletonMonoBehaviour<FPSCounter>
{
    int frameCount;
    float prevTime;

    void Start()
    {
        frameCount = 0;
        prevTime = 0.0f;
    }

    void Update()
    {
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            ShowFPS(frameCount / time);
//            Debug.LogFormat("{0}fps", frameCount / time);

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }

    public virtual void ShowFPS(float time)
    {
        Debug.LogFormat("{0}fps", time);
    }
}

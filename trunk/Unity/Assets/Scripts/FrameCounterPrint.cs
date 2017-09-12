using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCounterPrint : MonoBehaviour {


    public float updateInterval = 0.5f;
    public double lastInterval; // Last interval end time
    public int frames = 0; // Frames over current interval
    public float fps ; // Current FPS

    private GUIStyle mGUIStyle = new GUIStyle();
	// Use this for initialization
	void Start () {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        this.mGUIStyle.normal.textColor = Color.green;
        this.mGUIStyle.fontSize = 20;
    }
	
	// Update is called once per frame
	void Update ()
    {
        ++frames;
        var timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width * 0.5f - 100,10,200,150), "FPS : " + fps.ToString("f2"),this.mGUIStyle);
    }
}

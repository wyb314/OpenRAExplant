using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour {

	public GUIStyle guiStyle = new GUIStyle();

	public float updateInterval = 0.5F;
	private double lastInterval;
	private int frames = 0;
	private float fps;
	void Start() {
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;

		guiStyle.fontSize = 20;
		guiStyle.normal.textColor = Color.green;
        guiStyle.alignment = TextAnchor.UpperCenter;
	    

	}
	void OnGUI() {
		GUI.Label(new Rect(Screen.width * 0.5f-100,5,200,100), "" + fps.ToString("f2"),this.guiStyle);
	}
	void Update() {
		++frames;
		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > lastInterval + updateInterval) {
			fps = (float) (frames / (timeNow - lastInterval));
			frames = 0;
			lastInterval = timeNow;
		}

	    //if (Input.GetKeyDown(KeyCode.Q))
	    //{
     //       Vector3 rot = new Vector3(0.8f, .3f, 2);
	    //    rot = Vector3.zero;
     //       ThinksquirrelSoftware.Utilities.CameraShake.Shake(10, Vector3.one, rot, 0.10f, 30.0f, 0.08f, 1.0f, true, false);
	    //}
	}
}

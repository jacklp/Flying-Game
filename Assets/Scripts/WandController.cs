using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour {
	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
	public bool gripButtonDown = false;
	public bool gripButtonUp = false;
	public bool gripButtonPressed = false;

	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	public bool triggerButtonDown = false;
	public bool triggerButtonUp = false;
	public bool triggerButtonPressed = false;
	public Vector3 wingPos;
	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device controller{ get { return SteamVR_Controller.Input ((int)trackedObj.index); } } 

	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (controller == null) {
			Debug.Log ("Controller not initalized");
			return;
		}

		Debug.Log (trackedObj.transform.position);


		gripButtonDown = controller.GetPressDown (gripButton);
		gripButtonUp = controller.GetPressUp (gripButton);
		gripButtonPressed = controller.GetPress (gripButton);

		triggerButtonDown = controller.GetPressDown (triggerButton);
		triggerButtonUp = controller.GetPressUp (triggerButton);
		triggerButtonPressed = controller.GetPress (triggerButton);

		if (gripButtonDown) {
			Debug.Log("Grip Button was just pressed");
		}

		if (gripButtonUp) {
			Debug.Log("Grip Button was just released");
		}

		if (triggerButtonDown) {
			Debug.Log ("trigger Button was just pressed");
		}

		if (triggerButtonUp) {
			Debug.Log ("trigger Button was just released");
		}
	}
}

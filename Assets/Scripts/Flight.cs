using UnityEngine;
using System.Collections;

public class Flight : MonoBehaviour {

	public bool controlScheme;

	public GameObject left;
	public GameObject right;
	public GameObject head;
	public GameObject eyes;

	private Vector3 leftRelative;
	private Vector3 rightRelative;

	private bool rightWingOpen;
	private bool leftWingOpen;

	public float speed;
	public float boost_speed;
	private float boost;

	public Vector3 upRotation;
	public Vector3 downRotation;
	public Vector3 leftRotation;
	public Vector3 rightRotation;

	private SteamVR_TrackedObject leftTrackedObj;
	private SteamVR_Controller.Device leftController{ get { return SteamVR_Controller.Input ((int)leftTrackedObj.index); } } 
	private Valve.VR.EVRButtonId leftTriggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

	private SteamVR_TrackedObject rightTrackedObj;
	private SteamVR_Controller.Device rightController{ get { return SteamVR_Controller.Input ((int)rightTrackedObj.index); } }
	private Valve.VR.EVRButtonId rightTriggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

	private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

	// Use this for initialization
	void Start () {
		leftTrackedObj = left.GetComponent<SteamVR_TrackedObject> ();
		rightTrackedObj = right.GetComponent<SteamVR_TrackedObject> ();
		boost = 0;
	}
	//2 control schemes


	// Update is called once per frame
	void Update () {

		GetControllerMovements ();
		moveBird ();
	}

	private string handleClicks(SteamVR_Controller.Device controller)
	{
		if (controller.GetPressDown (touchPad)) {
			float touch_x = controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			float touch_y = controller.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y;

			if (touch_x < -0.5)
				return "Left";
			else if (touch_x > 0.5)
				return "Right";
			else if (touch_y > 0.5)
				return "Up";
			else
				return "Down";
		}

		return "";
	}

	void moveBird(){

		//FORWARD MOVEMENT
		if (leftWingOpen & rightWingOpen) {


			if (leftController.GetPress (leftTriggerButton) && !rightController.GetPress (rightTriggerButton)) {
				boost = boost_speed * 2f;
				Debug.Log ("left Trigger Pressed");
			} 
			if (rightController.GetPress (rightTriggerButton) && !leftController.GetPress (leftTriggerButton)) {
				boost = boost_speed * 2f;
				Debug.Log ("right Trigger Pressed");
			} 
			if (leftController.GetPress (leftTriggerButton) && rightController.GetPress (rightTriggerButton)) {
				boost = boost_speed *4f;
				Debug.Log ("both triggers pressed");
			} else {
				Debug.Log ("none pressed");
				boost = 0;
			}
			transform.position += transform.forward * Time.deltaTime * (speed + boost);

		}

		//LEFT ROTATION
		if (leftWingOpen && !rightWingOpen) {
			transform.Rotate (leftRotation);
		} 

		//RIGHT ROTATION
		if (rightWingOpen && !leftWingOpen) {
			transform.Rotate (rightRotation);
		} 

		//PITCH UP

		if (handleClicks (leftController) == "Up" || handleClicks (rightController) == "Up" ) {
			Debug.Log ("pitch up");
			transform.Rotate (upRotation);
		}

		if(handleClicks(leftController) == "Down" || handleClicks (rightController) == "Down"){
			Debug.Log ("pitch down");
			transform.Rotate (downRotation);
		}
			
	}


	void GetControllerMovements(){

		leftRelative = head.transform.InverseTransformPoint (left.transform.position);
		rightRelative = head.transform.InverseTransformPoint (right.transform.position);

		//Debug.Log ("LEFT" + leftRelative);
		//Debug.Log ("RIGHT" + rightRelative);

		if (leftRelative.x < -0.2 && leftRelative.y < -0.6) {
			leftWingOpen = true;
		//	Debug.Log ("Left Wing Open");
		} else {
			leftWingOpen = false;
		//	Debug.Log ("Left Wing Closed");
		}

		if (rightRelative.x > 0.2 && rightRelative.y < -0.6) {
		//	Debug.Log ("Right Wing Open");
			rightWingOpen = true;
		} else {
		//	Debug.Log ("Right Wing Closed");
			rightWingOpen = false;
		}
	}
}

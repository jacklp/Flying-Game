using UnityEngine;
using System.Collections;

public class PhysicsFlight : MonoBehaviour {


	private Vector3 currentAngle;

	public GameObject left;
	public GameObject right;
	public float thrust;
	public float rotationalFactor;
	public float angularDrag;

	private Rigidbody rigidbody;
	private float combinedMomentumUp;
	private float rightMomentumUp;
	private float leftMomentumUp;

	private SteamVR_TrackedObject leftTrackedObj;
	private SteamVR_Controller.Device leftController{ get { return SteamVR_Controller.Input ((int)leftTrackedObj.index); } } 
//	private Valve.VR.EVRButtonId leftTriggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

	private SteamVR_TrackedObject rightTrackedObj;
	private SteamVR_Controller.Device rightController{ get { return SteamVR_Controller.Input ((int)rightTrackedObj.index); } }

	private float leftToRightRelative;

	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
		rigidbody.angularDrag = angularDrag;
		leftTrackedObj = left.GetComponent<SteamVR_TrackedObject> ();
		rightTrackedObj = right.GetComponent<SteamVR_TrackedObject> ();

		currentAngle = transform.eulerAngles;
	}
	
	//RECEIVE INPUT HERE
	void Update () {
		combinedMomentumUp = Mathf.Abs(rightController.velocity.y + leftController.velocity.y);
		rightMomentumUp = Mathf.Abs (rightController.velocity.y);
		leftMomentumUp = Mathf.Abs (leftController.velocity.y);

		leftToRightRelative = left.transform.position.y -right.transform.position.y;


		//Debug.Log ("momentum" + rightMomentumUp);
		//Debug.Log (currentAngle.y + rightMomentumUp - leftMomentumUp);
		/*
		currentAngle = new Vector3(
			Mathf.LerpAngle(currentAngle.x, currentAngle.x, Time.deltaTime),
			Mathf.LerpAngle(currentAngle.y, currentAngle.y + (leftToRightRelative.x * RotationalFactor), Time.deltaTime),
			Mathf.LerpAngle(currentAngle.z, currentAngle.z, Time.deltaTime));

		transform.eulerAngles = currentAngle;
		transform.Translate(Vector3.forward * Time.deltaTime);
*/
	}

	//MOVE PHYSICS OBJECTS
	void FixedUpdate(){
		rigidbody.AddForce (transform.forward * combinedMomentumUp * thrust);
		rigidbody.AddForce(transform.up * combinedMomentumUp*thrust);

		Debug.Log (leftToRightRelative);

		if (leftToRightRelative > 0.2 || leftToRightRelative < -0.2) {
			rigidbody.AddTorque (Vector3.up * leftToRightRelative * rotationalFactor, ForceMode.Impulse);
		//	rigidbody.velocity = new Vector3 (0, rigidbody.velocity.magnitude * transform.up, rigidbody.velocity.magnitude * transform.forward); 
		//	rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.magnitude * transform.up.y, rigidbody.velocity.z * transform.forward
			rigidbody.velocity = rigidbody.velocity.magnitude * transform.forward;
			//	rigidbody.velocity.
		}

	}
}

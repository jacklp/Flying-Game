using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class oldGameController : MonoBehaviour {

	public int numCars;

	public GameObject carPrefab;

	[SerializeField] private bool addAICars = false;

	public List<Transform> checkpoints;

	private const int numberRacers = 8;

	public List<String> cars;
	private List<Transform> carTransforms;

	// Initialized cars
	private bool raceInitialized = false;

	// Race start (after 3-2-1-GO)
	private bool raceStarted = false;

	private float LAP_MULTIPLIER;
	private float CHECKPOINT_MULTIPLIER;
	private List<float> distances;

	public int numberOfLaps = 3;

	void Awake()
	{
		LayerMask carMask = LayerMask.GetMask("Car");
		LayerMask partsMask = LayerMask.GetMask("Parts");
		if (carMask < 32 && partsMask < 32) {
			Physics.IgnoreLayerCollision(carMask.value, partsMask.value, true);
		}
	}

	void Start() {
		cars = new List<CarData>(numberRacers);
		carTransforms = new List<Transform>(numberRacers);

		LAP_MULTIPLIER = checkpoints.Count * 10000;
		CHECKPOINT_MULTIPLIER = 1000;
	}

	void Update() {
		if (raceInitialized) {
			UpdateCheckpoints();
			UpdatePositions();
		}

		if (raceStarted) {
			UpdateLapTimes();
			CheckWrongWay();
		}
	}

	public void InstantiateCars(int ind) {
		Vector3 pos;
		GameObject car;

		numCars = 0;
		if (ind == 0) {
			numCars = 1;
		} else if (ind == 1) {
			numCars = 2;
		} else {
			numCars = 4;
		}

		distances = new List<float>();

		for (int i = 0; i < numCars; ++i) {

			pos = new Vector3((10.0f * i) + transform.position.x, transform.position.y, transform.position.z);
			car = Instantiate(carPrefab, pos, transform.rotation) as GameObject;
			car.transform.name = "Car " + (i + 1);
			car.GetComponent<InputController>().SetCarId(i + 1);
			car.GetComponent<Engine>().carId = i + 1;
			Camera cam = car.transform.FindChild("mainCamera").GetComponent<Camera>();


			CarData carData = new CarData(i + 1, i + 1);
			cars.Add(carData);
			carTransforms.Add(car.transform);

			distances.Add(0.0f);

			switch (numCars) {
			case 2:
				if (i == 0)
				{
					cam.rect = new Rect(new Vector2(0.0f, 0.5f), new Vector2(1.0f, 0.5f));

				} else
				{
					cam.rect = new Rect(new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.5f));

				}
				break;
			case 4:
				if (i == 0)
				{
					cam.rect = new Rect(new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f));

				} else if (i == 1)
				{
					cam.rect = new Rect(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

				} else if (i == 2)
				{
					cam.rect = new Rect(new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.5f));

				} else
				{
					cam.rect = new Rect(new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.5f));

				}
				break;
			default:
				break;
			}
		}

		if (addAICars) {
			for (int i=0; i<numberRacers-numCars; ++i) {
				//TODO: Instantiate AI cars
			}
		}
		raceInitialized = true;
		StartCoroutine("Countdown");
	}

	IEnumerator Countdown() {
		yield return new WaitForSeconds(2.0f);
		for (int i=3; i>=0; --i) {
			for (int j=0; j<carTransforms.Count; ++j) {
				Text countdownText = carTransforms[j].FindChild("Canvas").FindChild("countdown").GetComponent<Text>();
				if (i == 0) {
					countdownText.text = "GO !";
					GameObject.Find("BgMusic").GetComponent<AudioSource>().Play();
				} else {
					countdownText.text = "" + i;
				}
			}

			yield return new WaitForSeconds(1.0f);
		}

		for (int i=0; i<carTransforms.Count; ++i) {
			carTransforms[i].GetComponent<InputController>().isActive = true;
			Text countdownText = carTransforms[i].FindChild("Canvas").FindChild("countdown").GetComponent<Text>();
			countdownText.text = "";
		}
		raceStarted = true;
	}

	private void UpdateCheckpoints() {
		for (int i=0; i<cars.Count; ++i) {
			Vector3 carCheckpoit = checkpoints[cars[i].nextCheckpoint].position - carTransforms[i].position;
			Vector3 carLocalRight = checkpoints[cars[i].nextCheckpoint].InverseTransformDirection(checkpoints[cars[i].nextCheckpoint].right);
			Vector3 carLocalForward = checkpoints[cars[i].nextCheckpoint].InverseTransformDirection(checkpoints[cars[i].nextCheckpoint].forward);
			float dot = Vector3.Dot(carCheckpoit.normalized, carLocalForward);
			if (dot < 0.0f && carCheckpoit.magnitude < 170.0f) {
				//Debug.Log("CP: " + (cars[i].nextCheckpoint));
				cars[i].previousCheckpointDistance = 0.0f;
				cars[i].wrongWayDistance = 0.0f;

				if (cars[i].nextCheckpoint == 0 && cars[i].currentCheckpoint == checkpoints.Count-1) {
					cars[i].currentLap = cars[i].currentLap + 1;

					if (cars[i].currentLap > numberOfLaps) {
						Text countdownText = carTransforms[i].FindChild("Canvas").FindChild("countdown").GetComponent<Text>();
						countdownText.verticalOverflow = VerticalWrapMode.Overflow;
						countdownText.horizontalOverflow = HorizontalWrapMode.Overflow;
						countdownText.fontSize = 18;
						countdownText.text = "Race Over!\nFinal Position: " + cars[i].currentPosition;
						carTransforms[i].GetComponent<InputController>().isActive = false;
						carTransforms[i].GetComponent<InputController>().SetThrottle(0.0f);
						carTransforms[i].GetComponent<Engine>().StopCar();
						raceStarted = false;
					}

					if (cars[i].bestLapTime == 0.0f || cars[i].currentLapTime < cars[i].bestLapTime) {
						cars[i].bestLapTime = cars[i].currentLapTime;
					}

					cars[i].currentLapTime = 0.0f;
				}

				cars[i].currentCheckpoint = cars[i].nextCheckpoint;
				cars[i].nextCheckpoint = (cars[i].nextCheckpoint + 1) % checkpoints.Count;
			}
		}
	}

	private void UpdatePositions() {
		for (int i = 0; i < cars.Count; ++i) {
			float cpdist = (checkpoints[cars[i].currentCheckpoint].position - carTransforms[i].position).magnitude;
			float distance = cars[i].currentLap * LAP_MULTIPLIER + (cars[i].currentCheckpoint+1) * CHECKPOINT_MULTIPLIER + cpdist;
			distances[i] = distance;
		}

		for (int i=0; i<cars.Count; ++i) {
			int pos = 1;
			for (int j=0; j<cars.Count; ++j) {
				if (distances[i] < distances[j]) {
					++pos;
				}
			}
			cars[i].currentPosition = pos;
		}
	}

	private void UpdateLapTimes() {
		for (int i=0; i<cars.Count; ++i) {
			cars[i].currentLapTime += Time.deltaTime;
		}
	}

	private void CheckWrongWay() {
		for (int i=0; i<cars.Count; ++i) {
			float cpdist = (carTransforms[i].position - checkpoints[cars[i].nextCheckpoint].position).magnitude;
			if (cars[i].previousCheckpointDistance < cpdist && cars[i].previousCheckpointDistance != 0.0f) {
				if (cars[i].wrongWayDistance != 0) {
					//Debug.Log("CPDIST: " + cpdist + " WWD: " + cars[i].wrongWayDistance + "   CP: " + cars[i].currentCheckpoint + " NCP: " + cars[i].nextCheckpoint);
					float dt = Mathf.Abs(cpdist - Mathf.Abs(cars[i].wrongWayDistance));
					if (dt > 75.0f) {
						//show wrong way text
						//Debug.Log("WRONG WAY " + dt + " Current: " + cars[i].currentCheckpoint + " Next: " + cars[i].nextCheckpoint);
					}

					if (dt > 150.0f) {
						//reset car to checkpoint
						carTransforms[i].GetComponent<InputController>().SetThrottle(0);
						carTransforms[i].GetComponent<Engine>().StopCar();
						carTransforms[i].position = checkpoints[cars[i].currentCheckpoint].position + Vector3.up * 1.5f;
						carTransforms[i].rotation = checkpoints[cars[i].currentCheckpoint].rotation;
						//Debug.Log("RESET CAR " + dt);
					}

				} else {
					cars[i].wrongWayDistance = cpdist;
				}

			} else {
				cars[i].wrongWayDistance = 0.0f;
			}
			//Debug.Log("PREV CP: " + cars[i].previousCheckpointDistance + "    WRW: " + cars[i].wrongWayDistance);
			cars[i].previousCheckpointDistance = cpdist;
		}
	}

	public List<CarData> GetCars() {
		return cars;
	}

	public List<Transform> GetCheckpoints() {
		return checkpoints;
	}

}

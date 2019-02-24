using UnityEngine;
using System.Collections;

/// <summary>
/// Main camera controller keeps the camera focused on the selected character, feature, or item.
/// When it comes time to have cinematics or dialog, a different camera will be used.
/// </summary>
public class MainCameraController : MonoBehaviour {

//	private UIManager UI;
	public Transform selection;
	public Transform inspection;
	public bool freeCamera = false;
	public float distance = 6.0f;
	public float height = 8.0f;
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	public float rotateSpeed = 100;
	public Vector3 cameraOldPosition = new Vector3
		(0.0f, 8.0f, -8.0f);

	void Start() {
	//	UI = FindObjectOfType<UIManager> ();
	}

	void FixedUpdate () {

		// Is there zooming
		CheckZoomLevel ();

	}

	void LateUpdate () {

		if (!freeCamera) {
			FollowTarget ();
		} else {

			// ---------------------------------------------
			// TODO: Set up free camera movement method
			// ---------------------------------------------
		}
	}




	void CheckZoomLevel () {
		float zoom = Input.GetAxis ("Mouse ScrollWheel");
		if (zoom > 0.0f) {
		
			// -----------------
			// TODO: Zoom in.
			// -----------------

		} else if (zoom < 0.0f) {

			// -----------------
			// TODO: Zoom out.
			// -----------------

		}
	}

	void FollowTarget () {

		float wantedRotationAngle = transform.eulerAngles.y;
		PrepareRotation (wantedRotationAngle);

		// Calculate the current rotation angles
		float wantedHeight = selection.position.y + height;
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, 
			rotationDamping * Time.deltaTime);

		// Damp the height
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		// Convert the angle into a rotation
		var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);


		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = selection.position;
		transform.position -= currentRotation * Vector3.forward * distance;

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x,currentHeight,transform.position.z);

		// Always look at the target
		transform.LookAt(selection);

	}

	private void PrepareRotation (float wantedAngle) {
		if (Input.GetKey ("q"))
			wantedAngle -= rotateSpeed;
		else if (Input.GetKey ("e"))
			wantedAngle += rotateSpeed;
	}
}
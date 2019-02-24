using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

	public Camera mainCamera;

	void Update() {
		transform.LookAt (transform.position + mainCamera.transform.rotation * Vector3.forward,
			mainCamera.transform.rotation * Vector3.up);
	}
}

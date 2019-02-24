using UnityEngine;
using System.Collections;

/// <summary>
/// Right mouse click menu is the class responsible for placing the RightMouseClickMenu canvas at
/// the mouse coordinates, populating it with the appropriate command buttons, and making it
/// visible when the user holds the right mouse button down for a specified length of time.  It is
/// meant to be placed on the main camera.
/// </summary>
public class RightMouseClickMenu : MonoBehaviour {

	public float timeRequired = 0.3f;
	private float timer = 0.0f;
	public bool isEnabled = true;
	public bool isDisplayed = false;

	// Update is called once per frame
	void Update () {
		if (isEnabled && !isDisplayed) {
			if (Input.GetMouseButton (1)) {
				timer += Time.deltaTime;
			} else {
				timer = 0.0f;
			}

			if (timer >= timeRequired) {
				Debug.Log ("held down");
				isDisplayed = true;
				timer = 0.0f;	// Re-initialize
			}
		}
	}
}

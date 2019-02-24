using UnityEngine;
using System.Collections;

/// <summary>
/// MouseOverDisplay displays appropriate information using the UI when the user hovers 
/// their mouse over an actor.
/// </summary>

public class MouseOverDisplay : MonoBehaviour {

	public float displayFor = 1.0f;

	private Canvas displayCanvas;
	private float timer = 0.0f;

	// Initialize the private variables and hide the canvas.
	void Awake() {
		Transform canvasTransform = this.transform.Find ("MouseOverDisplayCanvas");
		displayCanvas = canvasTransform.GetComponent<Canvas>();
		displayCanvas.enabled = false;
	}

	// If the canvas is enabled, keep it enabled for 'displayFor' seconds, then hide it.
	public void Update () {
		if (displayCanvas.enabled) {
			timer += Time.deltaTime;
			if (timer >= displayFor) {
				displayCanvas.enabled = false;
			}
		}
	}

	// Enable the canvas and set timer to 0..
	void OnMouseEnter () {
		displayCanvas.enabled = true;
		timer = 0.0f;

	}

	// Keep the timer at 0 if the mouse is still over the actor.
	void OnMouseOver () {
		timer = 0.0f;
	}















	/*
	 THIS IS HOW YOU MAKE GAME-OBJECTS APPEAR AND DISAPPEAR BASED ON CONDITIONS.
	private Color nameColorDisplay 		= new Color(1.0f, 1.0f, 1.0f, 1.0f);
	private Color nameColorFade 		= new Color(0.0f, 0.0f, 0.0f, 0.0f);

	private bool nameEnabledByMouse 	= false;
	private bool nameEnabledByKey 		= false;

	private Transform Name;
	private Renderer NameRenderer;

	void Awake () {
		Name = transform.Find("ObjectName");
		NameRenderer = Name.GetComponent<Renderer> ();

		NameRenderer.material.color = nameColorFade;
	}

	void Update () {
-----------------------
		 TO ENABLE THIS, ASSIGN ALT KEY BY NAME "ShowNamesTest"
         if (Input.GetButtonDown ("ShowNamesTest"))
         {
             Title_EnabledByKey = true;
             Custom_TitleEnable();
         }
         
         if (Input.GetButtonUp ("ShowNamesTest"))
         {
             Title_EnabledByKey = false;
             Custom_TitleDisable();
         }
-----------------------

	}

	void OnMouseEnter () {
		nameEnabledByMouse = true;
		NameEnable ();
	}

	void OnMouseExit () {
		nameEnabledByMouse = false;
		if (nameEnabledByKey)
			return;
		NameDisable ();
	}

	void NameEnable () {
		Debug.Log ("enabling...");
		NameRenderer.material.color = nameColorDisplay;
	}

	void NameDisable () {
		Debug.Log ("disabling...");
		for (var n = 0.0f; n < 0.5f; n += Time.deltaTime) {
			if (nameEnabledByMouse || nameEnabledByKey) {
				return;
			} else {
				NameRenderer.material.color = Color.Lerp (nameColorDisplay, nameColorFade, n / 0.5f);
			}
		}
	}
*/
}

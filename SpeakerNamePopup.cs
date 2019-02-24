using UnityEngine;
using UnityEditor;
using System.Collections;

public class SpeakerNamePopup : PopupWindowContent {

	public string speakerName;

	public override Vector2 GetWindowSize() {
		return new Vector2 (235, 20);
		//GUI.FocusControl ("SpeakerName");
	}

	public override void OnGUI(Rect rect) {
		//GUI.SetNextControlName ("SpeakerName");
		speakerName = GUILayout.TextField ("", 30);

	}

	public override void OnOpen() {
		Debug.Log ("Popup opened: " + this);
	}

	public override void OnClose() {
		Debug.Log ("Popup closed: " + this);
	}

	public string GetName() {
		return speakerName;
	}
}

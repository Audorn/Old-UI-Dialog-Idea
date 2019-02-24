using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public Canvas MainCanvas;
	public Canvas OptionsCanvas;

	void Awake()
	{
		OptionsCanvas.enabled = false;
	}

	public void OptionsOn()
	{
		Debug.Log ("Options");
		OptionsCanvas.enabled = true;
		MainCanvas.enabled = false;
	}

	public void BackOn()
	{
		Debug.Log ("Back");
		OptionsCanvas.enabled = false;
		MainCanvas.enabled = true;
	}

	public void LoadOn()
	{
		Debug.Log ("Play");
		SceneManager.LoadScene ("Arena");
	}

	public void QuitOn()
	{
		Application.Quit ();
	}
}

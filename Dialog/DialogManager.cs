using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace WanzerWeave.DialogSystem {



	/// <summary>
	/// Controls the dialog canvas and processes a dialog map.
	/// </summary>
	public class DialogManager : MonoBehaviour {

		[SerializeField] Canvas dialogCanvas;				// dialog user interface
		[SerializeField] Image dialogDisplay;				// Image that holds the recent dialog and scrolls if required
		[SerializeField] Image responsesDisplay;		// Image that holds the current response options

		[SerializeField] Image lineOfDialog;				// Image prefab that will display node data to user
		// Contents: 	tact (Image to contain tact border) -> leftPortrait (Image to contain Party Leader portrait)
		//						line (Text to contain the fullLine variable)
		//						color (Image to contain color border) -> rightPortrait (Image to contain npc portrait)

		[SerializeField] Image response;						// Image prefab that will display a response that can be clicked
		// Contents:	tact (Image to contain tact)
		//						text (Text to contain the shortLine variable)


		[SerializeField] Dialog dialog;				// dialog component of npc being communicated with

		[SerializeField] Line currentLine;		// current line of dialog


		//[SerializeField] bool pcInDialog = false;	// 



		IEnumerator ProcessDialog(Dialog dialog) {

			// display the line in currentLine
			Image lineToDisplay = Instantiate(lineOfDialog) as Image;
			lineToDisplay.transform.SetParent (dialogDisplay.transform, false);

			// copy the fullLine to the line component in the instance of the prefab
			Text line = lineToDisplay.transform.Find ("line").GetComponent<Text>();
			line.text = dialog.lines [dialog.startingLine].fullLine;

			// make a list of met prereqs
			List<ushort> metPrereqs = new List<ushort> ();
		//	foreach (Coroutine prereq in currentLine.prereqs) {

		//	}

			// make a list of prereqs that are met
			// check those lines for shortLine values (to see if they are player responses)
			// if they aren't then there should only be one prereq path available, 
			// make a series of responses from the prereq RefIds and child them to the response panel
			// wait for player input
			// 


			return null;
		}


		/// <summary>
		/// Starts the dialog.
		/// </summary>
		/// <param name="dialog">Dialog component of the target npc.</param>
		public void StartDialog(Dialog dialog) {

			// clear the dialogDisplay
			foreach (Transform child in dialogDisplay.transform)
				GameObject.Destroy (child.gameObject);

			// clear the responses
			foreach (Transform child in responsesDisplay.transform)
				GameObject.Destroy (child.gameObject);

			// activate the dialog canvas and set the first line of dialog
			dialogCanvas.gameObject.SetActive(true);
			currentLine = dialog.lines [dialog.startingLine];

			// call the coroutine that manages the dialog
			StartCoroutine(ProcessDialog(dialog));

			// find the first line of dialog
			// display the first line of dialog

			// if the children of that line of dialog have content in their shortLine variables show possible responses
			// otherwise show the correct child of the current line of dialog

			// loop
		}

	}



}

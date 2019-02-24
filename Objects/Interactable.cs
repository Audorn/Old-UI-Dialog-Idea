using UnityEngine;
using System.Collections;
using WanzerWeave.DialogSystem;

namespace WanzerWeave.UI {



	public class Interactable : MonoBehaviour {

		[SerializeField] DialogManager dialogManager;

		//public bool isMenuEnabled = true;
		//public bool isMenuDisplayed = false;

		// Get the dialog manager
		void Start () {
			dialogManager = FindObjectOfType<DialogManager> ();
		}
		
		// Update is called once per frame
		void Update () {
			// ---
			// TODO: Display the RightClickMenu when isMenuDisplayed == true;
			// ---
		}

		/// <summary>
		/// Called when user presses the 'interact' key (default 'f') while targeting an object with an Interactable
		/// component within range.
		/// </summary>
		public void Interact() {

			// prepare the possibile interactions
			Dialog targetDialog = GetComponent<Dialog> ();

			/// Interaction priority:
			/// 	Non-Hostile, with dialog 		- Dialog.StartDialog();
			/// 	Hostile, capable of attack 	- ***.Attack();
			/// 	Can be picked up						- ***.Take();
			/// 	Has accessible inventory		- ***.Open();
			/// 	Can be activated						- ***.Activate();

			// does this object have dialog?
			Debug.Log("testing");
			if (targetDialog /* && isNotHostile */ ) {
				Debug.Log ("has dialog");
				dialogManager.StartDialog (targetDialog);
			}

			Debug.Log ("Default action completed!");
		}

		/*
		public void DisplayMenu() {
			isMenuDisplayed = true;
			Debug.Log ("Menu displayed!");
			isMenuDisplayed = false;
			Debug.Log ("Menu hidden!");
		}
		*/
	}



}
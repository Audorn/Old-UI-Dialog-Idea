using UnityEngine;
using System.Collections;

namespace WanzerWeave.UI {


	/// <summary>
	/// Watches for and reacts to user input.  The defaults are:
	/// </summary>
	public class InputManager : MonoBehaviour {

		/// LMB (Fire1) 			== attack
		/// RMB (Fire2) 			== ability
		/// CMB (Fire3) 			== 'unassigned'
		/// MouseWheel 				== 'unassigned', mabye switch to first or third person?
		/// w,s (Vertical) 		== movement forward, back
		/// a,d (Horizontal)	== movement left, right
		/// f (Interact)			== interact with object
		/// space (Tactical)	== dialate time and zoom out to tactical view

		[SerializeField] private MainCameraController mainCamera;
		[SerializeField] private Interactable target;
		[SerializeField] private ActorController actor;

		public float holdTimeForMenu = 1.0f;
		private float timer;

		/// <summary>
		/// Watch for user input.
		/// </summary>
		void Update () {

			// LMB - Attack, RMB - Ability
			CheckLeftClick ();
			CheckRightClick ();

		}

		/// <summary>
		/// Calls Selection.Select() of the target if it has a Selection component, otherwise
		/// reverts selection to the party leader if appropriate.
		/// </summary>
		void CheckLeftClick () {
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 100.0f)) {
					Selection target = hit.transform.GetComponent<Selection> ();
					if (target) {
						actor = hit.transform.GetComponent<ActorController> ();
						target.Select ();
					} else {
						Debug.Log ("Target has not Selection component!");
						Debug.Log ("Check to see if selection is on party leader, revert if appropriate.");
					}
				} else {
					Debug.Log ("Raycast hit no GameObjects!");
				}
			}
		}

		/// <summary>
		/// Calls ActorController.Move(), UserCommand.DefaultAction, or 
		/// UserCommand.RightClickMenu() as appropriate.
		/// </summary>
		void CheckRightClick () {
			if (Input.GetMouseButtonDown (1)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 100.0f)) {

					// Did the user right-click on something valid?
					target = hit.transform.GetComponent<Interactable> ();
					if (target) {


						target.Interact ();

						/*
						// Watch how long they are holding the right-mouse button and call a menu
						// or default action when appropriate.
						if (target.isMenuEnabled && !target.isMenuDisplayed) {
							timer = Time.time;
							StartCoroutine (TimeRightClick ());
						}
						*/

					} else {

						// No interactable object hit, must be a move order
						actor.Move (hit.point);
						Debug.Log ("Interactable component not found!");
					}
				} else {
					Debug.Log ("Raycast hit no GameObject!");
				}
			}
		}

		private IEnumerator TimeRightClick() {

			while (timer != 0.0f) {
				if (Input.GetMouseButtonUp (1)) {

					// Button was held long enough for the menu
					if (Time.time - timer > holdTimeForMenu) {
						DisplayMenu ();
						timer = 0.0f;

						// Button was only clicked, perform default action
					} else {
						Act ();
						timer = 0.0f;
					}
				}

				yield return null;
			}
		}

		private void DisplayMenu() {
			if (target) {
				//target.DisplayMenu ();
			} else {
				Debug.Log ("UserCommand component not found!");
			}
		}

		/// <summary>
		/// Performs the correct action based on strict conditions:
		/// If Friendly or neutral objects that have dialog, speak.
		/// If Hostile object that can be attacked, attack, if not but it
		/// has a Speaking component, speak.
		/// If a non-hostile object that can be picked up, pick it up.
		/// If a non-hostile object that can be used, use it.
		/// </summary>
		private void Act() {

			// Can it speak?

			if (target) {
				target.Interact ();
			} else {
				Debug.Log ("UserCommand component not found!");
			}
		}
	}
}
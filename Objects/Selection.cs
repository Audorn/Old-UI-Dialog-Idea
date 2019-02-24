using UnityEngine;
using System.Collections;

/// <summary>
/// Checked by InputManager() on a mouse left click.  Manages selectability of a GameObject in
/// the scene and selects/deselects appropriately.
/// </summary>

public class Selection : MonoBehaviour {
	[SerializeField] private MainCameraController cameraController;
	[SerializeField] private ActorData actor;
	[SerializeField] private PartyMember targetPartyMember;
	[SerializeField] private FeatureData feature;
	[SerializeField] private ItemData item;

	public bool canSelect = true;

	public void Start() {
		// Initialize private variables
		cameraController = FindObjectOfType<MainCameraController> ();
	}

	/// <summary>
	/// Selects the gameObject in an appropriate manner, deselecting the current gameObject in
	/// certain circumstances, reverting to the party leader.
	/// </summary>
	public void Select() {

		// Early out if already selected or if target cannot be selected
		if (transform == cameraController.selection || !canSelect)
			return;

		// ==========
		// What type of object is being selected?

		// Is this a party member?
		actor = GetComponent<ActorData> ();
		targetPartyMember = GetComponent<PartyMember> ();
		Debug.Log (actor);
		Debug.Log (targetPartyMember);
		if (actor && targetPartyMember) {
			PartyMember currentPartyMember = cameraController.selection.GetComponent<PartyMember> ();

			// Only select the targeted party member if the current one is not receiving orders
			if (!currentPartyMember.isReceivingOrders) {
				cameraController.selection = transform;
			} else {
				Debug.Log ("Finalize orders for " + cameraController.selection.gameObject + " first!");
			}
		} else {
			Debug.Log (gameObject + " is not an actor!");

			// Is this an item?
			item = GetComponent<ItemData> ();
			if (item) {
				
				// ---
				// TODO: Figure out what happens when you click an item.
				// ---
				Debug.Log("Item: " + item + " selected");
			} else {
				Debug.Log (gameObject + " is not an item!");

				// Is this a feature?
				feature = GetComponent<FeatureData> ();
				if (feature) {

					// ---
					// TODO: Figure out what happens when you click a feature.
					// ---
					Debug.Log ("Feature: " + feature + " selected");
				} else {
					Debug.Log (gameObject + " is not a feature!");
					Debug.Log ("Nothing selected!  !?Why is a selection on this gameObject!?");
				}
			}
		}
		// ==========
	}


}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WanzerWeave.DialogEditor {


	/// <summary>
	/// A map of nodes that make up the dialog for a single object within the game that is capable of speaking in any way.
	/// </summary>
	[System.Serializable]
	public class DialogMap {

		// unique read-only reference id for each dialog map
		ushort _refId;
		public ushort RefId { 
			get { return _refId; }
			private set { _refId = value; }
		}

		// reference id generator to keep track of nodes within a dialog map
		ushort _nodeRefId = 0;
		public ushort NodeRefId {
			get { return _nodeRefId; }
			private set { _nodeRefId = value; }
		}

		// the speaker this dialog map is assigned to
		public GameObject speakerObject;
		public string speakerDisplayName = "NO NAME"; 
		//public Texture2D portrait;

		// actual lines of dialog
		public List<Node> nodes = new List<Node> ();
		public int startNode;
		public bool displayStartNode = false;



		/// <summary>
		/// Make sure there are no parent or child references to this node.
		/// </summary>
		/// <param name="Node">Node being removed.</param>
		private void ScrubNodeConnections(Node node) {

			for (int i = 0; i < nodes.Count; i++) {
				Node n = nodes [i];

				// this node has parents
				if (n.parents.Count > 0) {
					for (int p = 0; p < n.parents.Count; p++) {

						// this parent is the node being deleted
						if (n.parents [p] == node.RefId) {
							n.parents.RemoveAt (p);
							p--;
						}
					}
				}

				// this node has children
				if (n.children.Count > 0) {
					for (int c = 0; c < n.children.Count; c++) {

						// this child is the node being deleted
						if (n.children [c] == node.RefId) {
							n.children.RemoveAt (c);
							n.prereqs.RemoveAt (c);
							c--;
						}
					}
				}
			}
		}

		/// <summary>
		/// Removes a node from this dialog maps list of nodes.
		/// </summary>
		/// <param name="index">Index of the node to be removed.</param>
		public void RemoveNodeAtIndex(int index) {

			// this is not the only node
			if (nodes.Count > 1)
				ScrubNodeConnections (nodes[index]);

			nodes.RemoveAt (index);
		}

		/// <summary>
		/// Removes a node from this dialog maps list of nodes.
		/// </summary>
		/// <param name="value">Value of the node to be removed.</param>
		public void RemoveNode(Node node) {

			// this is not the only node
			if (nodes.Count > 1)
				ScrubNodeConnections (node);

			nodes.Remove (node);
		}

		/// <summary>
		/// Adds a node to this dialog maps list of nodes.
		/// </summary>
		public void AddNode(Rect newRect) {
			Node newNode = new Node (this);
			newNode.SetRect (newRect);
			nodes.Add (newNode);
		}

		/// <summary>
		/// Gets an unused node reference identifier and prepares the next one.
		/// </summary>
		/// <returns>The node reference identifier.</returns>
		public ushort GetNodeRefId() {
			ushort refId = NodeRefId;
			NodeRefId++;
			return refId;
		}

		/// <summary>
		/// Resets the node reference identifier.
		/// </summary>
		public void ResetNodeRefId() {
			NodeRefId = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WanzerWeave.DialogEditor.DialogMap"/> class.
		/// </summary>
		/// <param name="DD">The dialogDatabase.asset file being edited.</param>
		public DialogMap(DialogDatabase DD) {

			// reset the MapRefId if all speakers have been deleted
			if (DD.maps.Count == 0)
				DD.ResetMapRefId ();

			// assign a SpeakerRefId to this speakers RefId
			this.RefId = DD.GetMapRefId();
		}
	}



}









































/*                                      //  OLD VERSION FOR INVENTORY ITEM!!
[System.Serializable]                   //  Our Representation of an InventoryItem
public class InventoryItem 
{
	public string itemName = "New Item";  //  What the item will be called in the inventory
	public Texture2D itemIcon = null;     //  What the item will look like in the inventory
	public Rigidbody itemObject = null;   //  Optional slot for a PreFab to instantiate when discarding
	public bool isUnique = false;         //  Optional checkbox to indicate that there should only be one of these items per game
	public bool isIndestructible = false; //  Optional checkbox to prevent an item from being destroyed by the player (unimplemented)
	public bool isQuestItem = false;      //  Examples of additional information that could be held in InventoryItem
	public bool isStackable = false;      //  Examples of additional information that could be held in InventoryItem
	public bool destroyOnUse = false;     //  Examples of additional information that could be held in InventoryItem
	public float encumbranceValue = 0;    //  Examples of additional information that could be held in InventoryItem  !!!
}
*/
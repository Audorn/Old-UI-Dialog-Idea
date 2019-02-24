using UnityEngine;
using UnityEngine.Events;
using System.Runtime.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WanzerWeave.DialogEditor {



	/// <summary>
	/// A Node is a line of dialog.  Dialog is structured so that a particular speaker will have all the nodes of all
	/// possible conversations it may partake in, in a complex dendritic structure with prerequisites governing what path
	/// a conversation may take.
	/// </summary>
	[Serializable]
	public class Node {

		// unique read-only reference id for each node
		ushort _refId;
		public ushort RefId {
			get { return _refId; }
			private set { _refId = value; }
		}

		// Dialog Editor control variables
		public Rect rect;											// location in the node map display
		public bool isExpanded = true;			// uses different heights for collapsed and not collapsed
		public ushort tier;										// governs y location when autosorting


		// actual lines of dialog - shortLine is the summary for a PC dialog option
		public string fullLine = "";
		public string shortLine = "";

		// relationship variables
		public int maxParents = 5;						// this is a limitation of the size of the nodes
		public List<ushort> parents = new List<ushort> ();
		public List<ushort> children = new List<ushort> ();
		public bool wantsChild = false;

		// prereqs for each child - jagged list: children[0] == List<Coroutine> () in [0]
		public List<List<Coroutine>> prereqs = new List<List<Coroutine>> ();

		// variables that might be used for each prereq
		public Vector3 teleportPartyLeaderDestination;

		public bool CanConnect(ushort childRId, ushort parentRId) {

			bool status = true;

			if (childRId == parentRId) { 
				Debug.Log ("Cannot parent a node to itself!");
				status = false;
			}

			for (int i = 0; i < children.Count; i++) {

				if (children [i] == childRId) {
					Debug.Log ("Child already in parent's list of children!");
					status = false;
				}
			}

			for (int i = 0; i < parents.Count; i++) {

				if (parents [i] == parentRId) {
					Debug.Log ("Parent already in child's list of parents!");
					status = false;
				}
			}

			return status;
		}

		/// <summary>
		/// Checks to see if the child can be assigned to the parent.  If it can, it assigns it and create a new
		/// list of prereqs for that child and the first prereq in the list.
		/// </summary>
		/// <param name="refId">The reference id of the child.</param>
		public void ConnectToChild(ushort childRefId) {

			if (CanConnect (childRefId, RefId)) {
				children.Add (childRefId);
				prereqs.Add (new List<Coroutine> ());
				prereqs [prereqs.Count - 1].Add (new Coroutine ());
			}
		}

		/// <summary>
		/// Checks to see if the parent can be assigned to the child.  If it can, it assigns it.
		/// </summary>
		/// <param name="refId">The reference id of the parent.</param>
		public void ConnectToParent(ushort parentRefId) {

			if (CanConnect (RefId, parentRefId))
				parents.Add(parentRefId);
		}

		/// <summary>
		/// Collapses the node.
		/// </summary>
		/// <param name="collapsedHeight">Collapsed height of the node from dialog.asset.</param>
		public void CollapseNode(int collapsedHeight) {
			rect.height = collapsedHeight;
			isExpanded = false;
		}

		/// <summary>
		/// Expands the node.
		/// </summary>
		/// <param name="expandedHeight">Expanded height of the node from dialog.asset.</param>
		public void ExpandNode(int expandedHeight) {
			rect.height = expandedHeight;
			isExpanded = true;
		}

		/// <summary>
		/// Sets the rect of this node to a new rect.
		/// </summary>
		/// <param name="newRect">New rect to replace the old one.</param>
		public void SetRect(Rect newRect) {
			rect = newRect;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WanzerWeave.DialogEditor.Node"/> class.
		/// </summary>
		/// <param name="map">The dialog map being edited.</param>
		public Node(DialogMap map) {

			// reset the nodeRefId if all nodes for this dialog map have been deleted
			if (map.nodes.Count == 0)
				map.ResetNodeRefId ();

			// assign a NodeRefId to this nodes RefId
			this.RefId = map.GetNodeRefId();
		}
	}



}
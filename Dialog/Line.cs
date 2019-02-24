using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WanzerWeave.DialogEditor;

namespace WanzerWeave.DialogSystem {


	[System.Serializable]
	public class Line {

		// unique read-only reference id for each node
		ushort _refId;
		public ushort RefId {
			get { return _refId; }
			private set { _refId = value; }
		}

		// actual lines of dialog - shortLine is the summary for a PC dialog option
		public string fullLine = "";
		public string shortLine = "";

		// relationship variables
		public List<ushort> parents = new List<ushort> ();
		public List<ushort> children = new List<ushort> ();

		// prereqs for each child - jagged list: children[0] == List<Coroutine> () in [0]
		public List<List<Coroutine>> prereqs = new List<List<Coroutine>> ();
	

		/// <summary>
		/// Compares the node with this line of dialog.
		/// </summary>
		/// <returns><c>true</c>, if line data matches the node data, <c>false</c> otherwise.</returns>
		/// <param name="node">Node being compared to this line.</param>
		public bool CompareToNode (Node node) {

			// check the node
			if (RefId != node.RefId || fullLine != node.fullLine || shortLine != node.shortLine)
				return false;

			// are there parents or children in the line but not in the node
			if (parents != null && node.parents == null || children != null && node.children == null)
				return false;

			// are there parents or children in the node but not in the line
			if (node.parents != null && parents == null || node.children != null && children == null)
				return false;

			// are there the same number of parents and the same number of children
			if (parents.Count != node.parents.Count || children.Count != node.children.Count)
				return false;

			// compare the parent values
			for (int i = 0; i < parents.Count; i++) {
				if (parents [i] != node.parents [i])
					return false;
			}

			// compare the chilren values
			for (int i = 0; i < children.Count; i++) {
				if (children [i] != node.children [i])
					return false;
			}

			// compare jagged arrays of prerequisites
			for (int i = 0; i < prereqs.Count; i++) {
				for (int j = 0; j < prereqs [i].Count; j++) {
					if (prereqs [i] [j] != node.prereqs [i] [j])
						return false;
				}
			}
			// all checks passed, node is correct
			return true;
		}

		/// <summary>
		/// Copies all the pertinent data from the dialogDatabase.asset dialog map node.
		/// </summary>
		/// <param name="node">Node being copied from.</param>
		public void UpdateLine (Node node) {
			RefId = node.RefId;
			fullLine = node.fullLine;
			shortLine = node.shortLine;

			parents.Clear ();
			children.Clear ();
			prereqs.Clear ();

			for (int i = 0; i < node.parents.Count; i++) {
				ushort newParent = node.parents [i];
				parents.Add (newParent);
			}

			for (int i = 0; i < node.children.Count; i++) {
				ushort newChild = node.children [i];
				children.Add (newChild);
			}

			if (node.prereqs != null) {
				for (int i = 0; i < node.prereqs.Count; i++) {
					prereqs.Add (new List<Coroutine> ());
					for (int j = 0; j < node.prereqs [i].Count; j++) {
						Coroutine newCoroutine = node.prereqs [i] [j];
						prereqs [i].Add (newCoroutine);
					}
				}
			}
		}

	}

}

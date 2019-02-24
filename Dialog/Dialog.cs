using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WanzerWeave.DialogEditor;

namespace WanzerWeave.DialogSystem {


	[System.Serializable]
	public class Dialog : MonoBehaviour {

		[SerializeField] ushort _refId;
		public ushort RefId {
			get { return _refId; }
			private set { _refId = value; }
		}
			
		public string speakerDisplayName;

		public List<Line> lines = new List<Line> ();
		public int startingLine;
		public bool displayStartingLine = false;


		/// <summary>
		/// Compare the dialog map from dialogDatabase.asset to the HasDialog and see if it is updated.
		/// </summary>
		/// <returns><c>true</c>, if the data is the same, <c>false</c> otherwise.</returns>
		/// <param name="map">The dialog map being compared to this dialog component.</param>
		public bool CompareToDialogMap (DialogMap map) {

			// check speaker data
			if (RefId != map.RefId || speakerDisplayName != map.speakerDisplayName)
				return false;

			// does lines exist in this dialog component but not nodes in the dialog map
			if (lines != null && map.nodes == null)
				return false;

			// does nodes exist in the dialog map but not lines in this component
			if (map.nodes != null && lines == null)
				return false;

			// does the starting line and starting node match up
			if (map.startNode != startingLine)
				return false;

			// does the displayStartingNode toggle sync up
			if (map.displayStartNode != displayStartingLine)
				return false;
			
			// are there the same number of nodes and lines
			if (lines.Count != map.nodes.Count)
				return false;
			else {
				
				// check the nodes
				for (int i = 0; i < lines.Count; i++) {
					Line currentLine = lines [i];
					Node currentNode = map.nodes [i];

					if (!currentLine.CompareToNode (currentNode))
						return false;
				}
			}

			// all checks passed speaker is correct
			return true;
		}

		/// <summary>
		/// Copies all the pertinent data from the dialog.asset speaker and its nodes.
		/// </summary>
		/// <param name="newRefId">New reference identifier.</param>
		public void UpdateDialog (DialogMap map) {
			RefId = map.RefId;
			speakerDisplayName = map.speakerDisplayName;
			startingLine = map.startNode;
			displayStartingLine = map.displayStartNode;

			if (lines.Count > 0)
				lines.Clear ();

			// convert the nodes to lines of dialog for the game UI
			foreach (Node node in map.nodes) {

				Debug.Log ("test");
				Line newLine = new Line ();
				newLine.UpdateLine (node);
				lines.Add (newLine);
			}

			Debug.Log (lines.Count);
		}
	}



}
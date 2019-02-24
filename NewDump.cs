using UnityEngine;
using System.Collections;

public class NewDump : MonoBehaviour {

/*

	// background layout

	private string speakerName = "";											// used to set/change a speaker's name
	private Vector2 speakerListDisplay = new Vector2 (298, 600); 		// scrolling window
	private Vector2 listScrollPosition;															// scrollbar x,y
	private int selectedSpeaker;  												// speaker being edited
	private Rect speakerSearchPanel = new Rect (2, 620, 298, 140);
	//private Vector2 speakerSearchDisplay = new Vector2 (290, 110);	// scrolling window
	private Vector2 searchScrollPosition;														// scrollbar x,y
	private string searchSpeaker = "";										// speaker being searched for
	private int searchRowLength = 7;											// number of results in each row
	private Rect speakerDetailPanel = new Rect (2, 764, 298, 185);

	private bool showMap = true;													// shows list if false

	private Vector2 centerDisplay = new Vector2 (700, 950); 				// scrolling window
	private Vector2 centerScrollPosition;														// scrollbar x,y
	private Vector2 mapDimensions = new Vector2 (5000, 10000);			// interior width
	private int mapGridSpacing = 25;											// grid spacing within total map area


	//private Rect nodeDetailPanel = new Rect (1010, 8, 300, 120);
	private Vector2 nodePrerequisiteDisplay = new Vector2 (300, 120);	// scrolling window
	private Vector2 nodePrerequisiteScrollPosition;									// scrollbar x,y
	private Vector2 nodeEffectDisplay = new Vector2 (300, 120);			// scrolling windows
	private Vector2 nodeEffectScrollPosition;												// scrollbar x,y
	private Rect displayControlsPanel = new Rect (1010, 900, 300, 120);

	// node layout and control
	private int nodeWidth = 200;
	private int expandedNodeHeight = 100;
	//private int collapsedNodeHeight = 30;
	//private int nodeXPadding = 10;
	//private int nodeYPadding = 10;
	private bool moveNode = false;												// move current node when scrolling?
	private int selectedNode;															// current node

	// general variables and styles
	private int buttonWidth = 20;													// width of common minibuttons
	private int indexWidth = 30;													// width of index displays
	private GUIStyle panelStyle = new GUIStyle (EditorStyles.helpBox);					// panels
	private GUIStyle labelStyle = new GUIStyle(EditorStyles.label);							// lables
	private GUIStyle miniButtonStyle = new GUIStyle (EditorStyles.miniButton);	// minibuttons
	private Color trueColor;

	//private float mapZoom = 1.0f;													// 
	private int mapScrollPadding = 20;										// thickness of scroll-sensitive area
	private int mapScrollSpeed = 100;											// speed of scrolling because of padding
	private int scrollbarThickness = 15;									// used to calculate LineNode clamping

	private int parentSearchingForChild = -1;							// parent index or -1 for none
	//private int childSearchingToSwitch = -1;							// child index or -1 for none

	public List<Texture2D> tacts = new List<Texture2D> ();
	public List<Texture2D> tactsManip = new List<Texture2D> ();





	/// <summary>
	/// Controls the functionality and display of the Dialog Editor.
	/// </summary>
	void  OnGUI () {

		// dialog.asset is loaded!
		if (dialog != null) {

			GUILayout.BeginHorizontal (); // contain the displays horizontally
			// ============================================================ Left Column



			// speaker(s) present, show them and all the ways to interact with them
			if (dialog.speakers.Count > 0) {



				GUILayout.BeginArea (speakerSearchPanel, panelStyle);
				ShowSearchPanel ();
				GUILayout.EndArea ();

				GUILayout.BeginArea (speakerDetailPanel, panelStyle);
				ShowSpeakerDetailPanel ();
				GUILayout.EndArea ();
			}

			// ============================================================ End Left Column

			// speaker(s) present 
			if (dialog.speakers.Count > 0) {
				// clamp the currently selected speaker to a valid number
				selectedSpeaker = Mathf.Clamp (selectedSpeaker, 0, dialog.speakers.Count - 1);
				Speaker speaker = dialog.speakers [selectedSpeaker];

				// this speaker has nodes
				if (speaker.nodes.Count > 0) {
					// clamp the selected node to a valid number
					selectedNode = Mathf.Clamp (selectedNode, 0, speaker.nodes.Count - 1);
					Node node = speaker.nodes[selectedNode];

					// ============================================================ Center Display
					GUILayout.BeginArea (centerColumn);
					centerScrollPosition = EditorGUILayout.BeginScrollView (centerScrollPosition, 
						"textfield", GUILayout.Width (centerDisplay.x), GUILayout.MaxHeight (centerDisplay.y));

					// dark background for the display
					GUI.color = Color.black;
					GUILayout.Label ("", panelStyle, GUILayout.Width (mapDimensions.x), 
						GUILayout.Height (mapDimensions.y));
					GUI.color = trueColor;

					// show the nodes on a map with free movement
					if (showMap)
						ShowNodeMap (speaker);

					// show the nodes in a list
					else
						ShowNodeList (speaker);

					GUILayout.EndScrollView ();
					GUILayout.EndArea ();
					// ============================================================ End Center Display


					// ============================================================ Right Column
					GUILayout.BeginArea (rightColumnRect);

					// title label
					labelStyle.alignment = TextAnchor.MiddleCenter;
					labelStyle.fontStyle = FontStyle.Bold;
					GUILayout.Label ("Line Selected For Editing", labelStyle);
					labelStyle = new GUIStyle (EditorStyles.label);

					ShowNodeDetailPanel (node);

					nodePrerequisiteScrollPosition = EditorGUILayout.BeginScrollView 
						(nodePrerequisiteScrollPosition, "helpbox", 
							GUILayout.Width (nodePrerequisiteDisplay.x), 
							GUILayout.Height (nodePrerequisiteDisplay.y));

					ShowPrerequisites ();

					GUILayout.EndScrollView ();

					nodeEffectScrollPosition = EditorGUILayout.BeginScrollView 
						(nodeEffectScrollPosition, "helpbox", 
							GUILayout.Width (nodeEffectDisplay.x), 
							GUILayout.Height (nodeEffectDisplay.y));


					ShowEffects ();

					GUILayout.EndScrollView ();



					GUILayout.BeginArea (displayControlsPanel);
					GUILayout.BeginHorizontal ("helpbox");

					// PLACE CONTROLS HERE

					GUILayout.EndHorizontal ();
					GUILayout.EndArea ();



					GUILayout.EndArea ();
					// ============================================================ End Right Column
				}

			}
			GUILayout.EndHorizontal ();

		}
	}






	/// <summary>
	/// Show the details of the currently selected speaker:
	/// ---------------------------------------
	/// | Index#: Name                         |
	/// |																			 |
	/// |	Lines/Rects: #/#       AddNodeButton |
	/// |																			 |
	/// |																			 |
	/// |																			 |
	/// |																			 |
	/// |																			 |
	/// ---------------------------------------
	/// </summary>
	private void ShowSpeakerDetailPanel() {
		if (dialog.speakers.Count > 0) {
			Speaker speaker = dialog.speakers [selectedSpeaker];
			int nameWidth = 290;
			string displayName = selectedSpeaker.ToString () + ": "	+ speaker.speakerName;
			int nodesRectsWidth = 120;
			string nodes = "Nodes: " + speaker.nodes.Count;
			int distanceFromX0 = 2;
			int distanceFromY0 = 795;

			// speaker index followed by name
			labelStyle.fontStyle = FontStyle.Bold;
			GUILayout.Label (displayName, labelStyle, GUILayout.Width (nameWidth));
			labelStyle = new GUIStyle (EditorStyles.label);

			// Nodes: #                             AddNodeButton
			GUILayout.BeginHorizontal ();
			GUILayout.Label (nodes, EditorStyles.miniLabel, GUILayout.Width (nodesRectsWidth));

			// create the rect for the node and place it on the grid next to the AddNodeButton
			if (GUILayout.Button ("Add Node", EditorStyles.miniButton, GUILayout.ExpandWidth (false))) {
				float x = Mathf.Clamp ((distanceFromX0 + centerScrollPosition.x)
					- ((distanceFromX0 + centerScrollPosition.x) % mapGridSpacing), 0, mapDimensions.x);
				float y = Mathf.Clamp ((distanceFromY0 + centerScrollPosition.y)
					- ((distanceFromY0 + centerScrollPosition.y) % mapGridSpacing), 0, mapDimensions.y);

				// make the node to the speaker and add its Rect to the list in dialog.asset
				speaker.AddNode (new Rect (x, y, nodeWidth, expandedNodeHeight));

				// stop this node from being moved on scrolling
				moveNode = false;
			}
			GUILayout.EndHorizontal ();

			// temporary placement of showMap button
			if (GUILayout.Button (showMap ? "Show List" : "Show Map", EditorStyles.miniButton))
				showMap = !showMap;
		}
	}


	private void ShowNodeMap(Speaker speaker) {

		// right-clicking stops the selected node from moving on scrolling
		if (Event.current.button == 1 && Event.current.type == EventType.mouseUp)
			moveNode = false;

		BeginWindows ();

		// make the nodes and their connections visible one by one, connections first
		for (int i = 0; i < speaker.nodes.Count; i++) {

			Node node = speaker.nodes [i];
			Rect rect = speaker.nodes [i].rect;
			float startX = rect.x + (rect.width / 2) + 4; // what is this math???
			float startY = rect.y + rect.height;

			// this node has children
			if (node.children.Count > 0) {

				// cycle through the children
				for (int childIndex = 0; childIndex < node.children.Count; childIndex++) {

					Node child = speaker.nodes [node.children[childIndex]];
					Rect childRect = child.rect;
					int xSpacing =  5;


					// cycle through the child's parents to find out which one we are
					for (int parentIndex = 0; parentIndex < child.parents.Count; parentIndex++) {

						float endX = childRect.x + (buttonWidth / 2 + 2) + ((xSpacing + buttonWidth) * parentIndex);
						float endY = childRect.y;

						// match found
						if (child.parents [parentIndex] == i) {

							// prepare the bezier curve from this node to this child, anchored at
							// the proper destination button on the top of the child node
							Vector3 startPos = new Vector3 (startX, startY, 0);
							Vector3 endPos = new Vector3 (endX, endY, 0);
							Vector3 startTan = startPos + Vector3.up * 50;
							Vector3 endtan = endPos + Vector3.down * 50;
							Color shadowColor = new Color (0, 0, 0, 0.18f);

							// draw the shadow, then the curve
							Handles.DrawBezier (startPos, endPos, startTan, endtan, shadowColor, null, (i + 1) * 5);
							Handles.DrawBezier (startPos, endPos, startTan, endtan, Color.black, null, 1);
						}
					}
				}
			}

			// this is the currently selected node, prepare its Rect
			if (i == selectedNode) {

				// Place on grid
				rect.x -= rect.x % mapGridSpacing;
				rect.y -= rect.y % mapGridSpacing;

				// selected node moves with the screen
				if (moveNode) {

					int clampXMax = (int)(centerDisplay.x + centerScrollPosition.x - rect.width - scrollbarThickness);
					int clampYMax = (int)(centerDisplay.y + centerScrollPosition.y - rect.height - scrollbarThickness);

					// Clamp to screen
					rect.x = Mathf.Clamp (rect.x, 0, clampXMax); // might have problems with min
					rect.y = Mathf.Clamp (rect.y, 0, clampYMax);

					// Pan the screen if the line node is within the padding
					if (rect.x < centerScrollPosition.x + mapScrollPadding)
						centerScrollPosition.x -= mapScrollSpeed;
					if (rect.x + rect.width > centerScrollPosition.x + centerDisplay.x - mapScrollPadding)
						centerScrollPosition.x += mapScrollSpeed;
					if (rect.y < centerScrollPosition.y + mapScrollPadding)
						centerScrollPosition.y -= mapScrollSpeed;
					if (rect.y + rect.height > centerScrollPosition.y + centerDisplay.y - mapScrollPadding)
						centerScrollPosition.y += mapScrollSpeed;

				}

				dialog.speakers [selectedSpeaker].nodes [i].rect = rect;
			}

			// draw the rect
			dialog.speakers [selectedSpeaker].nodes [i].rect = GUILayout.Window (i,
				dialog.speakers[selectedSpeaker].nodes [i].rect, DrawNode, "", panelStyle);

		}
		EndWindows ();
	}

	/// <summary>
	/// Draw a node; this will be called after lines are drawn, so the nodes will
	/// appear to be on top of them.
	/// </summary>
	/// <param name="id">Identifier for the rect?</param>
	void DrawNode (int id) {

		List<Node> nodes = dialog.speakers [selectedSpeaker].nodes;
		Node node = nodes[id];
		//Rect rect = node.rect;
		string lineToDisplay = node.fullLine;

		// on a left-click, this node becomes selected and will move when scrolling
		if (Event.current.button == 0 && Event.current.type == EventType.mouseUp) {
			selectedNode = id;
			moveNode = true;
		}

		GUILayout.BeginHorizontal ();

		// ANCHOR BUTTONS FOR LINES - STACK THEM LEFT TO RIGHT
		// LAST IS THE ^ BUTTON TO CONNECT TO A PARENT

		int widthToButtonAtX0 = nodeWidth - buttonWidth - 15;

		// this node has parents
		if (node.parents.Count > 0) {

			// find each parent
			for (int parentIndex = 0; parentIndex < node.parents.Count; parentIndex++) {
				Debug.Log (node.parents.Count + ":  " + node.parents[parentIndex]);
				Debug.Log (nodes[node.parents[parentIndex]]);
				Node parent = nodes[node.parents[parentIndex]];

				// find this child in the parent's list of children
				for (int displayOrder = 0; displayOrder < parent.children.Count; displayOrder++) {

					// found the child in the parent's list
					if (parent.children [displayOrder] == id) {

						// button showing the index of this child within the parent's list
						if (GUILayout.Button (displayOrder.ToString (), EditorStyles.miniButton, 
							GUILayout.Width (buttonWidth))) {

							// switch with the next clicked one

						}
						break;
					}
				}
			}
		}

		GUILayout.Space (widthToButtonAtX0 - (node.parents.Count * (buttonWidth + 5)));

		// child destination button, to be used after 'v' button from parent
		if (GUILayout.Button ("^", EditorStyles.miniButton, GUILayout.Width(buttonWidth))) {

			// corresponding 'v' button has NOT been clicked
			if (parentSearchingForChild < 0)
				Debug.Log ("No parent selected; click the 'v' button on the parent first.");

			// parent already linked to child
			else if (node.parents.Contains (parentSearchingForChild))
				Debug.Log ("Failed assigning node to parent; this node is already assigned to that parent.");

			// parent IS this node
			else if (parentSearchingForChild == id)
				Debug.Log ("Cannot assign a node to itself.");
			else {
				// checks passed
				Node parentNode = nodes [parentSearchingForChild];

				// assign the parent that was trying to make this a child to the child's list 
				// of parents (parentSearchingForChild is the index of that parent)
				// assign an empty UnityEvent to access functions to be executed on display
				node.parents.Add (parentSearchingForChild);

				// assign the child to the parent; assign an empty UnityEvent to access logical 
				// checks to make sure the child can be selected or displayed.
				parentNode.children.Add (id);
				parentNode.childPrereqs.Add(new List<UnityEvent> ()); // IS THIS HOW TO UNITYEVENT?


				// reset this variable
				parentSearchingForChild = -1;
			}

		}


		GUILayout.EndHorizontal ();


		GUIStyle style = new GUIStyle ();
		style.fontStyle = FontStyle.Bold;

		GUILayout.BeginHorizontal ();
		GUILayout.Label (dialog.speakers[selectedSpeaker].speakerName + ":" + id.ToString(), 
			style, GUILayout.Width(148));


		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();

		// =============
		// MAKE THESE TWO IMAGES OVERLAY EACHOTHER WITH ALPHA TRANSPARENCY!!
		//GUIStyle style = new GUIStyle ();
		//style.imagePosition = ImagePosition.ImageOnly;
		//if (tacts[0] != null)
		//	GUILayout.Label (tacts[0], style);
		//if (dialog.speakers [selectedSpeaker].portrait != null)
		//	GUILayout.Label (dialog.speakers[selectedSpeaker].portrait, style);

		// TEMPORARILY TO BLOCK OUT THE SPACE
		GUILayout.Label ("", EditorStyles.textField, GUILayout.Width(40), GUILayout.Height(40));
		// =============

		// display the shortLine if it exists, otherwise display the fullLine
		if (node.shortLine != "")
			lineToDisplay = node.shortLine;
		GUILayout.Label (lineToDisplay, EditorStyles.miniLabel);
		GUILayout.EndHorizontal ();

		// center and prepare the style for the 'v' button
		GUILayout.BeginHorizontal ();
		GUILayout.Space ((nodeWidth / 2) - (buttonWidth / 2));
		if (parentSearchingForChild == id) {
			miniButtonStyle.normal.textColor = Color.magenta;
			miniButtonStyle.fontStyle = FontStyle.Bold;
		}

		// button to select a parent that wants to connect to a child
		if (GUILayout.Button ("v", miniButtonStyle,	GUILayout.Width (buttonWidth))) {
			if (parentSearchingForChild == id)
				parentSearchingForChild = -1;
			else 
				parentSearchingForChild = id;
		}
		miniButtonStyle = new GUIStyle (EditorStyles.miniButton);

		// button to delete this node
		if (GUILayout.Button ("X", miniButtonStyle, GUILayout.Width(buttonWidth))) {

			// this node has parents
			if (node.parents.Count > 0) {
				for (int i = 0; i < node.parents.Count; i++) {
					Node parent = nodes[node.parents[i]];

					// go through the parent's children list
					for (int j = 0; j < parent.children.Count; j++) {

						// this child and its corresponding prerequisites need to be removed
						if (parent.children [j] == id) {

							// remove the child from their parent's children and prerequisite lists
							parent.children.RemoveAt(j);
							parent.childPrereqs.RemoveAt(j);

							// adjust selectedNode
							if (selectedNode > 0)
								selectedNode--;
							break;

						}
					}
				}

				// now that all the children and prerequisite references have been removed
				// the child refrences higher than id will need to be decremented by 1 so 
				// that they will be prepared for the removal of the node
				for (int i = 0; i < nodes.Count; i++) {

					// go through the children list
					for (int j = 0; j < nodes[i].children.Count; j++) {

						// this references a child that needs its index to be decremented
						if (nodes[i].children[j] > id) 
							nodes[i].children[j]--;
					}
				}
			}

			// this node has children
			if (node.children.Count > 0) {
				for (int i = 0; i < node.children.Count; i++) {
					Node child = nodes[node.children[i]];

					// go through the child's parent list
					for (int j = 0; j < child.parents.Count; j++) {

						// remove this node from the parents list of their child
						if (child.parents [j] == id) {
							child.parents.RemoveAt (j);
							break;
						}
					}
				}

				// now that all the parent references have been removed, the parent refrences
				// higher than the id will need to be decremented by 1 so that htey will be
				// prepared for the removal of the node
				for (int i = 0; i < nodes.Count; i++) {

					// go through the parent list
					for (int j = 0; j < nodes[i].parents.Count; j++) {

						// this is a parent reference that will need to be decremented
						if (nodes [i].parents [j] > id) 
							nodes [i].parents [j]--;
					}
				}
			}

			// remove this node
			nodes.RemoveAt (id);
		}

		GUILayout.EndHorizontal ();

		// make this node draggable
		GUI.DragWindow ();
	}

	private void ShowNodeList (Speaker speaker) {

	}

	private void ShowNodeDetailPanel (Node node) {

		int indexLabelWidth = 60;

		GUILayout.BeginHorizontal ();

		// index #
		GUILayout.Label ("Index: " + selectedNode.ToString (), EditorStyles.miniLabel,
			GUILayout.Width (indexLabelWidth));

		// show number of parents and children
		string labelDisplay = "Parents/Children: ";
		labelDisplay += node.parents != null ? node.parents.Count.ToString () : "0";
		labelDisplay += "/";
		labelDisplay += node.children != null ? node.children.Count.ToString () : "0";

		GUILayout.Label (labelDisplay,	labelStyle);
		GUILayout.EndHorizontal ();


	}

	private void ShowPrerequisites() {
		Node node = dialog.speakers [selectedSpeaker].nodes [selectedNode];
		List<List<UnityEvent>> listOfPrereqs = node.childPrereqs;

		int prereqId = 0;

		// make a box for each UnityEvent and stack them
		foreach (List<UnityEvent> prereqs in listOfPrereqs) {

			GUILayout.BeginVertical ("box");
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("prereq: " + prereqId);
			prereqId++;
			GUILayout.EndHorizontal ();

			foreach (UnityEvent prereq in prereqs) {

				// allow modification of prereqs

			}

			GUILayout.EndVertical ();
		}

	}


	private void ShowEffects() {
		Node node = dialog.speakers [selectedSpeaker].nodes [selectedNode];
		List<UnityEvent> effects = node.effects;

		int effectId = 0;

		// make a box for each UnityEvent and stack them
		foreach (UnityEvent effect in effects) {

			GUILayout.BeginVertical ("box");
			GUILayout.BeginHorizontal ();

			GUILayout.Label ("effect: " + effectId);

			// allow modification of effects

			effectId++;

			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
		}
	}
	*/

}

using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using WanzerWeave;
using WanzerWeave.DialogSystem;

/// TODO: 
/// 
/// 
/// NOTE: HAVING MULTIPLE CHILDREN AND DELETING THE LOWERMOST ONE CRASHES THE PROGRAM
/// WITH AN INDEX EXCEPTION ISSUE.  RETRACT THE INDEX INFO BEING ACCESSED BY THE OTHER
/// CHILDREN WHEN ONE IS DELETED.
/// 
/// Implement zooming of the line node map
/// 	Implement a small indicator, reset, and map node size panel
/// 
/// Implement link to cancel if a destination node is not clicked
/// 	link needs to add child to parent and parent to child
/// 	Create a list of prerequisites or an event containing delegates 
/// 	that will share indexes with parents
/// 		these will determine what is required for this node to be available
/// 		through links from different parents.
/// 	Create a list of effects or an event containing delegates
///			these will be accessed and executed through the link when the child 
/// 		is selected by the dialog	system.
/// Create a scrollable area for parents and stack them like speakers in it
/// 	Display needs to include parent line and a series of options to set prereqs
/// 	highlight the links and connections when working on prereqs in this panel
/// Create the area that contains data about the line-node
/// 	Who says it
/// 		check box for party leader
/// 			allows a tact to be selected from a dropdown menu
/// 		text field only available if party leader is false
/// 	portraitLeft texture
/// 		toggle to select manually
/// 			automatically assigns party leader portrait if shortLine has a value
/// 			unless toggled to select manually
/// 	portraitright texture
/// 		toggle to select manually
/// 			automatically assigns this speaker's portrait unless toggled to select
/// 			manually
/// 	fullLine
/// 	shortLine
/// 		If shortLine is != "" then this is a dialog option for the player.
/// Create tact images and place them in the Resources folder
/// Create a few portrait images and place them in the resources folder
/// Implement the tact images and portrait images with alpha transparency
/// 
/// Create a file to replace the inspection panel for Dialog.asset
/// 



namespace WanzerWeave.DialogEditor {



	/// <summary>
	/// Editing tool for creating and manipulating dialog visually.
	/// </summary>
	public class DialogEditor : EditorWindow {

		// asset file
		public DialogDatabase dialogDatabase;

		// layout variables
		Rect leftColumnRect = new Rect (3, 3, 300, 950);
		Rect centerColumnRect = new Rect (305, 3, 900, 950);
		Rect rightColumnRect = new Rect (1210, 3, 300, 950);
		Vector2 dialogMapStackDisplay = new Vector2 (290, 390);
		Vector2 dialogMapStackScrollPosition;
		Vector2 dialogMapSearchDisplay = new Vector2 (290, 120);
		Vector2 dialogMapSearchScrollPosition;
		Vector2 nodeBackgroundDisplay = new Vector2 (900, 900);
		int scrollThickness = 16;
		Vector2 nodeBackgroundScrollPosition;
		Vector2 nodeBackgroundDimensions = new Vector2 (5000, 10000);
		Vector2 childPrereqsStackDisplay = new Vector2 (300, 300);
		Vector2 childPrereqsStackScrollPosition;


		// defaults
		Color defaultGUIColor;

		// input fields
		string speakerDisplayName = "";

		// selection & search control
		int selectedMap;
		string searchedMap = "";

		// center display selection and control
		enum NodeDisplayMethod { Map, List };
		NodeDisplayMethod nodeDisplayMethod = NodeDisplayMethod.Map;
		int mapGridSpacing = 20;

		// node selection and control
		int nodeWidth = 189;
		int nodeExpandedHeight = 120;
		int nodeCollapsedHeight = 40;
		int selectedNode;
		Rect childOrderButtonsPanel = new Rect (0, 3, 102, 20);


		/// <summary>
		/// Called every frame the editor is focused.
		/// </summary>
		void OnGUI() {
			
			// no dialog.asset loaded
			if (dialogDatabase == null) {
				if (GUILayout.Button ("New", EditorStyles.miniButton))
					CreateNewDialogDatabase ();
				if (GUILayout.Button ("Open", EditorStyles.miniButton))
					OpenDialogDatabase ();
			}

			// dialog.asset loaded
			if (dialogDatabase != null) {

				// no speakers present
				if (dialogDatabase.maps.Count == 0)
					ShowAddDialogMapPanel ();

				// dialog maps are present
				if (dialogDatabase.maps.Count > 0) {
					selectedMap = Mathf.Clamp (selectedMap, 0, dialogDatabase.maps.Count - 1);
					DialogMap map = dialogDatabase.maps [selectedMap];

					// column layout
					GUILayout.BeginHorizontal ();
					GUILayout.BeginArea (leftColumnRect);		// ============================================= LEFT COLUMN
					ShowAddDialogMapPanel ();
					ShowDialogMapStack ();
					ShowSearchPanel ();
					ShowDialogMapDetailPanel (map);
					GUILayout.EndArea ();										// ============================================= END LEFT COLUMN

					GUILayout.BeginArea (centerColumnRect);	// ============================================= CENTER COLUMN
					ShowNodeDisplay ();
					GUILayout.EndArea ();										// ============================================= END CENTER COLUMN

					if (map.nodes.Count > 0) {
						selectedNode = Mathf.Clamp (selectedNode, 0, map.nodes.Count - 1);
						Node node = map.nodes [selectedNode];

						GUILayout.BeginArea (rightColumnRect);	// ============================================= RIGHT COLUMN
						ShowBasicNodeDetails (node);
						ShowChildPrereqsPanel (node);

						GUILayout.EndArea ();										// ============================================= END RIGHT COLUMN
					}

					GUILayout.EndHorizontal ();
				}
			}

			EditorUtility.SetDirty (dialogDatabase);
		}

		/// <summary>
		/// Shows the scrolling prerequisites panel, each child will have a list of prerequisites that will translate into
		/// a list of coroutines to run in the dialog manager.
		/// </summary>
		/// <param name="node">Node being described.</param>
		void ShowChildPrereqsPanel(Node node) {

			GUIStyle labelStyle = new GUIStyle (EditorStyles.label);

			GUILayout.BeginVertical ("helpbox");
			// scrolling window
			childPrereqsStackScrollPosition = GUILayout.BeginScrollView (childPrereqsStackScrollPosition, 
				GUILayout.Width (childPrereqsStackDisplay.x), GUILayout.Height(childPrereqsStackDisplay.y));

			// this node has prereqs for its children
			if (node.prereqs.Count > 0) {

				// go through the list of coroutines for each child
				for (int i = 0; i < node.prereqs.Count; i++) {

					// keep all of the prerequisites for a single child in a box
					GUILayout.BeginVertical ("box");

			

					GUILayout.EndVertical ();


					DialogMap map = dialogDatabase.maps [i];
					if (selectedMap == i)
						labelStyle.fontStyle = FontStyle.Bold;
					else if (searchedMap != "" && map.speakerDisplayName.ToUpper ().Contains (searchedMap.ToUpper ()))
						labelStyle.normal.textColor = Color.blue;

					ShowDialogMap (i, labelStyle);

					labelStyle = new GUIStyle (EditorStyles.label);	// reset the label style for the next loop
				}
			} 

			// this node has no children
			else
				GUILayout.Label ("No children!");
			
			GUILayout.EndVertical ();
			GUILayout.EndArea ();


		}

		/// <summary>
		/// Shows the basic information about the node - speaker name, index : refId, fullLine and shortLine contents,
		/// the Set Start Node button and toggle for controlling whether the start node fullLine is displayed, and the
		/// number of parents and/or children and their indexes.
		/// </summary>
		/// <param name="node">Node being described.</param>
		void ShowBasicNodeDetails(Node node) {
			DialogMap map = dialogDatabase.maps [selectedMap];

			int speakerDisplayNameWidth = 230;
			string idText = selectedNode.ToString () + ": <color=red>" + node.RefId.ToString () + "</color>";
			int idWidth = 60;
			int parentsWidth = 70;
			int childrenWidth = 70;
			Vector2 fullLineDimensions = new Vector2 (292, 100);
			Vector2 shortLineDimensions = new Vector2 (292, 20);
			GUIStyle idStyle = new GUIStyle (EditorStyles.label);
			idStyle.fontStyle = FontStyle.Bold;
			idStyle.alignment = TextAnchor.MiddleRight;
			idStyle.richText = true;
			GUIStyle nameTextStyle = new GUIStyle (EditorStyles.label);
			nameTextStyle.fontStyle = FontStyle.Bold;
			GUIStyle boldMiniStyle = new GUIStyle (EditorStyles.miniLabel);
			boldMiniStyle.fontStyle = FontStyle.Bold;
			GUIStyle relationshipStyle = new GUIStyle (EditorStyles.miniLabel);
			relationshipStyle.richText = true;
			GUIStyle lineStyle = new GUIStyle (EditorStyles.textArea);
			lineStyle.fontSize = 10;
			GUIStyle toggleStyle = new GUIStyle (EditorStyles.toggle);
			toggleStyle.fontSize = 10;
			toggleStyle.fontStyle = FontStyle.Bold;

			GUILayout.BeginVertical ("helpbox");
			GUILayout.BeginHorizontal ();
			GUILayout.Label (map.speakerDisplayName, nameTextStyle, GUILayout.Width (speakerDisplayNameWidth));
			GUILayout.Label (idText, idStyle, GUILayout.Width(idWidth));
			GUILayout.EndHorizontal ();

			GUILayout.Space (10);
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Full Line:", boldMiniStyle);
			if (map.startNode > map.nodes.Count - 1)
				toggleStyle.normal.textColor = Color.red;
			map.displayStartNode = GUILayout.Toggle (map.displayStartNode, 
				"Display Start Node: " + map.startNode.ToString (), toggleStyle);
			toggleStyle.normal.textColor = Color.black;
			if (GUILayout.Button ("Set Start Node", EditorStyles.miniButton, GUILayout.ExpandWidth (false)))
				map.startNode = FindNodeIndexFromRefId (node.RefId);

			GUILayout.EndHorizontal ();
			node.fullLine = GUILayout.TextArea (node.fullLine, lineStyle,	
				GUILayout.Width(fullLineDimensions.x), GUILayout.Height(fullLineDimensions.y));
			GUILayout.Label ("Short Line:", boldMiniStyle);
			node.shortLine = GUILayout.TextField (node.shortLine, 100,
				GUILayout.Width (shortLineDimensions.x), GUILayout.Height (shortLineDimensions.y));

			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("<b>Parents:</b> " + node.parents.Count, relationshipStyle, GUILayout.Width(parentsWidth));
			GUILayout.Space (20);
			if (node.parents.Count > 0) {

				string displayText = "Indexes: ";
				for (int i = 0; i < node.parents.Count; i++) {
					ushort refId = node.parents [i];
					displayText += FindNodeIndexFromRefId (refId).ToString ();
					if (i != node.parents.Count - 1)
						displayText += ", ";
				}
				GUILayout.Label (displayText, relationshipStyle);
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("<b>Children:</b> " + node.children.Count, relationshipStyle, GUILayout.Width(childrenWidth));
			GUILayout.Space (20);
			if (node.children.Count > 0) {

				string displayText = "Indexes: ";
				for (int i = 0; i < node.children.Count; i++) {
					ushort refId = node.children [i];
					displayText += FindNodeIndexFromRefId (refId).ToString ();
					if (i != node.children.Count - 1)
						displayText += ", ";
				}
				GUILayout.Label (displayText, relationshipStyle);
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();

		}

		/// <summary>
		/// Makes sure only one node can be searching for a child at a time.
		/// </summary>
		/// <param name="node">Node.</param>
		void LookForChild(Node node) {

			// see if this node is already searching for a child
			if (node.wantsChild) {
				node.wantsChild = false;
				return;
			}
				

			DialogMap map = dialogDatabase.maps [selectedMap];

			for (int i = 0; i < map.nodes.Count; i++) {
				Node n = map.nodes [i];

				// clear any other nodes searching for children
				if (n.wantsChild)
					n.wantsChild = false;
			}

			// tag this node as searching for child
			node.wantsChild = true;
		}

		/// <summary>
		/// Draws the node button that connects this node to a child, the collapse/expand button, and the delete button.
		/// </summary>
		/// <param name="node">Node being drawn.</param>
		/// <param name="id">Index of the node being drawn from the dialog maps list of nodes.</param>
		void DrawNodeBottomControls(Node node, int id) {
			DialogMap map = dialogDatabase.maps [selectedMap];

			int button = 20;
			int middleX = (nodeWidth / 2) - (button / 2) + 2;
			int toRight = middleX - (button * 3 - 13);
			string expandCollapseDisplay = node.isExpanded ? "-" : "=";
			GUIStyle downArrow = new GUIStyle (EditorStyles.miniButton);
			downArrow.normal.textColor = node.wantsChild ? Color.magenta : Color.black;
			downArrow.fontStyle = node.wantsChild ? FontStyle.Bold : FontStyle.Normal;

			GUILayout.BeginHorizontal();
			GUILayout.Space (middleX);
			if (GUILayout.Button ("v", downArrow, GUILayout.Width (button)))
				LookForChild (node);

			GUILayout.Space (toRight);

			// expand or collapse node accordingly
			if (GUILayout.Button (expandCollapseDisplay, EditorStyles.miniButton, GUILayout.Width (button))) {
				if (node.isExpanded)
					node.CollapseNode (nodeCollapsedHeight);
				else if (!node.isExpanded) 
					node.ExpandNode (nodeExpandedHeight);
			}

			if (GUILayout.Button ("X", EditorStyles.miniButton, GUILayout.Width (button))) {
				map.RemoveNodeAtIndex (id);
				selectedNode = Mathf.Clamp (selectedNode, 0, map.nodes.Count - 1);

			}
			GUILayout.EndHorizontal ();
		}

		/// <summary>
		/// Draws as much of the fullLine, or shortLine if it is available, as it can fit.
		/// </summary>
		/// <param name="node">Node that is being worked on.</param>
		void DrawNodeDialogLine(Node node) {
			
			int displayLineHeight = 20;							// the default for a collapsed node
			string displayLine = node.fullLine;
			if (node.shortLine != "")
				displayLine = node.shortLine;

			// layout of the node display
			if (node.isExpanded) 
				displayLineHeight = nodeExpandedHeight - nodeCollapsedHeight;

			// show some of the shortLine or fullLine
			GUILayout.Label (displayLine, EditorStyles.miniLabel, 
				GUILayout.Width (nodeWidth), GUILayout.Height (displayLineHeight));
			
		}

		/// <summary>
		/// Finds the node index from reference identifier.
		/// </summary>
		/// <returns>The node index from reference identifier.</returns>
		/// <param name="refId">Reference identifier.</param>
		int FindNodeIndexFromRefId(ushort refId)
		{
			DialogMap map = dialogDatabase.maps [selectedMap];
			for (int i = 0; i < map.nodes.Count; i++) {
				Node n = map.nodes [i];
				if (n.RefId == refId)
					return i;
			}

			Debug.Log ("FindNodeIndexFromRefId should never be unable to find the node!");
			return -1;
		}

		/// <summary>
		/// Finds the nodes index based on the reference id.
		/// </summary>
		/// <returns>The nodes index based on the reference id.</returns>
		/// <param name="refId">The reference id of the node being searched for.</param>
		Node FindNodeFromRefId(ushort refId) {

			DialogMap map = dialogDatabase.maps [selectedMap];
			for (int i = 0; i < map.nodes.Count; i++) {
				Node n = map.nodes [i];

				if (n.RefId == refId)
					return n;
			}

			// node not found, return null
			Debug.Log("Reference Id not found!");
			return null;
		}

		/// <summary>
		/// Finds the child order within the parents list of children.
		/// </summary>
		/// <returns>The index of the child in the parents list of children.</returns>
		/// <param name="child">Child node being searched for.</param>
		/// <param name="parent">Parent node being examined.</param>
		int FindChildOrder(Node child, Node parent) {

			if (parent.children.Count > 0) {

				for (int i = 0; i < parent.children.Count; i++) {

					// return the index containing the child
					if (child.RefId == parent.children [i])
						return i;
				}
			}

			// child not found, return a null int
			Debug.Log("Parent does not contain a reference to this child!");
			return new int ();
		}

		/// <summary>
		/// Draw the parent node connections one by one to a max set in the node class.
		/// </summary>
		/// <param name="node">Node being drawn.</param>
		void DrawNodeParentButtons(Node node) {

			int button = 17;
			childOrderButtonsPanel.x = 3 + button + 8;

			GUILayout.BeginArea (childOrderButtonsPanel);
			GUILayout.BeginHorizontal ();
			for (int i = 0; i < node.parents.Count; i++) {
				Node parent = FindNodeFromRefId(node.parents[i]);
				int order = parent != null ? FindChildOrder (node, parent) : 9;



				if (GUILayout.Button (order.ToString(), EditorStyles.miniButton, GUILayout.Width (button)))
					Debug.Log ("Clicked " + i.ToString());

			}
			GUILayout.EndHorizontal ();
			GUILayout.EndArea ();

		}

		/// <summary>
		/// Get the reference id of the node that is currently searching for a child.
		/// </summary>
		/// <returns>The reference id of the node currently searching for a child.</returns>
		ushort GetParentRefId() {
			DialogMap map = dialogDatabase.maps [selectedMap];

			for (int i = 0; i < map.nodes.Count; i++) {
				Node n = map.nodes [i];

				if (n.wantsChild)
					return n.RefId;
			}

			// no one wants a child, return a null ushort
			Debug.Log("No one is searching for a child!");
			return new ushort ();
		}

		/// <summary>
		/// Draw the controls that allow parents to connect to this child.
		/// </summary>
		/// <param name="node">Node being drawn.</param>
		/// <param name="id">Identifier for the window, it is also the index of the node.</param>
		void DrawNodeTopControls(Node node, int id) {

			int button = 20;
			int idWidth = 50;
			int toRight = nodeWidth - ((int)childOrderButtonsPanel.width - 28);
			string idText = id.ToString () + ":<color=#ff0000ff>" + node.RefId.ToString () + "</color>";
			GUIStyle idStyle = new GUIStyle (EditorStyles.miniLabel);
			GUIStyle upArrow = new GUIStyle (EditorStyles.miniButton);
			idStyle.richText = true;
			idStyle.alignment = TextAnchor.MiddleRight;
			upArrow.fontSize = 10;
			upArrow.fontStyle = FontStyle.Bold;
			upArrow.normal.textColor = Color.blue;

			// receive connection from parent node
			GUILayout.BeginHorizontal();
			if (GUILayout.Button ("^", upArrow, GUILayout.Width (button))) {
				Node pNode = FindNodeFromRefId (GetParentRefId ());
				if (pNode != null) {
					node.ConnectToParent (GetParentRefId ());
					pNode.ConnectToChild (node.RefId);
					pNode.wantsChild = false;
				}
			}

			// the buttons that show selection order and can be clicked to switch the order between children
			DrawNodeParentButtons (node);

			// draw the index and reference id in the top right
			GUILayout.Space (toRight);
			GUILayout.Label (idText, idStyle, GUILayout.Width(idWidth));
			GUILayout.EndHorizontal ();
		}

		/// <summary>
		/// Draws the node and manages its layout.
		/// </summary>
		/// <param name="id">Index of the node in the dialog maps list of nodes.</param>
		void DrawNode(int id) {

			DialogMap map = dialogDatabase.maps [selectedMap];
			Node node = map.nodes [id];

			// on a left-click, select this node
			if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
				selectedNode = id;


			// place on grid
			node.rect.x -= node.rect.x % mapGridSpacing;
			node.rect.y -= node.rect.y % mapGridSpacing;

			// keep node in screen
			node.rect.x = Mathf.Clamp(node.rect.x, nodeBackgroundScrollPosition.x, 
				nodeBackgroundScrollPosition.x + nodeBackgroundDisplay.x - nodeWidth - scrollThickness);
			int currentNodeHeight = node.isExpanded ? nodeExpandedHeight : nodeCollapsedHeight;
			node.rect.y = Mathf.Clamp (node.rect.y, nodeBackgroundScrollPosition.y,
				nodeBackgroundScrollPosition.y + nodeBackgroundDisplay.y - currentNodeHeight - scrollThickness);

			// buttons handling connections to parents
			DrawNodeTopControls (node, id);

			// show the fullLine, or shortLine if available
			DrawNodeDialogLine (node);

			// buttons handling connections to children and collapsing or deleting the node
			DrawNodeBottomControls (node, id);

			GUI.DragWindow ();

		}

		/// <summary>
		/// Iterate through the nodes and draw them on the map display.
		/// </summary>
		void ShowNodeMapDisplay() {

			// early out
			if (dialogDatabase.maps [selectedMap].nodes.Count == 0)
				return;

			List<Node> nodes = dialogDatabase.maps [selectedMap].nodes;

			BeginWindows ();

			// ==========
			// DRAW THE LINE RIGHT HERE!!!
			// ==========

			// show the nodes
			for (int i = 0; i < nodes.Count; i++) {
				Node node = nodes [i];

				// draw the node and keep its location updated
				node.rect = GUILayout.Window (i, node.rect, DrawNode, "", EditorStyles.helpBox);
			}

			EndWindows ();
		}

		/// <summary>
		/// Switch the display type from Map to List or List to Map.
		/// </summary>
		void ChangeNodeDisplayType() {

			if (nodeDisplayMethod == NodeDisplayMethod.Map)
				nodeDisplayMethod = NodeDisplayMethod.List;
			else if (nodeDisplayMethod == NodeDisplayMethod.List)
				nodeDisplayMethod = NodeDisplayMethod.Map;
		}

		/// <summary>
		/// Figure out what type of node display we're using.
		/// </summary>
		/// <returns>The opposite display type as a string to be used in the ShowNodeDisplay button.</returns>
		string FindNodeDisplayType() {

			string changeTo = "";

			// which display method to use
			switch (nodeDisplayMethod) {
			case NodeDisplayMethod.Map:
				changeTo = "Switch To List Display";
				break;
			case NodeDisplayMethod.List:
				changeTo = "Switch To Map Display";
				break;
			}

			return changeTo;
		}

		/// <summary>
		/// Updates dialog for all speakers that are attached to dialog maps.
		/// </summary>
		void UpdateAllSpeakers() {

			for (int i = 0; i < dialogDatabase.maps.Count; i++) {
				DialogMap map = dialogDatabase.maps [i];

				// only look at maps with speakers assigned
				if (map.speakerObject != null) {

					// early out if there's no 'Dialog' component on the assigned gameObject
					if (map.speakerObject.GetComponent<Dialog> () == null)
						return;
					
					Dialog speakerDialog = map.speakerObject.GetComponent<Dialog> ();

					// this speaker needs to be updated
					if (!speakerDialog.CompareToDialogMap (map))
						speakerDialog.UpdateDialog (map);
				}
			}
		}

		/// <summary>
		/// Checks the status of the dialog maps and whether the gameObjects they are attached to are currently updated,
		/// setting the button color to red and changing the display text to show what needs attention.
		/// </summary>
		/// <returns>Text to be displayed on the button.</returns>
		/// <param name="updateAllStyle">Style colored red if attention is needed.</param>
		string PrepareUpdateAllButton(ref GUIStyle updateAllStyle) {

			string displayText = "Speakers are Assigned And Updated";
			List<int> speakersToUpdate = new List<int> ();
			List<int> mapsToAssign = new List<int> ();

			for (int i = 0; i < dialogDatabase.maps.Count; i++) {

				// only look at speakers with objects assigned
				if (dialogDatabase.maps [i].speakerObject != null) {

					// alert if there's no Dialog component on the assigned gameObject
					if (dialogDatabase.maps [i].speakerObject.GetComponent<Dialog> () == null) {
						displayText = "Speaker " + i.ToString () + " has no Dialog component!";
						updateAllStyle.normal.textColor = Color.red;
						return displayText;
					}

					Dialog speakerDialog = dialogDatabase.maps [i].speakerObject.GetComponent<Dialog> ();

					// this speaker needs to be updated
					if (!speakerDialog.CompareToDialogMap (dialogDatabase.maps [i]))
						speakersToUpdate.Add(i);

				// this map has no object assigned
				} else {
					mapsToAssign.Add(i);
				}
			}


			// there are speakers to update - this takes priority over maps to assign
			if (speakersToUpdate.Count > 0) {
				displayText = "Update: ";
				for (int i = 0; i < speakersToUpdate.Count; i++) {
					displayText += speakersToUpdate[i].ToString ();
					if (i != speakersToUpdate.Count - 1)
						displayText += ", ";
				}
				updateAllStyle.normal.textColor = Color.red;
			}

			// there are maps to assign - this is written over if there are speakers to update
			else if (mapsToAssign.Count > 0) {
				displayText = "Assign Maps: ";
				for (int i = 0; i < mapsToAssign.Count; i++) {
					displayText += mapsToAssign[i].ToString ();
					if (i != mapsToAssign.Count - 1)
						displayText += ", ";
				}
				updateAllStyle.normal.textColor = Color.red;
			}


			return displayText;
		}

		/// <summary>
		/// Shows the nodes in a manageable display, either a map or a list.
		/// </summary>
		void ShowNodeDisplay() {

			// early out
			if (dialogDatabase.maps.Count == 0)
				return;

			int displayButtonWidth = 150;
			int addNodeButtonWidth = 80;
			int updateAllButtonWidth = 250;
			int nodeX = (int)((nodeBackgroundScrollPosition.x + 140) - (nodeBackgroundScrollPosition.x % mapGridSpacing));
			int nodeY = (int)((nodeBackgroundScrollPosition.y + 5) - (nodeBackgroundScrollPosition.y % mapGridSpacing));
			Rect newNodeLocation = new Rect (nodeX, nodeY, nodeWidth, nodeExpandedHeight);
			string changeDisplayTo = FindNodeDisplayType();
			GUIStyle updateAllStyle = new GUIStyle (EditorStyles.miniButton);
			string updateAllText = PrepareUpdateAllButton (ref updateAllStyle);

			DialogMap map = dialogDatabase.maps [selectedMap];


			GUILayout.BeginHorizontal ("helpbox");
			// node display controls
			if (GUILayout.Button (changeDisplayTo, EditorStyles.miniButton, GUILayout.Width (displayButtonWidth)))
				ChangeNodeDisplayType ();
			if (GUILayout.Button ("Add Node", EditorStyles.miniButton, GUILayout.Width (addNodeButtonWidth)))
				map.AddNode (newNodeLocation);
			if (GUILayout.Button (updateAllText, updateAllStyle, GUILayout.Width (updateAllButtonWidth)))
				UpdateAllSpeakers ();

			GUILayout.EndHorizontal ();

			// scrolling window
			nodeBackgroundScrollPosition = GUILayout.BeginScrollView (nodeBackgroundScrollPosition, "textfield", 
				GUILayout.Width (nodeBackgroundDisplay.x), GUILayout.Height(nodeBackgroundDisplay.y));

			// the actual dark background that forces the scrolling window to scroll all the time
			GUI.color = Color.black;
			GUILayout.Label ("", EditorStyles.helpBox,
				GUILayout.Width (nodeBackgroundDimensions.x), GUILayout.Height (nodeBackgroundDimensions.y));
			GUI.color = defaultGUIColor;

			// display the nodes in the appropriate manner
			switch (nodeDisplayMethod) {
			case NodeDisplayMethod.Map: 
				ShowNodeMapDisplay ();
				break;
			case NodeDisplayMethod.List:
				Debug.Log ("Show List Display");
				break;
			}

			GUILayout.EndScrollView ();

		}

		/// <summary>
		/// Prepares the update button.
		/// </summary>
		/// <returns>The Dialog component of the gameObject attached to the speaker.</returns>
		/// <param name="map">The dialog map being Compareed.</param>
		/// <param name="updateButtonText">Update button text.</param>
		/// <param name="buttonStyle">Button style.</param>
		Dialog PrepareUpdateButton(DialogMap map, ref string updateButtonText, ref GUIStyle buttonStyle) {

			// if there's no 'HasDialog' component on the assigned gameObject
			if (map.speakerObject.GetComponent<Dialog> () == null) {
				updateButtonText = "No Dialog Component!";
				buttonStyle.normal.textColor = Color.red;
				return null;
			}	

			// the component is present, compare them
			Dialog speakerDialog = map.speakerObject.GetComponent<Dialog> ();

			if (!speakerDialog.CompareToDialogMap (map)) {
				buttonStyle.normal.textColor = Color.red;
				updateButtonText = "Update!";
			}

			return speakerDialog;
		}

		/// <summary>
		/// Show the dialog map detail panel.
		/// </summary>
		void ShowDialogMapDetailPanel(DialogMap map) {

			// early out
			if (dialogDatabase.maps.Count == 0)
				return;

			int nameWidth = 290;
			int indexWidth = 90;
			int refIdWidth = 100;
			int nodesWidth = 90;
			GUIStyle nameStyle = new GUIStyle (EditorStyles.label);
			GUIStyle indexStyle = new GUIStyle (EditorStyles.miniLabel);
			GUIStyle refStyle = new GUIStyle (EditorStyles.miniLabel);
			GUIStyle nodeStyle = new GUIStyle (EditorStyles.miniLabel);
			nameStyle.fontStyle = FontStyle.Bold;
			indexStyle.fontStyle = FontStyle.Bold;
			refStyle.normal.textColor = Color.red;
			refStyle.fontStyle = FontStyle.Bold;
			nodeStyle.normal.textColor = Color.blue;
			nodeStyle.fontStyle = FontStyle.Bold;

			GUILayout.BeginVertical ("helpbox");

			// speakerDisplayName
			GUILayout.Label (map.speakerDisplayName, nameStyle, GUILayout.Width (nameWidth));

			// index, RefId, and total nodes
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Index: " + selectedMap.ToString (), indexStyle, GUILayout.Width (indexWidth));
			GUILayout.Label ("RefId: " + map.RefId.ToString (), refStyle, GUILayout.Width (refIdWidth));
			GUILayout.Label ("Nodes: " + map.nodes.Count.ToString (), nodeStyle, GUILayout.Width (nodesWidth));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			map.speakerObject = (GameObject)EditorGUILayout.ObjectField (map.speakerObject, typeof(GameObject), false);

			if (map.speakerObject != null) {
				string updateButtonText = "Updated";
				GUIStyle buttonStyle = new GUIStyle (EditorStyles.miniButton);

				Dialog speakerDialog = PrepareUpdateButton (map, ref updateButtonText, ref buttonStyle);
				if (GUILayout.Button (updateButtonText, buttonStyle))
				if (speakerDialog != null)
					speakerDialog.UpdateDialog (map);

			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
		}

		/// <summary>
		/// Show the results of the search as rows of buttons that, when clicked, select the dialog map.
		/// </summary>
		void ShowSearchResults() {

			int matchButtonWidth = 260;
			int heightOfSpeakerBox = 26;

			// scrolling window
			dialogMapSearchScrollPosition = GUILayout.BeginScrollView (dialogMapSearchScrollPosition, 
				GUILayout.Width (dialogMapSearchDisplay.x), GUILayout.Height (dialogMapSearchDisplay.y));

			// a search string exists
			if (searchedMap.Length > 0) {
				GUIStyle resultButton = new GUIStyle (EditorStyles.miniButton);
				resultButton.alignment = TextAnchor.MiddleLeft;

				for (int i = 0; i < dialogDatabase.maps.Count; i++) {

					// match found
					if (dialogDatabase.maps [i].speakerDisplayName.ToUpper ().Contains (searchedMap.ToUpper ())) {

						string displayString = i.ToString () + ": " + dialogDatabase.maps [i].speakerDisplayName;

						GUILayout.BeginHorizontal ();
						// show the match as a button that selects the speaker and shows it in the speaker stack
						if (GUILayout.Button (displayString, resultButton, GUILayout.Width (matchButtonWidth))) {
							dialogMapStackScrollPosition.y = i * heightOfSpeakerBox;
							selectedMap = i;
						}
						GUILayout.EndHorizontal ();

					}

				}
			}

			GUILayout.EndScrollView ();

		}

		/// <summary>
		/// Show the search panel which will contain the search results as buttons in a scrollable display.
		/// </summary>
		void ShowSearchPanel() {

			int maxChars = 50;
			int inputWidth = 203;
			GUIStyle labelStyle = new GUIStyle (EditorStyles.label);
			labelStyle.fontStyle = FontStyle.Bold;

			GUILayout.BeginVertical ("helpbox");
			// search field
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Search: ", labelStyle, GUILayout.ExpandWidth (false));
			searchedMap = GUILayout.TextField (searchedMap, maxChars, "helpbox", GUILayout.Width (inputWidth));
			GUILayout.EndHorizontal ();

			// results in a scrolling window
			GUILayout.Space(2);
			ShowSearchResults ();
			GUILayout.EndVertical ();
		}

		/// <summary>
		/// Deletes the dialog map from the dialogDatabase.asset.
		/// </summary>
		/// <param name="index">Index of the dialog map to remove.</param>
		void DeleteDialogMap(int index) {

			// remove the speaker and make sure selectedMap is not out of bounds
			dialogDatabase.maps.RemoveAt (index);
			selectedMap = Mathf.Clamp (selectedMap, 0, dialogDatabase.maps.Count - 1);
		}

		/// <summary>
		/// Draw the dialog map in a box that's easily stackable.
		/// </summary>
		/// <param name="index">Index of the dialog map in dialogDatabase.maps[].</param>
		/// <param name="labelStyle">Label style of the dialog map for 'selected', 'searched', or normal.</param>
		void ShowDialogMap(int index, GUIStyle labelStyle) {

			int indexWidth = 28;
			int nameWidth = 174;

			GUILayout.BeginHorizontal ("box");
			GUILayout.Label (index + ":", EditorStyles.centeredGreyMiniLabel, GUILayout.Width (indexWidth));
			if (GUILayout.Button (dialogDatabase.maps[index].speakerDisplayName, labelStyle, GUILayout.Width (nameWidth)))
				selectedMap = index;
			if (GUILayout.Button ("$", EditorStyles.miniButton, GUILayout.ExpandWidth (false)))
				dialogDatabase.maps [index].speakerDisplayName = speakerDisplayName;
			if (GUILayout.Button ("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
				DeleteDialogMap (index);
			}
			GUILayout.EndHorizontal ();

		}

		/// <summary>
		/// Show the dialog maps as boxes stacked in a vertical scrolling window.
		/// </summary>
		void ShowDialogMapStack() {

			GUIStyle labelStyle = new GUIStyle (EditorStyles.label);

			GUILayout.BeginVertical ("helpbox");
			// scrolling window
		dialogMapStackScrollPosition = GUILayout.BeginScrollView (dialogMapStackScrollPosition, 
				GUILayout.Width (dialogMapStackDisplay.x), GUILayout.Height(dialogMapStackDisplay.y));

			for (int i = 0; i < dialogDatabase.maps.Count; i++) {
				DialogMap map = dialogDatabase.maps [i];
				if (selectedMap == i)
					labelStyle.fontStyle = FontStyle.Bold;
				else if (searchedMap != "" && map.speakerDisplayName.ToUpper().Contains(searchedMap.ToUpper()))
					labelStyle.normal.textColor = Color.blue;

				ShowDialogMap (i, labelStyle);

				labelStyle = new GUIStyle (EditorStyles.label);	// reset the label style for the next loop
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
		}

		/// <summary>
		/// Adds a new speaker to dialog.asset with the name from the input field.
		/// </summary>
		void AddDialogMap() {
			DialogMap newMap = new DialogMap (dialogDatabase);

			if (speakerDisplayName == "")
				newMap.speakerDisplayName = "NO NAME";
			else
				newMap.speakerDisplayName = speakerDisplayName;
			
			dialogDatabase.maps.Add (newMap);
			selectedMap = dialogDatabase.maps.Count - 1;
		}

		/// <summary>
		/// Always present on the top of the left column.  Creates a new speaker with the name from the input field.
		/// </summary>
		void ShowAddDialogMapPanel() {

			int maxChars = 40;
			int inputWidth = 197;

			GUILayout.BeginHorizontal ("helpbox", GUILayout.Width(dialogMapStackDisplay.x));
			speakerDisplayName = GUILayout.TextField (speakerDisplayName, maxChars, GUILayout.Width (inputWidth));
			if (GUILayout.Button ("Add Dialog Map", EditorStyles.miniButton, GUILayout.ExpandWidth (false)))
				AddDialogMap ();

			GUILayout.EndHorizontal ();
		}

		/// <summary>
		/// Opens a dialogDatabase.asset file
		/// </summary>
		void OpenDialogDatabase() {
			string absPath = EditorUtility.OpenFilePanel ("Select Dialog Database", "", "");
			if (absPath.StartsWith (Application.dataPath)) {
				string relPath = absPath.Substring (Application.dataPath.Length - "Assets".Length);
				dialogDatabase = AssetDatabase.LoadAssetAtPath (relPath, typeof(DialogDatabase)) as DialogDatabase;

				// if a list of dialog maps is not already present, make one
				if (dialogDatabase.maps == null)
					dialogDatabase.maps = new List<DialogMap> ();

				// open the dialog database asset
				if (dialogDatabase)
					EditorPrefs.SetString ("ObjectPath", relPath);
			}
		}

		/// <summary>
		/// Creates the new dialogDatabase.asset file
		/// </summary>
		void CreateNewDialogDatabase() {
			
			// There is no overwrite protection here!
			// There is No "Are you sure you want to overwrite your existing object?" if it exists.
			// This should probably get a string from the user to create a new name and pass it ...
			dialogDatabase = CreateDialog.Create();
			if (dialogDatabase) {
				string relPath = AssetDatabase.GetAssetPath(dialogDatabase);
				EditorPrefs.SetString ("ObjectPath", relPath);
			}
		}

		/// <summary>
		/// Called every time the editor is focused, load the dialogDatabase.asset and get the default GUI.color.
		/// </summary>
		void OnEnable() {
			if (EditorPrefs.HasKey ("ObjectPath")) {
				string objectPath = EditorPrefs.GetString ("ObjectPath");
				dialogDatabase = AssetDatabase.LoadAssetAtPath (objectPath, typeof(DialogDatabase)) as DialogDatabase;
				defaultGUIColor = GUI.color;
			}
		}

		/// <summary>
		/// Allow access to this editor through the Window menu.
		/// </summary>
		[MenuItem ("Window/Dialog Editor %#e")]
		static void Init() {
			EditorWindow.GetWindow (typeof(DialogEditor));
		}
	}



}


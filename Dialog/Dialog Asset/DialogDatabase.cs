using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WanzerWeave.DialogEditor {


	/// <summary>
	/// This is the dialog.asset file and will contain all the dialog for the game, separated according to dialog maps.
	/// </summary>
	public class DialogDatabase : ScriptableObject {
	
		public List<DialogMap> maps;

		// reference id generator to keep track of dialog maps
		ushort _mapRefId = 0;
		public ushort MapRefId {
			get { return _mapRefId; }
			private set { _mapRefId = value; }
		}

		/// <summary>
		/// Resets the dialog map reference identifier.
		/// </summary>
		public void ResetMapRefId() {
			MapRefId = 0;
		}

		/// <summary>
		/// Gets an unused dialog map reference identifier and prepares the next one.
		/// </summary>
		/// <returns>The speaker reference identifier.</returns>
		public ushort GetMapRefId() {
			ushort refId = MapRefId;
			MapRefId++;
			return refId;
		}
	}



}


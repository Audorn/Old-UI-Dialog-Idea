using UnityEngine;
using System.Collections;
using UnityEditor;

namespace WanzerWeave.DialogEditor {

	public class CreateDialog {
		[MenuItem("Assets/Create/DialogDatabase")]
		public static DialogDatabase Create()
		{
			DialogDatabase asset = ScriptableObject.CreateInstance<DialogDatabase>();

			AssetDatabase.CreateAsset(asset, "Assets/Data/Dialog/DialogDatabase.asset");
			AssetDatabase.SaveAssets();
			return asset;
		}
	}

}

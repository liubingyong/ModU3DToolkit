using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlayerPrefsTool : EditorWindow
{
	[MenuItem("ModU3DToolkit/PlayerPrefs/Clear PlayerPrefs")]
	static public void ClearPlayerPrefs()
	{
		PlayerPrefs.DeleteAll ();	
	}
}
using UnityEditor;
using UnityEngine;

namespace Ivyyy
{
    [CustomEditor(typeof(DialogContainer))]
    public class DialogContainerEditor : Editor
    {
	    public override void OnInspectorGUI()
	    {
		    //base.OnInspectorGUI();
		    EditorGUILayout.PropertyField(serializedObject.FindProperty("blackBoardList"));
		    serializedObject.ApplyModifiedProperties();
		
		    EditorGUILayout.Space();
		
		    if (((DialogContainer)target).blackBoardList != null)
		    {

			    if (GUILayout.Button("Open Custom Graph Window"))
				    DialogGraph.OpenDialogGraphWindow((DialogContainer)target);
		    }
		    else
			    GUILayout.Label ("Please assign a valid BlackBoardList!");
		
	    }
    }

}

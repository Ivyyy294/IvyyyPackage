using UnityEditor;

namespace Ivyyy
{

    [CustomEditor(typeof(EditBlackBoardValue))]
    public class EditBlackBoardValueEditor : Editor
    {
	    public override void OnInspectorGUI()
	    {
		    EditBlackBoardValue obj = (EditBlackBoardValue) target;

		    EditorGUILayout.PropertyField(serializedObject.FindProperty("blackBoardList"));
		    EditorGUILayout.Space();

		    if (obj.blackBoardList != null)
		    {
			    SerializedProperty guid = serializedObject.FindProperty("propertyGuid");

			    int index = obj.blackBoardList.GetGuidIndex (guid.stringValue);
			    index = EditorGUILayout.Popup ("Name", index, obj.blackBoardList.GetPropertyNameList().ToArray());
			    guid.stringValue = obj.blackBoardList.GetIndexGuid (index);
			    EditorGUILayout.PropertyField(serializedObject.FindProperty("editTyp"));
			    EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
		    }

		    serializedObject.ApplyModifiedProperties();
	    }
    }
}
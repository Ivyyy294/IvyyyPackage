using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ivyyy
{
    public class DialogEditValueNode : DialogNode
    {
	    DialogGraphView dialogGraphView;
	    DropdownField dropdownField;

        public override void Init ()
	    {
		    AddTitleEditField();
		    AddInputPort();
		    AddOutputPort ("Next");

		    Foldout textFoldout = DialogGraphUtility.CreateFoldout ("Settings");

		    //Name
		    int indexName = dialogGraphView.blackBoardProperties.GetGuidIndex(data.BlackBoardProperty.guid);
		    dropdownField = DialogGraphUtility.CreateDropDownField ("Name", dialogGraphView.blackBoardProperties.GetPropertyNameList(), indexName, onValueChanged=>{OnValueChanged(onValueChanged.newValue);});
		    textFoldout.Add (dropdownField);

		    //Comparison
		    int indexTyp = (int) data.editTyp;
		    textFoldout.Add (DialogGraphUtility.CreateDropDownField ("EditTyp", Enum.GetNames(typeof(BlackBoard.EditTyp)).ToList(), indexTyp, onValueChanged => {Enum.TryParse <BlackBoard.EditTyp> (onValueChanged.newValue, out data.editTyp);}));

		    //Value
		    //ToDo check if value is valid
		    TextField txtValue = DialogGraphUtility.CreateTextField ("Value", data.BlackBoardProperty.iVal.ToString(), evt=>{data.BlackBoardProperty.iVal = int.Parse (evt.newValue); MarkDirtyRepaint();});
		    textFoldout.Add (txtValue);

		    mainContainer.Add (textFoldout);

		    MarkDirtyRepaint();
		    RefreshExpandedState();
		    RefreshPorts();
	    }

	    public override bool IsBlackBoardPropertyInUse(string guid)
	    {
		    return data.BlackBoardProperty.guid == guid;
	    }

	    public override void RefreshBlackBoardProperties ()
	    {
		    int index = dropdownField.index;
		    dropdownField.choices = dialogGraphView.blackBoardProperties.GetPropertyNameList();
		    dropdownField.index = index;
		    dropdownField.MarkDirtyRepaint();
	    }

	    public static DialogEditValueNode Create(string nodeName, Vector2 localMousePosition, DialogGraphView dialogGraphView)
	    {
		    DialogNodeData data = new DialogNodeData
		    {
			    DialogTitle = nodeName,
			    Guid =  System.Guid.NewGuid().ToString(),
			    Type = DialogNodeData.NodeType.EDIT_VALUE
		    };

		    DialogEditValueNode node = Create (data, dialogGraphView);
		    node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		    return node;
	    }

	    public static DialogEditValueNode Create (DialogNodeData data, DialogGraphView dialogGraphView)
	    {
		    DialogEditValueNode node = new DialogEditValueNode {data = data, dialogGraphView = dialogGraphView};
		    node.Init ();

		    return node;
	    }

	    private void OnValueChanged (string newName)
	    {
		    data.BlackBoardProperty = dialogGraphView.blackBoardProperties.data.First(x=>x.name == newName);
		    MarkDirtyRepaint();
	    }
    }

}

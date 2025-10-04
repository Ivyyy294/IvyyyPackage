using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ivyyy
{
    public class DialogChoiceNode : DialogNode
    {
	    //Protected Values
	    public DialogGraphView parentView;

	    public override void Init()
	    {
		    //Title
		    title = data.DialogTitle;
		    var titleField = DialogGraphUtility.CreateTextField ("Title", data.DialogTitle, evt=> {data.DialogTitle = title = evt.newValue; MarkDirtyRepaint();});
		    mainContainer.Add (titleField);

		    AddInputPort();

		    //Button
		    titleContainer.Add (DialogGraphUtility.CreateButton ("New Choice", onClick:() => {CreateChoicePort (); }));

		    MarkDirtyRepaint();
		    RefreshExpandedState();
		    RefreshPorts();
		    SetPosition (new Rect (Vector2.zero, defaultSize));
	    }

	    public void CreateChoicePort(string overriddenPortName = "")
	    {
		    var generatedPort = DialogGraphUtility.CreatePort (this, Direction.Output);

		    //Remove default name label
		    var oldLabel = generatedPort.contentContainer.Q<Label>("type");
		    generatedPort.contentContainer.Remove (oldLabel);

		    var outputPortCount = outputContainer.Query ("connector").ToList().Count;

		    string choicePortName = string.IsNullOrEmpty (overriddenPortName)
			    ? $"Choice {outputPortCount}"
			    : overriddenPortName;
		
		    //Invisible Label to enable port drag and drop
		    generatedPort.contentContainer.Add (new Label ("\t\t"));
		    var textField = DialogGraphUtility.CreateTextArea (choicePortName, evt => {generatedPort.portName = evt.newValue; MarkDirtyRepaint();});
		    generatedPort.contentContainer.Add (textField);
		
		    //Button
		    generatedPort.contentContainer.Add (DialogGraphUtility.CreateButton ("X", onClick:() =>parentView.RemovePort (this, generatedPort)));

		    generatedPort.portName = choicePortName;

		    outputContainer.Add (generatedPort);
		    RefreshPorts();
		    RefreshExpandedState();		
	    }

	    public static DialogNode Create (string nodeName, Vector2 pos, DialogGraphView _parentView) 
	    {
		    DialogNode node = Create (nodeName, _parentView);
		    node.SetPosition (new Rect (pos, DialogNode.defaultSize));

		    return node;
	    }

	    public static DialogNode Create (string nodeName, DialogGraphView _parentView)
	    {
		    DialogNodeData data = new DialogNodeData
		    {
			    DialogTitle = nodeName,
			    DialogText = "hello world",
			    Guid =  System.Guid.NewGuid().ToString(),
			    Type = DialogNodeData.NodeType.CHOICE
		    };

		    return Create (data, _parentView);
	    }

	    public static DialogNode Create (DialogNodeData nodeData, DialogGraphView _parentView)
	    {
		    var dialogNode = new DialogChoiceNode {data = nodeData, parentView = _parentView};

		    dialogNode.Init();

		    return dialogNode;
	    }
    }

}

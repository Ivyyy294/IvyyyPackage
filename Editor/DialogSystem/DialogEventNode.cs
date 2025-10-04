using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ivyyy
{
    public abstract class DialogEventNode : DialogNode
    {
        public override void Init()
	    {
		    base.Init ();

		    //Title
		    title = data.DialogTitle;
		    var titleField = DialogGraphUtility.CreateTextField ("Title", data.DialogTitle, evt=> {data.DialogTitle = title = evt.newValue; MarkDirtyRepaint();});
		    mainContainer.Add (titleField);

		    AddInputPort();

		    //Events
		    var actionField = new ObjectField("Event");
		    actionField.objectType = typeof(GameEvent);
		    actionField.RegisterValueChangedCallback(evt => data.GameEvent = evt.newValue as GameEvent);
		    actionField.SetValueWithoutNotify (data.GameEvent);
		    mainContainer.Add(actionField);

		    MarkDirtyRepaint();
		    RefreshExpandedState();
		    RefreshPorts();
		    SetPosition (new Rect (Vector2.zero, defaultSize));
	    }
    }

    public class DialogRaiseEventNode : DialogEventNode
    { 
	    public static DialogEventNode Create(DialogNodeData data)
	    {
		    DialogRaiseEventNode node = new DialogRaiseEventNode() {data = data};
		    node.Init ();
		    return node;
	    }

	    //Create Raise Event Node
	    public static DialogEventNode Create(string nodeName)
	    {
		    DialogNodeData data = new DialogNodeData
		    {
			    DialogTitle = nodeName,
			    Guid =  System.Guid.NewGuid().ToString(),
			    Type = DialogNodeData.NodeType.RAISE_EVENT
		    };

		    return Create (data);
	    }

	    public static DialogEventNode Create(string nodeName, Vector2 localMousePosition)
	    {
		    DialogEventNode node = Create (nodeName);
		    node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		    return node;
	    }
    }

    public class DialogListenEventNode : DialogEventNode
    {
	    //Create Listen Event Node
	    public static DialogListenEventNode Create(string nodeName)
	    {
		    DialogNodeData data = new DialogNodeData
		    {
			    DialogTitle = nodeName,
			    Guid =  System.Guid.NewGuid().ToString(),
			    Type = DialogNodeData.NodeType.LISTEN_EVENT
		    };

		    return Create (data);
	    }

	    public static DialogListenEventNode Create(string nodeName, Vector2 localMousePosition)
	    {
		    DialogListenEventNode node = Create (nodeName);
		    node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		    return node;
	    }

	    public static DialogListenEventNode Create(DialogNodeData data)
	    {
		    DialogListenEventNode node = new DialogListenEventNode {data = data};
		    node.Init ();
		    return node;
	    }
    }

}
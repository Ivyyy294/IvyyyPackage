using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Ivyyy
{
    public class DialogNode : Node
    {
	    //Public Values
	    public static readonly Vector2 defaultSize = new Vector2 (150, 200);

	    public bool entryPoint = false;
	    public DialogNodeData data;
	
	    //Public Functions
	    public virtual bool IsBlackBoardPropertyInUse (string guid) {return false;}
	    public virtual void RefreshBlackBoardProperties () {}

	    public virtual void Init ()
	    {
		    AddOutputPort ("Next");
	    }

	    protected void AddInputPort ()
	    {
		    //Create Input Port
		    var inputPort = DialogGraphUtility.CreatePort (this, Direction.Input, Port.Capacity.Multi);
		    inputPort.portName = "Input";
		    inputContainer.Add (inputPort);
	    }

	    protected void AddTitleEditField()
	    {
		    //Title
		    title = data.DialogTitle;
		    var titleField = DialogGraphUtility.CreateTextField ("Title", data.DialogTitle, evt=> {data.DialogTitle = title = evt.newValue; MarkDirtyRepaint();});
		    mainContainer.Add (titleField);
	    }

	    protected void AddOutputPort (string name)
	    {
		    var generatedPort = DialogGraphUtility.CreatePort (this, Direction.Output);
		    generatedPort.portName = name;
		    outputContainer.Add (generatedPort);
	    }
    }

}

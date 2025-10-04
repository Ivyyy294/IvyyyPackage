using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Ivyyy
{
    public class DialogNpcNode : DialogNode
    {
	    public override void Init()
	    {
		    base.Init ();

		    //Title
		    title = data.DialogTitle;
		    var titleField = DialogGraphUtility.CreateTextField ("Title", data.DialogTitle, evt=> {data.DialogTitle = title = evt.newValue; MarkDirtyRepaint();});
		    //titleField.SetValueWithoutNotify (title);		
		    mainContainer.Add (titleField);

		    AddInputPort();

		    //Content
		    mainContainer.Add (DialogGraphUtility.CreateTextArea (data.DialogText, evt=>{data.DialogText = evt.newValue; MarkDirtyRepaint();}));

		    Foldout textFoldout = DialogGraphUtility.CreateFoldout ("More", data.Image == null);
		
		    //Image
		    var actionField = new ObjectField();
		    actionField.objectType = typeof(Sprite);
		    actionField.RegisterValueChangedCallback(evt => data.Image = evt.newValue as Sprite);
		    actionField.SetValueWithoutNotify (data.Image);
		    textFoldout.Add (actionField);

		    TextField respondTime = DialogGraphUtility.CreateTextField ("Cutsom respond time", data.customRespondTime.ToString());
		    respondTime.RegisterValueChangedCallback (evt=>
		    {
			    string value = evt.newValue.Replace ('.', ',');

			    if (string.IsNullOrEmpty (value))
			    {
				    data.customRespondTime = 0f;
				    respondTime.SetValueWithoutNotify ("0");
				    return;
			    }

			    float tmp;

			    if (!float.TryParse (value, out tmp))
				    respondTime.SetValueWithoutNotify (evt.previousValue);
			    else
				    data.customRespondTime = tmp;
		    });
		    textFoldout.Add (respondTime);

		    mainContainer.Add(textFoldout);

		    MarkDirtyRepaint();
		    RefreshExpandedState();
		    RefreshPorts();
		    SetPosition (new Rect (Vector2.zero, defaultSize));
	    }

	    public static DialogNpcNode Create(string nodeName, Vector2 localMousePosition)
	    {
		    DialogNpcNode node = Create (nodeName);
		    node.SetPosition (new Rect (localMousePosition, DialogNode.defaultSize));

		    return node;
	    }

	    public static DialogNpcNode Create(string nodeName)
	    {
		    DialogNodeData data = new DialogNodeData
		    {
			    DialogTitle = nodeName,
			    DialogText = "hello world",
			    Guid =  System.Guid.NewGuid().ToString(),
			    Type = DialogNodeData.NodeType.NPC
		    };

		    return Create (data);
	    }
	
	    public static DialogNpcNode Create (DialogNodeData data)
	    {
		    DialogNpcNode node = new DialogNpcNode {data = data};
		    node.Init ();

		    return node;
	    }
    }


}

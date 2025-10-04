using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace Ivyyy
{
    public class DialogGraphView : GraphView
    {
	    public Blackboard blackboard;
	    public BlackBoardList blackBoardProperties = ScriptableObject.CreateInstance <BlackBoardList>();

	    public DialogGraphView()
	    {
		    SetupZoom (ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

		    this.AddManipulator (new ContentDragger());
		    this.AddManipulator (new SelectionDragger());
		    this.AddManipulator (new RectangleSelector());
		    this.AddManipulator (CreateNodeContextMenu());
		    this.AddManipulator (CreateTextNodeContextMenu());
		    this.AddManipulator (CreateRaiseEventNodeContextMenu());
		    this.AddManipulator (CreateLogicNodeContextMenu());
		    this.AddManipulator (CreateWaitNodeContextMenu());
		    this.AddManipulator (CreateEditValueNodeContextMenu());
		    this.AddManipulator (CreatePlayerAutoContextMenu());
		    this.AddManipulator (CreateGroupContextMenu());

		    ////ToDo fix stylesheet
		    //var grid = new GridBackground();
		    //Insert (0, grid);
		    //grid.StretchToParentSize();

		    //AddElement (GenerateEntryPointNode());
		    AddStyles();
	    }

	    public void ClearBlackBoard()
	    {
		    blackBoardProperties.data.Clear();
		    blackboard.Clear();
	    }

	    public void AddPropertyToBlackBoard(string v, string guid = null)
	    {
		    while (blackBoardProperties.data.Any (x=>x.name == v))
			    v += "(1)";

		    BlackBoardProperty newData = blackBoardProperties.AddValue (v, guid);

		    VisualElement container = new VisualElement ();
		    BlackboardField blackboardField = new BlackboardField { text = newData.name, typeText = newData.guid};

		    blackboardField.Add(DialogGraphUtility.CreateButton("X", onClick: () =>
		    {
			    if (IsBlackBoardPropertyInUse(blackboardField.typeText))
				    EditorUtility.DisplayDialog ("Error", "The value cannot be deleted because it is still used in the dialogue!", "OK");
			    else
			    {
				    if (EditorUtility.DisplayDialog ("Warning", "Deleting this value can lead to data loss in external dialogues. Are you sure you want to delete the value?", "Yes", "Cancel"))
				    {
					    blackBoardProperties.RemoveValue(blackboardField.typeText);
					    blackboard.Remove(container);
				    }
			    }
		    }));

		    container.Add (blackboardField);
		    blackboard.Add (container);

		    RefreshBlackBoardProperties();
	    }

	    public void RefreshBlackBoardProperties ()
	    {
		    foreach (DialogNode node in nodes.Cast <DialogNode>())
			    node.RefreshBlackBoardProperties();
	    }

	    public Vector2 GetLocalMousePosition(Vector2 mousePosition)
        {
            Vector2 worldMousePosition = mousePosition;
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }

	    public Group CreateGroup (string title, Vector2 localMousePosition)
	    {
		    Group group = DialogGraphUtility.CreateGroup (title, localMousePosition);
		    AddElement (group);

		    foreach (GraphElement selectedElement in selection)
		    {
			    if (!(selectedElement is DialogNode)) continue;

			    DialogNode dialogNode = (DialogNode) selectedElement;
			    group.AddElement (dialogNode);
		    }

		    return group;
	    }

	    public void RemovePort(DialogNode dialogNode, Port generatedPort)
	    {
		    var targetEdge = edges.ToList().Where (x=>x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

		    //remove edges first
		    if (targetEdge.Any())
		    {
			    var edge = targetEdge.First();
			    edge.input.Disconnect (edge);

			    RemoveElement (targetEdge.First());
		    }

		    dialogNode.outputContainer.Remove (generatedPort);
		    dialogNode.RefreshPorts();
		    dialogNode.RefreshExpandedState();
	    }

	    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
	    {
		    var compatiblePorts = new List <Port>();

		    ports.ForEach (port =>
		    {
			    if (startPort != port && startPort.node != port.node)
				    compatiblePorts.Add (port);
		    });

		    return compatiblePorts;
	    }

	    //Private Functions
	    private void AddStyles()
	    {
            var guids = AssetDatabase.FindAssets ("DSNodeStyles", null);

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath (guids[0]);
                StyleSheet nodeStyleSheet = (StyleSheet)EditorGUIUtility.Load(path);
                //StyleSheet nodeStyleSheet = (StyleSheet) EditorGUIUtility.Load ("DSNodeStyles.uss");
		        styleSheets.Add (nodeStyleSheet);
            }
	    }

	    private IManipulator CreatePlayerAutoContextMenu()
	    {
		    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			    menuEvent=> menuEvent.menu.AppendAction ("Add PlayerAutoNode", actionEvent=>AddElement (DialogPlayerAutoNode.Create("PlayerAutoNode", GetLocalMousePosition (actionEvent.eventInfo.localMousePosition)))));

		    return contextualMenuManipulator;
	    }

	    private IManipulator CreateWaitNodeContextMenu()
	    {
		    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			    menuEvent=> menuEvent.menu.AppendAction ("Add WaitNode", actionEvent=>AddElement (DialogWaitNode.Create("WaitNode", GetLocalMousePosition (actionEvent.eventInfo.localMousePosition), this))));

		    return contextualMenuManipulator;
	    }

	    private IManipulator CreateLogicNodeContextMenu()
	    {
		    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			    menuEvent=> menuEvent.menu.AppendAction ("Add LogicNode", actionEvent=>AddElement (DialogLogicNode.Create("LogicNode", GetLocalMousePosition (actionEvent.eventInfo.localMousePosition), this))));

		    return contextualMenuManipulator;
	    }

	    private IManipulator CreateListenEventNodeContextMenu()
	    {
		    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			    menuEvent=> menuEvent.menu.AppendAction ("Add ListenEventNode", actionEvent=>AddElement (DialogListenEventNode.Create("ListenEventNode", GetLocalMousePosition (actionEvent.eventInfo.localMousePosition)))));

		    return contextualMenuManipulator;
	    }

	    private IManipulator CreateTextNodeContextMenu()
	    {
		    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			    menuEvent=> menuEvent.menu.AppendAction ("Add NpcNode", actionEvent=>AddElement (DialogNpcNode.Create("NpcNode", GetLocalMousePosition (actionEvent.eventInfo.localMousePosition)))));

		    return contextualMenuManipulator;
	    }
	
	    private IManipulator CreateEditValueNodeContextMenu()
	    {
		    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			    menuEvent=> menuEvent.menu.AppendAction ("Add EditValueNode", actionEvent=>AddElement (DialogEditValueNode.Create("EditValueNode", GetLocalMousePosition (actionEvent.eventInfo.localMousePosition), this))));

		    return contextualMenuManipulator;
	    }

	    private IManipulator CreateRaiseEventNodeContextMenu()
	    {
		    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			    menuEvent=> menuEvent.menu.AppendAction ("Add RaiseEventNode", actionEvent=>AddElement (DialogRaiseEventNode.Create("RaiseEventNode", GetLocalMousePosition (actionEvent.eventInfo.localMousePosition)))));

		    return contextualMenuManipulator;
	    }

	    private IManipulator CreateNodeContextMenu()
	    {
		    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			    menuEvent=> menuEvent.menu.AppendAction ("Add ChoiceNode", actionEvent=>AddElement (DialogChoiceNode.Create("ChoiceNode", GetLocalMousePosition (actionEvent.eventInfo.localMousePosition), this))));

		    return contextualMenuManipulator;
	    }

	    private IManipulator CreateGroupContextMenu()
	    {
		    ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator (
			    menuEvent=> menuEvent.menu.AppendAction ("Add Group", actionEvent=>CreateGroup("DialogGroup", GetLocalMousePosition (actionEvent.eventInfo.localMousePosition))));

		    return contextualMenuManipulator;
	    }

	    public DialogNode GenerateEntryPointNode()
	    { 
		    var node = new DialogNode
		    {
			    entryPoint = true,
			    title = "START",

			    data = new DialogNodeData
			    {
				    DialogTitle = "START",
				    Guid = System.Guid.NewGuid().ToString(),
				    DialogText = "ENTRYPOINT",
				    Type = DialogNodeData.NodeType.START
			    }
		    };

		    node.Init ();

		    node.RefreshExpandedState();
		    node.RefreshPorts();

		    node.SetPosition (new Rect (100, 200, 100, 150));

		    return node;
	    }

	    private bool IsBlackBoardPropertyInUse(string guid)
	    {
		    foreach (DialogNode node in nodes.Cast <DialogNode>())
		    {
			    if (node.IsBlackBoardPropertyInUse (guid))
				    return true;
		    }

		    return false;
	    }
    }

}

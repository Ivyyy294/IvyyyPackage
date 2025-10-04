using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
    public class DialogTree
    {
	    public struct Node
	    {
		    public DialogNodeData data;
		    public List <NodeLinkData> ports;
	    }

	    public DialogContainer dialogContainer;

	    public Stack <Node> nodesVisited = new Stack<Node>();

	    //Traverse to the next Node
	    public void Next (string guid)
	    {
		    if (dialogContainer != null)
		    {
			    DialogNodeData dialogNodeData = dialogContainer.GetDialogNodeData (guid);

			    if (dialogNodeData != null)
				    nodesVisited.Push (new Node() {data = dialogNodeData, ports = dialogContainer.GetDialogPorts (guid)});
			    else
				    Debug.LogError("Invalid guid!");
		    }
		    else
			    Debug.LogError("Invalid dialogContainer!");
	    }

	    public void Next (int portIndex = 0)
	    {
		    if (dialogContainer != null)
			    nodesVisited.Push (Peek (portIndex));
		    else
			    Debug.LogError("Invalid dialogContainer!");
	    }

	    public DialogTree.Node Peek (int portIndex = 0)
	    {
		    Node node = new Node();

		    if (dialogContainer != null)
		    {
			    string guid = null;

			    if (nodesVisited.Count == 0)
				    guid = dialogContainer.GetStartNodeGuid();
			    else if (portIndex >= 0 && nodesVisited.Peek().ports != null
				    && portIndex < nodesVisited.Peek().ports.Count)
				    guid = nodesVisited.Peek().ports[portIndex].targetNodeGuid;

			    node = new Node() {data = dialogContainer.GetDialogNodeData (guid), ports = dialogContainer.GetDialogPorts (guid)};
		    }
		    else
			    Debug.LogError("Invalid dialogContainer!");

		    return node;
	    }

	    //Returns the current Node or null if no Node is available
	    public DialogTree.Node CurrentNode()
	    {
		    if (nodesVisited.Count > 0)
			    return nodesVisited.Peek();

		    return default(DialogTree.Node);
	    }
    }

}

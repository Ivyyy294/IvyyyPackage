using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ivyyy
{

    [System.Serializable]
    [CreateAssetMenu (menuName = "Ivyyy/Dialog")]
    public class DialogContainer : ScriptableObject
    {
	    public List <NodeLinkData> nodeLinks = new List<NodeLinkData>();
	    public List <DialogNodeData> dialogNodeData = new List<DialogNodeData>();
	    public List <DialogGroupData> dialogGroupData = new List<DialogGroupData>();
	    public BlackBoardList blackBoardList;

	    public string GetStartNodeGuid ()
	    {
		    if (dialogNodeData.Any(x=>x.Type == DialogNodeData.NodeType.START))
			    return dialogNodeData.First(x=>x.Type == DialogNodeData.NodeType.START).Guid;
		    else
			    return default (string);
	    }

	    public DialogNodeData GetDialogNodeData (string guid)
	    {
		     DialogNodeData nodeData = null;

		    if (!string.IsNullOrEmpty(guid))
			    nodeData = dialogNodeData.Find (x=> x.Guid == guid);

		    return nodeData;
	    }

	    public List <NodeLinkData> GetDialogPorts (string guid)
	    {
		    if (!string.IsNullOrEmpty (guid))
			    return nodeLinks.Where(x=> x.baseNodeGuid == guid).ToList();

		    return null;
	    }
    }
}


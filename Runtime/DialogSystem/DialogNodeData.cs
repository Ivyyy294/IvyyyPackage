using UnityEngine;

namespace Ivyyy
{
    [System.Serializable]
    public class DialogNodeData
    {
	    public enum NodeType
	    {
		    CHOICE,
		    NPC,
		    RAISE_EVENT,
		    LISTEN_EVENT,
		    LOGIC,
		    WAIT,
		    START,
		    EDIT_VALUE,
		    PLAYER_AUTO
	    }

	    //Base Values
	    public string Guid;
	    public string DialogTitle;
	    public Vector2 Position;
	    public NodeType Type = NodeType.CHOICE;

	    //Text Node Values
	    public string DialogText;
	    public Sprite Image;
	    public float customRespondTime = 0f;

	    //Event Node Values
	    public GameEvent GameEvent = null;

	    //Logic Node Values
	    public BlackBoardProperty BlackBoardProperty = new BlackBoardProperty();

	    //PUCK Node
	    public AudioAsset audioAsset;

	    //Edit Value Node
	    public BlackBoard.EditTyp editTyp;

	    public DialogNodeData Copy ()
	    {
		    DialogNodeData data = new DialogNodeData();
		    data.Guid = Guid;
		    data.DialogTitle = DialogTitle;
		    data.Position = Position;
		    data.Type = Type;
		    data.DialogText = DialogText;
		    data.Image = Image;
		    data.GameEvent = GameEvent;
		    data.BlackBoardProperty = BlackBoardProperty;
		    data.customRespondTime = customRespondTime;
		    data.audioAsset = audioAsset;
		    data.editTyp = editTyp;

		    return data;
	    }
    }
}


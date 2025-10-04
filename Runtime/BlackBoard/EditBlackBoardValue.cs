using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
    public class EditBlackBoardValue : MonoBehaviour
    {
	    public BlackBoardList blackBoardList;
	    [SerializeField] string propertyGuid;
	    [SerializeField] BlackBoard.EditTyp editTyp;
	    [SerializeField] int value;

	    public void ModifyValue()
	    {
		    BlackBoard.Me().EditValue (propertyGuid, editTyp, value);
	    }
    }
}


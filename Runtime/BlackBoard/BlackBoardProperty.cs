using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ivyyy
{
    [System.Serializable]
    public class BlackBoardProperty
    {
	    public enum ComparisonTyp
	    {
		    EQUAL,
		    GREATER,
		    LESS
	    }

	    public string name;
	    public string guid;
	    public ComparisonTyp comparisonTyp;
	    public int iVal;

	    public bool Compare (BlackBoardProperty val)
	    {
		    if (val.comparisonTyp == BlackBoardProperty.ComparisonTyp.EQUAL
			    && iVal == val.iVal)
			    return true;
		    else if (val.comparisonTyp == BlackBoardProperty.ComparisonTyp.GREATER
			    && iVal > val.iVal)
			    return true;
		    else if (val.comparisonTyp == BlackBoardProperty.ComparisonTyp.LESS
			    && iVal < val.iVal)
			    return true;
		    else
			    return false;
	    }

	    static public List <string> GetComparisonTypList()
	    {
		    List <string> optionList = new List<string>();
		    optionList.Add (BlackBoardProperty.ComparisonTyp.EQUAL.ToString());
		    optionList.Add (BlackBoardProperty.ComparisonTyp.GREATER.ToString());
		    optionList.Add (BlackBoardProperty.ComparisonTyp.LESS.ToString());
		    return optionList;
	    }
    }

}

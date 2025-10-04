using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ivyyy
{
    public class BlackBoard
    {
	    [System.Serializable]
	    public enum EditTyp
	    {
		    SET,
		    INCREASE,
		    DECREASE
	    }

	    private static BlackBoard me;
	    public Dictionary <string, BlackBoardProperty> properties = new Dictionary <string, BlackBoardProperty>();

	    private BlackBoard()
	    {
		    UpdatePropertyList();
	    }

	    public static BlackBoard Me ()
	    {
		    if (me == null)
			    me = new BlackBoard();
		    else
			    me.UpdatePropertyList();

		    return me;
	    }

	    public void Clear()
	    {
		    foreach (var i in properties)
			    i.Value.iVal = 0;
	    }

	    public void EditValue (string guid, EditTyp editTyp, int val)
	    {
		    BlackBoardProperty tmp = GetProperty (guid);

		    if (tmp == null)
			    Debug.LogError ("Invalid BlackBoardProperty!");
		    else
		    {
			    if (editTyp == EditTyp.SET)
				    tmp.iVal = val;
			    else if (editTyp == EditTyp.INCREASE)
				    tmp.iVal += val;
			    else
				    tmp.iVal -= val;
		    }
	    }

	    public BlackBoardProperty GetPropertyByName (string name)
	    {
		    foreach (var i in properties)
		    {
			    if (i.Value.name == name)
				    return i.Value;
		    }

		    return null;
	    }

	    public BlackBoardProperty GetProperty (string guid)
	    {
		    if (properties.ContainsKey (guid))
			    return properties[guid];

		    return null;
	    }

	    public string GetGuidByName (string name)
	    {
		    foreach (var i in properties)
		    {
			    if (i.Value.name == name)
				    return i.Key;
		    }
		
		    return null;
	    }

	    public  Dictionary <string, BlackBoardProperty> GetProperties()
	    {
		    return properties;
	    }

	    private void UpdatePropertyList()
	    {
		    foreach (BlackBoardList list in BlackBoardList.instances)
		    {
			    //Create a working copy of the BlackBoardProperty
			    foreach (var i in list.data)
			    {
				    if (!properties.ContainsKey (i.guid))
					    properties.Add (i.guid, new BlackBoardProperty{name = i.name, guid = i.guid});
			    }
		    }
	    }
    }

}

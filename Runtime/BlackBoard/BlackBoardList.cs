using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ivyyy
{

    [System.Serializable]
    [CreateAssetMenu (menuName = "Ivyyy/BlackBoard")]
    public class BlackBoardList : ScriptableObject
    {
	    public static List <BlackBoardList> instances = new List <BlackBoardList>();
	    public List <BlackBoardProperty> data = new List <BlackBoardProperty>();

	    BlackBoardList()
	    {
		    if (!instances.Contains (this))
			    instances.Add (this);
	    }

	    public void Awake()
	    {
		    if (!instances.Contains (this))
			    instances.Add (this);
	    }

	    public void OnDestroy()
	    {
		    if (instances.Contains (this))
			    instances.Remove (this);
	    }


	    public BlackBoardProperty AddValue (string name, string guid = null)
	    {
		    if (guid == null)
		    {
			    guid = Guid.NewGuid().ToString();

			    //Making sure the new guid is unique
			    while (data.Any (x=>x.guid == guid))
				    guid = Guid.NewGuid().ToString();
		    }
		    else if (data.Any (x=>x.guid == guid))
		    {
			    Debug.LogError ("Invalid guid! The list already contains a value with the same guid!");
			    return null;
		    }

		    BlackBoardProperty tmp = new BlackBoardProperty {name = name, guid = guid};
		    data.Add (tmp);

		    return tmp;
	    }

	    public void ChangeData(string name, string guid)
	    {
		    if (data.Any (x=>x.guid == guid))
			    data.First(x=>x.guid == guid).name = name;
		    else
			    Debug.LogError("Invalid guid! The value couldn't be changed!");
	    }

	    public void RemoveValue (string guid)
	    {
		    if (data.Any (x=>x.guid == guid))
			    data.Remove (data.First(x=>x.guid == guid));
		    else
			    Debug.LogError("Invalid guid! The value couldn't be deleted!");
	    }

	    public int GetGuidIndex (string guid)
	    {
		    int index = -1;

		    for (int i = 0; i < data.Count; ++i)
		    {
			    if (data[i].guid == guid)
			    {
				    index = i;
				    break;
			    }
		    }

		    return index;
	    }

	    public string GetIndexGuid (int index)
	    {
		    if (index >= 0 && index < data.Count)
			    return data.ElementAt (index).guid;
		    else
			    return null;
	    }

	    public List <string> GetPropertyNameList()
	    {
		    List <string> list = new List<string>();

		    foreach (var i in data)
			    list.Add (i.name);

		    return list;
	    }
    }
}


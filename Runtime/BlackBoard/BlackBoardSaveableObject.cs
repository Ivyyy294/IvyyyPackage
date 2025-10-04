using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ivyyy
{
    public class BlackBoardSaveableObject : SaveableBehaviour
    {
        public override void ReadSerializedPackageData()
        {
            SerializedPackage valuePair = new SerializedPackage();

            for (int i = 0; i < m_serializedPackage.Count; ++i)
            {
                valuePair.DeserializeData (m_serializedPackage.Value(0).GetBytes());

                string key = valuePair.Value(0).GetString ();
                int val = valuePair.Value(1).GetInt32();

			    BlackBoard.Me().EditValue (key, BlackBoard.EditTyp.SET, val);
            }
        }

        public override void SetSerializedPackageData()
        {
            SerializedPackage valuePair = new SerializedPackage ();

            foreach (var i in BlackBoard.Me().GetProperties())
            {
                valuePair.Clear();
                valuePair.AddValue (i.Key);
                valuePair.AddValue (i.Value.iVal);
                m_serializedPackage.AddValue(valuePair.GetSerializedData());
            }
        }
    }
}


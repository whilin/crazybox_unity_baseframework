using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
//using Ketta;


[System.Serializable]
public class cxLocTextData : m9ExcelRowData
{
	public string key;
	public string [] localText;

	public void Read(m9ExcelRowDataReader reader)
    {
		localText = new string[4];

		reader.GetColumnString(0, ref key);
		reader.GetColumnString(1, ref localText[0]); //kor
		reader.GetColumnString(2, ref localText[1]); //eng
		reader.GetColumnString(3, ref localText[2]); //china
		reader.GetColumnString(4, ref localText[3]); //jp
	}
}


[CreateAssetMenu(fileName = "Localization Table", menuName = "CX Framework/Create Localization Table", order = 1)]
public class cxLocalizeTable : m9ExcelDataTable
{
	public List<cxLocTextData> locTextList = new List<cxLocTextData>();
	
	public override void AddData(m9ExcelRowData data)
    {
      	locTextList.Add(data as cxLocTextData);
    }

    public override void ClearDatas()
    {
       locTextList.Clear();
    }

    public override Type GetRowDataClassType()
    {
       return typeof(cxLocTextData);
    }
}

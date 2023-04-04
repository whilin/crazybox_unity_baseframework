
// Auto generated file. DO NOT MODIFY.

using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ItemRowDataM9 : m9ExcelRowData
{
	public string Name;

	public string Description;

	public string Icon;

	public int HP;

	public int Attack;

	public float Speed;


	public  void Read(m9ExcelRowDataReader rowData)// InitData(List<string> rowData, int offsetColumn)
    {
	//	BeginRow (rowData, offsetColumn);

       rowData.GetColumnString(0, ref Name);
	}
}

[CreateAssetMenu(fileName = "ExcelTestAsset", menuName = "CX Framework/Create ExcelTestAsset")]
public class ItemDataTableM9 : m9ExcelDataTable
{
	public List<ItemRowDataM9> elements = new List<ItemRowDataM9>();

    public override void ClearDatas()
    {
        elements.Clear();
    }

    public override void AddData(m9ExcelRowData data)
	{
		elements.Add(data as ItemRowDataM9);
	}

		public override System.Type GetRowDataClassType()
		{
			return typeof(ItemRowDataM9);
		}

	// public override int GetDataCount()
	// {
	// 	return elements.Count;
	// }

	// public override m9ExcelRowData GetData(int index)
	// {
	// 	return elements[index];
	// }
}

using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor;


public interface m9ExcelRowData
{
    void Read(m9ExcelRowDataReader rowData);
}

public class m9ExcelRowDataReader
{

    List<string> rowData;
    int offsetColumn;

    List<List<string>> table;
    int tableRow;

    public m9ExcelRowDataReader(List<string> rowData, int offsetColumn, List<List<string>> table, int tableRow)
    {
        this.rowData = rowData;
        this.offsetColumn = offsetColumn;

        this.table = table;
        this.tableRow = tableRow;
    }

    // public void BeginRow(List<string> _rowData, int _offsetColumn)
    // {
    //     rowData = _rowData;
    //     offsetColumn = _offsetColumn;
    // }

    public bool HasValue(int seq)
    {
         string result = rowData[seq + offsetColumn];
          result.Trim();

        return !string.IsNullOrEmpty(result);
    }

    public void GetColumnInt(int seq, ref int outData)
    {
        int result;

        if (int.TryParse(rowData[seq + offsetColumn], out result))
            outData = result;
        else
            outData = 0;
    }
    
    public void GetColumnLong(int seq, ref long outData)
    {
        long result;

        if (long.TryParse(rowData[seq + offsetColumn], out result))
            outData = result;
        else
            outData = 0;
    }

     public void GetColumnDouble(int seq, ref double outData)
    {
        double result;

        if (double.TryParse(rowData[seq + offsetColumn], out result))
            outData = result;
        else
            outData = 0;
    }



    public void GetColumnFloat(int seq, ref float outData)
    {
        float result;

        if (float.TryParse(rowData[seq + offsetColumn], out result))
            outData = result;
        else
            outData = 0.0f;
    }

    public void GetColumnString(int seq, ref string outData)
    {
        string result = rowData[seq + offsetColumn];
        result.Trim();

        outData = result;
    }

    public void GetColumnEnum<EnumType>(int seq, ref EnumType outData)
    {
        
		string result = rowData[seq + offsetColumn];
        result.Trim();

		if(!string.IsNullOrEmpty(result))
		{
		var value = (EnumType) Enum.Parse(typeof(EnumType), result);

		outData = value;
		}
    }
    public  List<EnumType>  GetArrayEnum<EnumType>(int seq)
    {
        
		string result = rowData[seq + offsetColumn];
        result.Trim();

        List<EnumType> outDatas = new List<EnumType>();

        var resutList =  result.Split(',');

        foreach( var resultE in resutList)
        {
            resultE.Trim();

            if(!string.IsNullOrEmpty(resultE))
            {
                var value = (EnumType) Enum.Parse(typeof(EnumType), resultE);
                outDatas.Add(value);
            }
        }

        return outDatas;
    }

    public List<EnumType> GetEnumList<EnumType>(int seq)
    {
        
        List<EnumType> outDatas = new List<EnumType>();
         for(int rowNext = tableRow; ; rowNext++)
        {
            if(rowNext >= table.Count )
                break;

             string result = table[rowNext][seq + offsetColumn];

            if(string.IsNullOrEmpty(result))
                break;

            if(!string.IsNullOrEmpty(result))
            {
                var value = (EnumType) Enum.Parse(typeof(EnumType), result);

                outDatas.Add(value);
            }
        }

        return outDatas;
    }

    public string GetString(int seq)
    {
        string result = rowData[seq + offsetColumn];
        result.Trim();

        return result;
    }
    public List<string> GetStringList(int seq)
    {
        List<string> list = new List<string>();

        for(int rowNext = tableRow; ; rowNext++)
        {
            if(rowNext >= table.Count )
                break;

		    string result = table[rowNext][seq + offsetColumn];
            if(string.IsNullOrEmpty(result))
                break;

            list.Add(result);
        }

        return list;
    }

     public List<string> GetStringListVertical(int seq)
    {
        List<string> list = new List<string>();

        for(int i= 0; ; i++)
        {
            int col = seq + offsetColumn+i;
            if(col >= table[tableRow].Count )
                break;

		    string result = table[tableRow][col];
            if(string.IsNullOrEmpty(result))
                break;

            list.Add(result);
        }

        return list;
    }

     public List<string> GetStringList(int seq, int count)
    {
        List<string> list = new List<string>();

        for(int rowNext = tableRow; rowNext < (tableRow + count); rowNext++)
        {
            if(rowNext >= table.Count )
                break;

		    string result = table[rowNext][seq + offsetColumn];
           // if(string.IsNullOrEmpty(result))
            //    break;
 
            list.Add(result);
        }

        return list;
    }
}


/// <summary>
/// Table of RowData
/// </summary>
public abstract class m9ExcelDataTable : ScriptableObject
{
    public UnityEngine.Object xlsxObject;

    public string xlsxPath;

    //public string rowDataClassName;
    public int xlsxSheetIndex;
    public int dataStartRowIndex;
    public int dataStartColumnIndex;

    public List<int> multiSheet;

    public abstract void ClearDatas();

    public abstract void AddData(m9ExcelRowData data);

    public abstract System.Type GetRowDataClassType();


#if UNITY_EDITOR
    [ContextMenu("Load From Excel")]
    public void LoadFromExcel()
    {
        m9ExcelConverter.ToAsset(this);
    }
#endif
}




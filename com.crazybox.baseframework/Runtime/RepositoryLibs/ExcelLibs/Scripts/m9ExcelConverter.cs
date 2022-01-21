
#if UNITY_EDITOR

using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEditor;

public class m9ExcelConverter
{
    public static void ToAsset(m9ExcelDataTable dataTable)
    {
        try
        {
            string xlsxPath = AssetDatabase.GetAssetPath(dataTable.xlsxObject);

            dataTable.xlsxPath = xlsxPath;

            int index = xlsxPath.LastIndexOf("/") + 1;
            string fileName = xlsxPath.Substring(index, xlsxPath.LastIndexOf(".") - index);

            xlsxPath = Application.dataPath + "/../" + xlsxPath;

            dataTable.ClearDatas();

            if (dataTable.multiSheet.Count != 0)
            {
                foreach (int sheetIndex in dataTable.multiSheet)
                {
                    var sheetData = m9ExcelReader.AsStringArray(xlsxPath, sheetIndex);

                    ToAsset(dataTable, sheetData, sheetIndex);
                }
            }
            else
            {
                var sheetData = m9ExcelReader.AsStringArray(xlsxPath, dataTable.xlsxSheetIndex);

                ToAsset(dataTable, sheetData,dataTable.xlsxSheetIndex);
            }

            AssetDatabase.Refresh();
            Debug.Log("Excel DataSheet Read Completed:"+dataTable.xlsxPath);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            AssetDatabase.Refresh();
        }

       
    }

    /*
    public static void ToAsset(string xlsxPath, m9ExcelDataTable dataTable)
    {
        try
        {
            int index = xlsxPath.LastIndexOf("/") + 1;
            string fileName = xlsxPath.Substring(index, xlsxPath.LastIndexOf(".") - index);

            var sheetData = m9ExcelReader.AsStringArray(xlsxPath);

            ToAsset(fileName, dataTable, sheetData);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            AssetDatabase.Refresh();
        }
    }
    */

    public static void ToAsset(m9ExcelDataTable dataTable, m9ExcelReader.SheetData sheetData, int sheetIndex)
    {

        Type dataType = dataTable.GetRowDataClassType();

        System.Reflection.ConstructorInfo dataCtor = dataType.GetConstructor(Type.EmptyTypes);
        //  HashSet<int> ids = new HashSet<int>();

       //.. if (tableReset)
       // ..    dataTable.ClearDatas();

        for (int row = dataTable.dataStartRowIndex; row < sheetData.rowCount; ++row)
        {
            try
            {
                for (int col = 0; col < sheetData.columnCount; ++col)
                    sheetData.At(row, col).Replace("\n", "\\n");

                var rowData = sheetData.Table[row];
                if (string.IsNullOrEmpty(rowData[dataTable.dataStartColumnIndex]))
                    continue;

                m9ExcelRowDataReader rowDataReader = new m9ExcelRowDataReader(rowData, dataTable.dataStartColumnIndex,
                                                                                sheetData.Table, row);

                m9ExcelRowData inst = dataCtor.Invoke(null) as m9ExcelRowData;

                //inst.Read(rowData, dataTable.dataStartColumnIndex);
                inst.Read(rowDataReader);

                dataTable.AddData(inst);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                Debug.LogError("Sheet & Row  : "+sheetIndex+","+row);
            }
        }   
    }
}

    // public static void ReadFromExcel(this m9ExcelDataTable table)
    // {
    //     m9ExcelConverter.ToAsset(table.xlsxPath, table);
    // }

#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class DataAccess
{

    public static int ReadIdx(int[] values, int index)
    {
        return
            values == null ? 0 : (index < values.Length ? values[index] : values[values.Length - 1]);
    }

    public static int ReadIdx(List<int> values, int index)
    {
        return
            values == null ? 0 : (index < values.Count ? values[index] : values[values.Count - 1]);
    }


    public static string ReadIdx(string[] values, int index)
    {
        return
            values == null ? string.Empty : (index < values.Length ? values[index] : values[values.Length - 1]);
    }


    public static string ReadIdx(List<string> values, int index)
    {
        return
            values == null ? string.Empty : (index < values.Count ? values[index] : values[values.Count - 1]);
    }

    public static double ReadIdx(double[] values, int index)
    {
        return index < values.Length ? values[index] : values[values.Length - 1];
    }


    public static double ReadIdx(List<double> values, int index)
    {
        return index < values.Count ? values[index] : values[values.Count - 1];
    }
}

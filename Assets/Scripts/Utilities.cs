using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utilities
{
    public static List<int> UTIL_SplitIntToList(int value)
    {
        var valueText = value.ToString();
        var valueLenght = valueText.Length - 1;
        var valueList = new List<int>();
        for(var digitCount = 0;digitCount<=valueLenght;digitCount++)
        {
            valueList.Add(valueText[digitCount]);
        }
        return valueList;
    }
}

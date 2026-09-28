using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utilities
{
    public static List<int> UTIL_SplitIntToList(int value)
    {
        // Debug.Log("----Value to split : " + value);
        // Debug.Log("----Value toString : " + value.ToString());
        var valueText = value.ToString();
        var valueLenght = valueText.Length - 1;
        var valueList = new List<int>();
        for(var digitCount = 0;digitCount<=valueLenght;digitCount++)
        {
            Debug.Log("----Layer value spitted " + digitCount + " : "  + valueText[digitCount]);
            valueList.Add(valueText[digitCount]);
        }
        //Debug.Log(valueList);
        return valueList;
    }
}

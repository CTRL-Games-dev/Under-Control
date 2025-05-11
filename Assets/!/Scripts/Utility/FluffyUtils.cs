using System.Collections.Generic;

public class FluffyUtils
{
    public static List<T> ShuffleList<T>(List<T> list)
    {
        List<T> clonedList = new();
        list.ForEach(item => clonedList.Add(item));

        for (int i = clonedList.Count - 1; i > 0; i--)
        {
            var k = UnityEngine.Random.Range(0, i);
            var value = clonedList[k];
            clonedList[k] = clonedList[i];
            clonedList[i] = value;
        }

        return clonedList;
    }
    public static List<T> ShuffleList<T>(T[] list)
    {
        List<T> clonedList = new();
        foreach(var x in list) clonedList.Add(x);

        for (int i = clonedList.Count - 1; i > 0; i--)
        {
            var k = UnityEngine.Random.Range(0, i);
            var value = clonedList[k];
            clonedList[k] = clonedList[i];
            clonedList[i] = value;
        }

        return clonedList;
    }
    public static List<T> CloneList<T>(List<T> list)
    {
        List<T> clonedList = new();
        list.ForEach(item => clonedList.Add(item));
        return clonedList;
    }
}
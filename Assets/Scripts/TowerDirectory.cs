using System.Collections.Generic;
using UnityEngine;

public static class TowerDirectory
{
    private static List<Object> listOfTower = new List<Object>();
    private static string path = "Prefabs/Towers";
    private static Object[] resourceFolderTowers;

    private static void LoadResourcesFromFolder()
    {
        resourceFolderTowers = Resources.LoadAll($"{path}");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void PopulateTowerListResources()
    {
        LoadResourcesFromFolder();
        foreach (var child in resourceFolderTowers)
        {
            listOfTower.Add(child);
        }
    }

    public static List<Object> ReturnListOfTowersFromDirectory()
    {
        return listOfTower;
    }

}
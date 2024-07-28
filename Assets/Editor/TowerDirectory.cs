using System.Collections.Generic;
using UnityEngine;

public class TowerDirectory
{
    private static List<Object> listOfTower = new List<Object>();
    private static string path = "Prefabs/Towers";
    private static Object[] resourceFolderTowers;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] // load up after scene load.. so we can subscribe to event trigger (in Awake())
    private static void PopulateTowerListResources()
    {
        LoadResourcesFromFolder();
        foreach (var child in resourceFolderTowers)
        {
            listOfTower.Add(child);
            if (listOfTower.Count == resourceFolderTowers.Length) // only trigger event when list is equalled to the last item in the resource folder
            {
                EventSystemManager.Instance.TriggerResourceFolderLoaded(ReturnListOfTowersFromDirectory());
            }
        }
    }

    private static void LoadResourcesFromFolder()
    {
        resourceFolderTowers = Resources.LoadAll($"{path}");
    }

    private static List<Object> ReturnListOfTowersFromDirectory()
    {
        return listOfTower;
    }
}
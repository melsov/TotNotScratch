using UnityEngine;
public static class PrefabHelper 
{
    public static string PrefabGameThingHelperFolder = "GameThingHelper";

    public static T InstantiatePrefab<T>(string resourceRelPath) where T : Component {
        T result = Resources.Load<T>(resourceRelPath);
        if(!result) {
            throw new System.Exception("not found in Resources folder: " + resourceRelPath);
        }
        return result;
    }
    
    public static T InstantiateGameThingHelper<T>(string helperName) where T : Component {
        return InstantiatePrefab<T>(string.Format("{0}/{1}", PrefabGameThingHelperFolder, helperName));
    }
}

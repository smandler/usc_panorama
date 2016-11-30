using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 
/// The script loads the data from Presentation.JSON.
/// All information about scenes is kept in classes.
/// 
/// </summary>
public class ParseJson : MonoBehaviour {
    public static Presentation items;

    public void Start()
    {
        string url = Application.dataPath + "/Files/presentation.json";
        using (StreamReader r = new StreamReader(url))
        {
            string json = r.ReadToEnd();
            Debug.Log(json);

            items = Presentation.CreateFromJSON(json);
            
        }
    }

    public static Presentation GetPresentation() {
        return items;
    }

}


[System.Serializable]
public class Presentation
{
    public string name;
    public string author;
    public string copyright;
    public Scenes[] scenes;
    private Scenes currentScene;

    public static Presentation CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Presentation>(jsonString);
    }

    public void SetCurrentScene(Scenes current)
    {
        currentScene = current;
    }

    public Scenes GetCurrentScene()
    {
        return currentScene;
    }
}

[System.Serializable]
public class Scenes
{
    public string scene;
    public int sound_source;
    public int angle;
    public string sound;
    public Frames[] frames;
    public Clips[] clips;
    public int north;
    public int east;
    public int south;
    public int west;
}

[System.Serializable]
public class Frames
{
    public int source;
    public string image;
    public int width;
    public float hight;
}

[System.Serializable]
public class Clips
{
    public int source;
    public string video = "";
}

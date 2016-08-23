using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
            
//            Debug.Log(items.scenes[0].frames[0].image);
 //           Debug.Log(items.scenes[0].frames[1].image);
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
    public Frames[] frames;
    public int north;
    public int east;
    public int south;
    public int west;
}

[System.Serializable]
public class Frames
{
    public string image;
    public string video;
    public int width;
    public float hight;
}

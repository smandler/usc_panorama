using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ParseJson : MonoBehaviour {
    public string url = Application.dataPath + "/Files/presentation.json";

    public void Start()
    {
        using (StreamReader r = new StreamReader(url))
        {
            string json = r.ReadToEnd();
            Debug.Log(json);
            Presentation items = Presentation.CreateFromJSON(json);
            
            Debug.Log(items.scenes[0].frames[0].image);
            Debug.Log(items.scenes[0].frames[1].image);
        }
    }




}

[System.Serializable]
public class Presentation
{
    public string name;
    public string author;
    public string copyright;
    public Scenes[] scenes;

    public static Presentation CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Presentation>(jsonString);
    }
}

[System.Serializable]
public class Scenes
{
    public string scene;
    public Frames[] frames;
    public string north;
    public string east;
    public string south;
    public string west;
}

[System.Serializable]
public class Frames
{
    public string image;
    public string video;
    public int width;
    public float hight;
}

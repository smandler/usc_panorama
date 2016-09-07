using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class PresentationControl : MonoBehaviour
{
    public GameObject sound;

    public Material Frame1;
    public Material Frame2;
    public Material Frame3;
    public Material Frame4;
    public Material Frame5;
    public Material Frame6;
    public Material Frame7;
    public Material Frame8;
    public Material Frame9;
    public Material Frame10;
    public Material Frame11;
    public Material Frame12;
    public Material Frame13;
    public Material Frame14;
    public Material Frame15;
    public Material Frame16;
    public Material Frame17;
    public Material Frame18;
    public Material Frame19;
    public Material Frame20;

    private WWW ww;
    private WWW www;
    private List<Texture2D> imageFrames;

    public void Start()
    {
        LoadNewScene(0);
        LoadTextures();
    }

    public void Split(Texture2D image, int width, int height, List<Texture2D> imageFrames)
    {
        bool perfectWidth = true;// image.width % width == 0;
        bool perfectHeight = image.height % height == 0;

        int lastWidth = width;
        if (!perfectWidth)
        {
            lastWidth = image.width - ((image.width / width) * width);
        }

        int lastHeight = height;
        if (!perfectHeight)
        {
            lastHeight = image.height - ((image.height / height) * height);
        }

        int widthPartsCount = image.width / width + (perfectWidth ? 0 : 1);
        int heightPartsCount = image.height / height + (perfectHeight ? 0 : 1);

        for (int i = 0; i < widthPartsCount; i++)
        {
            for (int j = 0; j < heightPartsCount; j++)
            {
                int tileWidth = i == widthPartsCount - 1 ? lastWidth : width;
                int tileHeight = j == heightPartsCount - 1 ? lastHeight : height;

                Texture2D g = new Texture2D(tileWidth, tileHeight);
                g.SetPixels(image.GetPixels(i * width, j * height, tileWidth, tileHeight));
                g.Apply();
                imageFrames.Add(g);
            }
        }
    }

    public void LoadTextures()
    {
        // load textures
        Frame1.mainTexture = (imageFrames[0]);
        Frame2.mainTexture = (imageFrames[1]);
        Frame3.mainTexture = (imageFrames[2]);
        Frame4.mainTexture = (imageFrames[3]);
        Frame5.mainTexture = (imageFrames[4]);
        Frame6.mainTexture = (imageFrames[5]);
        Frame7.mainTexture = (imageFrames[6]);
        Frame8.mainTexture = (imageFrames[7]);
        Frame9.mainTexture = (imageFrames[8]);
        Frame10.mainTexture = (imageFrames[9]);

        Frame11.mainTexture = (imageFrames[10]);
        Frame12.mainTexture = (imageFrames[11]);
        Frame13.mainTexture = (imageFrames[12]);
        Frame14.mainTexture = (imageFrames[13]);
        Frame15.mainTexture = (imageFrames[14]);
        Frame16.mainTexture = (imageFrames[15]);
        Frame17.mainTexture = (imageFrames[16]);
        Frame18.mainTexture = (imageFrames[17]);
        Frame19.mainTexture = (imageFrames[18]);
        Frame20.mainTexture = (imageFrames[19]);

     //   SceneManager.LoadScene(1);
        Resources.UnloadUnusedAssets(); // clean
    }

    public Texture2D LoadImage(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);

            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    public IEnumerator LoadFromWWW(string url)
    {
        ww = new WWW(url);
        // wait until downloaded...
        while (!ww.isDone) { }
        yield return ww;
    }

    public IEnumerator LoadSoundWWW(string url)
    {
        www = new WWW(url);

        while (!www.isDone) { }
        yield return www;

        AudioClip presentationClip = www.GetAudioClip(true, true, AudioType.WAV);
        
        sound.GetComponent<AudioSource>().clip = presentationClip;
        sound.GetComponent<AudioSource>().Play();
    }

    public void LoadNewScene(int numScene)
    {

        Texture2D img;
        Scenes newScene;
        Presentation present = ParseJson.GetPresentation();
        string url = "";
        int frameWidth;
        int imageWidth = 20;


        Resources.UnloadUnusedAssets(); // clean

        if (numScene == 0)
            present.SetCurrentScene(present.scenes[0]);

        // get next scene
        newScene = GetNextSceneByDirection(numScene, present);

        // load scene sound
        int sound_source = newScene.sound_source;
        if (sound_source == 1)
        {
            url = "file://" + Application.dataPath + "/Files/" + newScene.sound;
            //  img = LoadImage(url);
        }
        else if (sound_source == 2) // load from WWW
        {
            url = newScene.sound;

        }
        StartCoroutine("LoadSoundWWW", url);
        
        

        // clean frames list
        imageFrames = new List<Texture2D>();

        int framesLength = 0;
        framesLength = newScene.frames.Length;

        // read frames, feel texture
        for (int i = 0; i < framesLength; i++)
        {
            img = null;
            // load image
            int source = newScene.frames[i].source;

            if (source == 1)
            {
                url = "file://" + Application.dataPath + "/Files/" + newScene.frames[i].image;
              //  img = LoadImage(url);
            }
            else if (source == 2) // load from WWW
            {
                url = newScene.frames[i].image;
      
            }
            StartCoroutine("LoadFromWWW", url);
            img = ww.texture;

            // frame width if 2 than frame feels 1/2 CAVE, if 4 than 1/4
            frameWidth = newScene.frames[i].width;

            switch (frameWidth)
            {
                case 1:
                    // full CAVE
                    imageWidth = 20;
                    break;
                case 2:
                    // 1/2 CAVE
                    imageWidth = 10;
                    break;
                case 4:
                    // 1/4 CAVE
                    imageWidth = 5;
                    break;
                default:
                    // error
                    print("Incorrect frame number.");
                    break;
            }

            if (img != null)
            {
                Split(img, img.width / imageWidth, img.height, imageFrames);
            }

            Destroy(img);
        }

        //LoadTextures();       
        present.SetCurrentScene(newScene);
    }


    public Scenes GetNextSceneByDirection(int dir, Presentation pres)
    {
        Scenes curScene = null;
        Scenes nextScene = null;
        int sceneNum = 0;

        curScene = pres.GetCurrentScene();

        switch (dir)
        {
            case 0:
                // first scene
                sceneNum = 0;
                break;
            case 4:
                sceneNum = curScene.east;
                break;
            case 3:
                sceneNum = curScene.west;
                break;
            case 2:
                sceneNum = curScene.south;
                break;
            case 1:
                sceneNum = curScene.north;
                break;
            default:
                print("Incorrect scene number.");
                break;
        }

        if (sceneNum > 0)
            sceneNum--;

        try
        { 
            nextScene = pres.scenes[sceneNum];
        }
        catch (Exception e)
        {
            print(e.Message);
            // if no scene next is current one
            nextScene = curScene;
        }

        return nextScene;
    }

}



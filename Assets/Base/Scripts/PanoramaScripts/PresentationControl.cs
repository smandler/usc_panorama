using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class PresentationControl : MonoBehaviour
{
    public GameObject sound;
    public GameObject vFrame;

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

    public Material VideoFrame;

    private WWW www_img;
    private WWW www_sound;
    private WWW www_video;
    private int sound_angle;
    private List<Texture2D> imageFrames;
    private Boolean isVideo = false;
    private MovieTexture video;
    private Boolean isPaused = false;

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

        // if there is a video frame, show videoFrame
        if (isVideo)
        {
            vFrame.SetActive(true);
            Frame13.mainTexture = (imageFrames[8]);
            Frame14.mainTexture = (imageFrames[9]);
            Frame15.mainTexture = (imageFrames[10]);
            Frame16.mainTexture = (imageFrames[11]);
            Frame17.mainTexture = (imageFrames[12]);
            Frame18.mainTexture = (imageFrames[13]);
            Frame19.mainTexture = (imageFrames[14]);
            Frame20.mainTexture = (imageFrames[15]);
        } else
        {
            vFrame.SetActive(false);
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
        }

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
        www_img = new WWW(url);
        // wait until downloaded...
        while (!www_img.isDone) { }
        yield return www_img;
    }

    public IEnumerator LoadSoundWWW(string url)
    {
        www_sound = new WWW(url);

        while (!www_sound.isDone) { }
        yield return www_sound;

  //      AudioClip presentationClip = ;
        
        sound.GetComponent<AudioSource>().clip = www_sound.GetAudioClip(true, false, AudioType.WAV);
        sound.GetComponent<AudioSource>().spread = sound_angle;

        if (!video.isPlaying)
            sound.GetComponent<AudioSource>().Play();
    }

    public IEnumerator LoadVideoWWW(string url)
    {
        sound.GetComponent<AudioSource>().Stop();

        //load videoj
        www_video = new WWW(url);
        
        while (!www_video.isDone) { }
        yield return www_video;

        video = www_video.movie;
        
        // connect sound
        vFrame.GetComponent<AudioSource>().clip = video.audioClip;
        vFrame.GetComponent<Renderer>().material.mainTexture = video;

        video.Play();
        vFrame.GetComponent<AudioSource>().Play();

        yield return null;
    }

    public void LoadNewScene(int numScene)
    {

        Texture2D img;
        Scenes newScene;
        Presentation present = ParseJson.GetPresentation();
        string url = "";
        string urlVideo = "";
        int frameWidth;
        int imageWidth = 20;

        // clean
        Resources.UnloadUnusedAssets();
        StopAllCoroutines();
        
        // vFrame is not active
        isVideo = false;
        vFrame.SetActive(false);

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

        sound_angle = newScene.angle;
        StartCoroutine("LoadSoundWWW", url);      

        // clean frames list
        imageFrames = new List<Texture2D>();

        int framesLength = 0;
        framesLength = newScene.frames.Length;

        // read frames, feel texture
        for (int i = 0; i < framesLength; i++)
        {
            img = null;
            urlVideo = "";
            // load image
            int source = newScene.frames[i].source;

            if (source == 1)
            {
                url = "file://" + Application.dataPath + "/Files/" + newScene.frames[i].image;
                if (newScene.frames[i].video != "")
                    urlVideo = "file://" + Application.dataPath + "/Files/" + newScene.frames[i].video;
            }
            else if (source == 2) // load from WWW
            {
                url = newScene.frames[i].image;
                urlVideo = newScene.frames[i].video;

            }
            StartCoroutine("LoadFromWWW", url);
            img = www_img.texture;

            // load video
            if (urlVideo != "")
            {
                isVideo = true;
                vFrame.SetActive(true);
                StartCoroutine("LoadVideoWWW", urlVideo);
                
            }
                

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
                    if (isVideo)
                    {
                        imageWidth = 4;
                    } else
                    {
                        imageWidth = 5;
                    }                  
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

    internal void ControlVideo(int mode)
    {
        try
        {
            switch (mode)
            {
                case 1:
                    // play video
                    vFrame.GetComponent<AudioSource>().clip = video.audioClip;
                    //video.Play();
                    video.Play();
                    vFrame.GetComponent<AudioSource>().Play();
                    isPaused = false;
                    // stop playing sounds
                    sound.GetComponent<AudioSource>().Pause();
                    break;
                case 2:
                    //stop video
                    video.Pause();
                    vFrame.GetComponent<AudioSource>().Pause();
                    isPaused = true;
                    // play sounds
                    sound.GetComponent<AudioSource>().Play();
                    break;
                default:
                    print("Incorrect command.");
                    break;
            }
        }
        catch (Exception e)
        {
            print(e.Message);

        }

    }
}



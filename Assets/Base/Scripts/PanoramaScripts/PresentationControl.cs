﻿using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEditor;

public class PresentationControl : MonoBehaviour
{
    public List<Texture2D> imageFrames;

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


    public void Start()
    {
        Texture2D im;
        string url = Application.dataPath + "/Files/image.jpg";

        im = LoadImage(url);

        if (im != null)
        {
            Split(im, im.width / 20, im.height);
        }
    }

    public void Split(Texture2D image, int width, int height)
    {
        imageFrames.Clear();
        bool perfectWidth = image.width % width == 0;
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

    public void LoadNextScene(int numScene)
    {

        Texture2D img;
        Presentation present = ParseJson.GetPresentation();

        Scenes[] newScene = present.scenes;
        string nextScene = newScene[0].west;
        string url = Application.dataPath + "/Files/park.jpg";

        img = LoadImage(url);

        if (img != null)
        {
            Split(img, img.width / 20, img.height);
            Debug.Log("PICTURE");
            SceneView.RepaintAll();
        }
    }
}


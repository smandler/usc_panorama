using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class SplitImages : MonoBehaviour {
    public List<Texture2D> imageTiles;
    private Renderer rd;
    private Texture2D im;
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
        //string url = "C:/Svetlana/project/Images/image.jpg"; 
        string url =  Application.dataPath + "/Images/image.jpg";
        Debug.Log(url);
        rd = GetComponent<Renderer>();
        im = LoadImage(url);

        if (im != null)
        {
            Split(im, im.width / 20, im.height);
            LoadTiles();
        }
    }

    public void Split(Texture2D image, int width, int height)
    {
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
                imageTiles.Add(g);
            }
        }
    }

    public static Texture2D LoadImage (string filePath)
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

    public void LoadTiles()
    {
        Frame1.mainTexture = (imageTiles[0]);
        Frame2.mainTexture = (imageTiles[1]);
        Frame3.mainTexture = (imageTiles[2]);
        Frame4.mainTexture = (imageTiles[3]);
        Frame5.mainTexture = (imageTiles[4]);
        Frame6.mainTexture = (imageTiles[5]);
        Frame7.mainTexture = (imageTiles[6]);
        Frame8.mainTexture = (imageTiles[7]);
        Frame9.mainTexture = (imageTiles[8]);
        Frame10.mainTexture = (imageTiles[9]);

        Frame11.mainTexture = (imageTiles[10]);
        Frame12.mainTexture = (imageTiles[11]);
        Frame13.mainTexture = (imageTiles[12]);
        Frame14.mainTexture = (imageTiles[13]);
        Frame15.mainTexture = (imageTiles[14]);
        Frame16.mainTexture = (imageTiles[15]);
        Frame17.mainTexture = (imageTiles[16]);
        Frame18.mainTexture = (imageTiles[17]);
        Frame19.mainTexture = (imageTiles[18]);
        Frame20.mainTexture = (imageTiles[19]);

    }
}


using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AutoFade : MonoBehaviour
{
    private static AutoFade instance = null;
    private string m_LevelName = "";
    private int m_LevelIndex = 0;
    private bool fading = false;
    public GameObject fadeSphere;

    private static AutoFade Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (new GameObject("AutoFade")).AddComponent<AutoFade>();
            }
            return instance;
        }
    }

    public static bool Fading
    {
        get { return Instance.fading; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }

    private void DrawQuad(Color aColor, float aAlpha)
    {
        getReal3D.GUI.BeginGUI();
          GUITexture tex = instance.gameObject.GetComponent<GUITexture>();
          if (tex == null)
          {
              tex = instance.gameObject.AddComponent<GUITexture>();
              instance.transform.localScale = Vector3.zero;
              Texture2D tex2d = new Texture2D(1, 1);
              tex2d.SetPixels(new Color[1] { Color.white });
              tex2d.Apply();
              tex.texture = tex2d;
          }
          tex.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
          tex.color = new Color(aColor.r, aColor.g, aColor.b, aAlpha);
        getReal3D.GUI.EndGUI();
    }

    private IEnumerator Fade(float aFadeOutTime, float aFadeInTime, Color aColor)
    {
              float t = 0.0f;
              while (t < 1.0f)
              {
                  yield return new WaitForEndOfFrame();
                  t = Mathf.Clamp01(t + Time.deltaTime / aFadeOutTime);
                  DrawQuad(aColor, t);
              }

              while (t > 0.0f)
              {
                  yield return new WaitForEndOfFrame();
                  t = Mathf.Clamp01(t - Time.deltaTime / aFadeInTime);
                  DrawQuad(aColor, t);
              }

              fading = false;
    }

    private void StartFade(float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        fading = true;
        StartCoroutine(Fade(aFadeOutTime, aFadeInTime, aColor));
    }

    public static void LoadScene(string aLevelName, float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        if (Fading) return;
        Instance.StartFade(aFadeOutTime, aFadeInTime, aColor);
    }

    public static void LoadScene(int aLevelIndex, float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        if (Fading) return;
        Instance.StartFade(aFadeOutTime, aFadeInTime, aColor);
    }
}

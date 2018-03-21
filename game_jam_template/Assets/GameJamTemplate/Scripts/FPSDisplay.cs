using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    public bool showfps = true;
    float deltaTime = 0.0f;
    private Rect rect;
    private GUIStyle style;
    private int desimate = 0;
    private string text;

    void Start()
    {
        int w = Screen.width, h = Screen.height;
        style = new GUIStyle();
        rect = new Rect(60, 725, w, h* 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1f);
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        desimate += 1;
        if (desimate==100)
        {
            desimate = 0;
            float msec = deltaTime * 10.0f;
            float fps = 1.0f / deltaTime;
            text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        }

    if (showfps)
        { 
            GUI.Label(rect, text, style);
        }
    }
}
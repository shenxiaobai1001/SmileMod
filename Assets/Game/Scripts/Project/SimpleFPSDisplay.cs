using UnityEngine;

public class SimpleFPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(10, 10, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        float fps = 1.0f / deltaTime;
        float ms = deltaTime * 1000.0f;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", ms, fps);

        // ±³¾°
        GUI.Box(new Rect(5, 5, 200, 40), "");
        GUI.Label(rect, text, style);
    }
}
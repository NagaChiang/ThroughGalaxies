using UnityEngine;
using System.Collections;

public class CameraResizer : MonoBehaviour {

    public int DefaultHeight;
    public int DefaultWidth;
    public Camera MainCamera;

    private bool IsFullScreen = false;

    void Start()
    {
        // Clear screen (especially for splash screen)
        GL.Clear(true, true, Color.black);

        // Resize
        OnResize(Screen.width, Screen.height);
    }

    void Update()
    {
        // Switch to fullscreen
        if(Screen.fullScreen != IsFullScreen)
        {
            OnResize(Screen.width, Screen.height);
            IsFullScreen = Screen.fullScreen;
        }
    }

    private void OnResize(Resolution windowRes)
    {
        // Resize
        float hToWDefault = (float)DefaultHeight / DefaultWidth;
        float hToWWindow = (float)windowRes.height / windowRes.width;
        Rect resizedRect = new Rect();

        if(hToWWindow >= hToWDefault)
        {
            // Width too short
            resizedRect.height = windowRes.width * hToWDefault;
            resizedRect.width = windowRes.width;
        }
        else
        {
            // Height too short
            resizedRect.height = windowRes.height;
            resizedRect.width = windowRes.height / hToWDefault;
        }

        // Center the viewport
        resizedRect.x = (windowRes.width / 2.0f) - (resizedRect.width / 2.0f);
        resizedRect.y = (windowRes.height / 2.0f) - (resizedRect.height / 2.0f);

        // Assign and record aspect rect
        MainCamera.pixelRect = resizedRect;
    }

    // Overload
    private void OnResize(int width, int height)
    {
        Resolution res = new Resolution();
        res.width = width;
        res.height = height;
        OnResize(res);
    }
}

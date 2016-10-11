using UnityEngine;
using System.Collections;

public class CameraResizer : MonoBehaviour {

    public float DefaultHeight;
    public float DefaultWidth;
    public Camera MainCamera;

    private bool IsFullScreen = false;

    void Start()
    {
        // Get window resolution
        Resolution res = new Resolution();
        res.width = Screen.width;
        res.height = Screen.height;

        // Clear screen (especially for splash screen)
        GL.Clear(true, true, Color.black);
        
        // Resize
        OnResize(res);
    }

    void Update()
    {
        // Switch to fullscreen
        if(Screen.fullScreen != IsFullScreen)
        {
            OnResize(Screen.currentResolution);
            IsFullScreen = Screen.fullScreen;
        }
    }

    private void OnResize(Resolution windowRes)
    {
        // Resize
        float hToWDefault = DefaultHeight / DefaultWidth;
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
        resizedRect.x = (windowRes.width / 2) - (resizedRect.width / 2);
        resizedRect.y = (windowRes.height / 2) - (resizedRect.height / 2);

        // Assign and record aspect rect
        MainCamera.pixelRect = resizedRect;
    }
}

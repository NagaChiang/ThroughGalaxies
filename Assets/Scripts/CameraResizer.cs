using UnityEngine;
using System.Collections;

public class CameraResizer : MonoBehaviour {

    public float DefaultHeight;
    public float DefaultWidth;
    public Camera MainCamera;

    private bool IsFullScreen = false;

    void Start()
    {
        // Resize
        OnResize(MainCamera.pixelRect);
    }

    void Update()
    {
        // Switch to fullscreen
        if(Screen.fullScreen != IsFullScreen)
        {
            OnResize(MainCamera.pixelRect);
            IsFullScreen = Screen.fullScreen;
        }
    }

    private void OnResize(Rect windowRect)
    {
        // Resize
        float hToWDefault = DefaultHeight / DefaultWidth;
        float hToWWindow = windowRect.height / windowRect.width;
        Rect resizedRect = new Rect(windowRect);
        if(hToWWindow >= hToWDefault)
        {
            // Width too short
            resizedRect.height = resizedRect.width * hToWDefault;
        }
        else
        {
            // Height too short
            resizedRect.width = resizedRect.height / hToWDefault;
        }

        // Center the viewport
        resizedRect.x = (Screen.width / 2) - (resizedRect.width / 2);
        resizedRect.y = (Screen.height / 2) - (resizedRect.height / 2);

        // Assign and record aspect rect
        MainCamera.pixelRect = resizedRect;
    }
}

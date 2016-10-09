using UnityEngine;
using System.Collections;

public class CameraResizer : MonoBehaviour {

    public float DefaultAspect;
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

    private void OnResize(Rect newAspectRect)
    {
        // Set width depending on height
        Rect resizedRect = new Rect(newAspectRect);
        resizedRect.width = resizedRect.height * DefaultAspect;

        // Center the viewport
        resizedRect.x = (Screen.width / 2) - (resizedRect.width / 2);

        // Assign and record aspect rect
        MainCamera.pixelRect = resizedRect;
    }
}

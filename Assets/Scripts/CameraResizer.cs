using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraResizer : MonoBehaviour {

    public int Width;
    public int Height;

	void Start ()
    {
        // Get camera
        Camera camera = GetComponent<Camera>();

        // set aspect ratio
        //camera.aspect = (float)Width / Height;
    }
}

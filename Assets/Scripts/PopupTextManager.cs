using UnityEngine;
using System.Collections;

public class PopupTextManager : MonoBehaviour {

    public PopupText popupText;

    private GameObject _canvas;

    public void showMessage(string text, Vector3 posWorld)
    {
        // get canvas
        if (_canvas == null)
            _canvas = GameObject.Find("Canvas");

        // project the world position to screen position
        Vector2 posScreen = Camera.main.WorldToScreenPoint(posWorld);

        // instantiate
        PopupText pop = Instantiate(popupText);
        pop.setText(text);
        pop.transform.SetParent(_canvas.transform, false);
        pop.transform.position = posScreen;
    }
}

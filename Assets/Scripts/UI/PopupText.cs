using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupText : MonoBehaviour {

    public Text UIText;
    public Animator animator;

    void Start()
    {
        // destroy game object as the animation ends
        AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfos[0].clip.length);
    }

    public void setText(string str)
    {
        UIText.text = str;
    }
}

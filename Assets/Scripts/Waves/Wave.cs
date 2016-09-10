using UnityEngine;
using System.Collections;

// base class for hazard waves
public abstract class Wave : MonoBehaviour {

    public float duration;

    public abstract void spawn(float difficulty);
}

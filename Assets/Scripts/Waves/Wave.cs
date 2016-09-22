using UnityEngine;
using System.Collections;

// base class for hazard waves
public abstract class Wave : MonoBehaviour {

    [Header("Wave")]
    public bool isBoss;
    public float duration;

    public abstract void spawn(float difficulty);
}

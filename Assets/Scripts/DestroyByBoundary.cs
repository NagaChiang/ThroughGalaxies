using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour {

	void OnTriggerExit(Collider other)
    {
        // destroy the object
        Destroy(other.gameObject);
    }

}

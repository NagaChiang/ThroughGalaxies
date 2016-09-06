using UnityEngine;
using System.Collections;

public abstract class Powerup : MonoBehaviour {

    public float verticalSpeed;
    public float rotateFactor;

    void Start()
    {
        // random speed
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = new Vector3(0.0f, 0.0f, -verticalSpeed);

        // random rotation
        rigidbody.angularVelocity = Random.insideUnitSphere * rotateFactor;
    }

    void OnTriggerEnter(Collider other)
    {
        // only for player
        if(other.tag == "Player")
        {
            // do power up effects
            doPowerup(other.gameObject.GetComponent<PlayerController>());

            // destroy this
            Destroy(gameObject);
        }
    }

    public abstract void doPowerup(PlayerController player);
}

using UnityEngine;
using System.Collections;

public abstract class Powerup : SfxBase {

    public float verticalSpeed;
    public float rotateFactor;

    // player collecting prefernces
    private const float COLLECT_SPEED = 30.0f;
    private const float COLLECT_RANGE = 5.0f;

    private bool isLooted;

    protected override void Start()
    {
        // Sfx base
        base.Start();

        // random speed
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = new Vector3(0.0f, 0.0f, -verticalSpeed);

        // random rotation
        rigidbody.angularVelocity = Random.insideUnitSphere * rotateFactor;

        // looted or not
        isLooted = false;
    }

    void FixedUpdate()
    {
        if (!isLooted)
        {
            // move towards player within a certain range
            collected(COLLECT_SPEED, COLLECT_RANGE);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // only for player
        if(other.tag == "Player" && !isLooted)
        {
            // mark as looted
            isLooted = true;

            // do power up effects
            doPowerup(other.gameObject.GetComponent<PlayerController>());

            // destroy
            destroy();
            Destroy(gameObject);
        }
    }

    private void collected(float speed, float range)
    {
        // get player
        GameObject objPlayer = GameObject.FindWithTag("Player");
        if (objPlayer == null)
            return;
        Vector3 posPlayer = objPlayer.transform.position;

        // check distance
        float distance = Vector3.Distance(transform.position, posPlayer);
        if (distance <= range)
        {
            // calculate the speed applied to the powerup
            Vector3 speedToPlayer = (posPlayer - transform.position).normalized; // direction
            speedToPlayer *= speed / distance;

            // Set speed of this powerup
            GetComponent<Rigidbody>().velocity = speedToPlayer;
        }

        // out of range
        else
        {
            // zero out the velocity
            GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, -verticalSpeed);
        }
    }

    public abstract void doPowerup(PlayerController player);
}

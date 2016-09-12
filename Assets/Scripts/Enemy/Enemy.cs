using UnityEngine;
using System.Collections;
using System;

public class Enemy : Damageable {

    public int score;
    public float tiltFactor;
    public Limit boundaryX;
    public Weapon[] weapons;

    private GameManager _gameManager;

    new void Start()
    {
        // from Damageable
        base.Start();

        // find the game manager
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("Can't find the GameManager.");
    }

    void OnTriggerEnter(Collider other)
    {   
        // hit player
        if (other.tag == "Player")
        {
            // apply damage depending on remaining health
            Damageable target = other.GetComponent<Damageable>();
            if (target != null)
                target.applyDamage(maxHealth);

            // destroy this ship
            destroy();
        }
    }

    protected void FixedUpdate()
    {
        // update position (check boundary)
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.position = new Vector3
        (
            Mathf.Clamp(rigidbody.position.x, boundaryX.min, boundaryX.max),
            rigidbody.position.y,
            rigidbody.position.z    
        );

        // update rotation (tilt)
        Quaternion quat = rigidbody.rotation;
        rigidbody.rotation = Quaternion.Euler(quat.eulerAngles.x, quat.eulerAngles.y,
                                                rigidbody.velocity.x * tiltFactor);

    }

    protected override void destroy()
    {
        // temp fix... TODO
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        // add score to game manager
        _gameManager.addScore(score);

        // explosion, destroy gameobject
        base.destroy();
    }
}
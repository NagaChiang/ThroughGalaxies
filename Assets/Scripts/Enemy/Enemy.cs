using UnityEngine;
using System.Collections;
using System;

public class Enemy : Damageable {

    [Header("Enemy")]
    public int score;
    public float tiltFactor;
    public float rotateSpeed; // prior than tilt
    public Limit boundaryX;
    public Limit boundaryZ;
    public Weapon[] weapons;
    public Weapon weaponOnDestroy; // fire on destroy

    private GameManager _gameManager;

    protected override void Start()
    {
        // from Damageable
        base.Start();

        // find the game manager
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("Can't find the GameManager.");
    }

    protected override void FixedUpdate()
    {
        // Damageable
        base.FixedUpdate();

        // update position (check boundary)
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        float posX = Mathf.Clamp(rigidbody.position.x, boundaryX.min, boundaryX.max);
        float posZ = rigidbody.position.z;
        if (boundaryZ.max > 0)
            posZ = Mathf.Clamp(posZ, boundaryZ.min, boundaryZ.max);

        rigidbody.position = new Vector3(posX, 0.0f, posZ);

        // update rotation
        if (rotateSpeed != 0)
        {
            // rotate
            Quaternion quat = rigidbody.rotation;
            float rotZ = Mathf.Repeat(quat.eulerAngles.z + rotateSpeed * Time.deltaTime, 360.0f);
            rigidbody.rotation = Quaternion.Euler(quat.eulerAngles.x, quat.eulerAngles.y, rotZ);
        }
        else
        {
            // tilt
            Quaternion quat = rigidbody.rotation;
            rigidbody.rotation = Quaternion.Euler(quat.eulerAngles.x, quat.eulerAngles.y,
                                                    rigidbody.velocity.x * tiltFactor);
        }

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
        }
    }

    public override void destroy()
    {
        // temp fix... TODO
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        // add score to game manager
        _gameManager.addScore(score);

        // show score popup text
        PopupTextManager popupManager = GameObject.FindWithTag("PopupTextManager").GetComponent<PopupTextManager>();
        if (popupManager)
            popupManager.showMessage(score.ToString(), transform.position);
        else
            Debug.LogError("Can't find the PopupTextManager.");

        // fire weapon on destroy
        if (weaponOnDestroy)
            weaponOnDestroy.fire();

        // explosion, destroy gameobject
        base.destroy();
    }
}
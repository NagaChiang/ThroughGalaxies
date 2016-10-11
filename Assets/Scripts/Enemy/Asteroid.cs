using UnityEngine;
using System.Collections;

public class Asteroid : Damageable {

    public Limit verticalSpeed;
    public float rotateFactor;
    public int score;
    public int damage;

    private GameManager _gameManager;

	new void Start ()
    {
        // from Damageable
        base.Start();

        // random speed
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = new Vector3(0.0f, 0.0f, Random.Range(verticalSpeed.min, verticalSpeed.max));

        // random rotation
        rigidbody.angularVelocity = Random.insideUnitSphere * rotateFactor;

        // find the game manager
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("Can't find the GameManager.");
    }

    void OnTriggerEnter(Collider other)
    {
        // hit player
        if(other.tag == "Player")
        {
            // apply damage depending on remaining health
            Damageable target = other.GetComponent<Damageable>();
            if (target != null)
                target.applyDamage(damage);

            // destroy this asteroid
            destroy();
        }
    }

    public override void destroy()
    {
        // add score to game manager
        _gameManager.addScore(score);

        // show score popup text
        PopupTextManager popupManager = GameObject.FindWithTag("PopupTextManager").GetComponent<PopupTextManager>();
        if (popupManager)
            popupManager.showMessage(score.ToString(), transform.position);
        else
            Debug.LogError("Can't find the PopupTextManager.");

        // explosion, destroy gameobject
        base.destroy();
    }
}

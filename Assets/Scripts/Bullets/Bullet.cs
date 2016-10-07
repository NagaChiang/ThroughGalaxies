using UnityEngine;
using System.Collections;

public enum BulletSource
{
    player,
    enemy,
}

public class Bullet : SfxBase {

    [Header("Bullet")]
    public BulletSource bulletSource;
    public GameObject[] explosions;
    public float initialSpeed;
    public float accelerationDelay;
    public float acceleration;
    public float accAcceleration;
    public int damage;
    public LineRenderer lineBulletPath;

    private float _startTime;
    private const float _RAY_LENGTH = 50.0f;
    private LineRenderer _bulletPath;

    protected override void Start ()
    {
        // Sfx base
        base.Start();

        // initial speed
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * initialSpeed;

        // record the start time for acceleration delay
        _startTime = Time.time;

        // draw bullet path
        if (lineBulletPath)
        {
            // attach instance to bullet itself
            _bulletPath = (LineRenderer)Instantiate(lineBulletPath,
                                            lineBulletPath.transform.position,
                                            lineBulletPath.transform.rotation);
            _bulletPath.transform.SetParent(gameObject.transform);
        }
    }

    void FixedUpdate()
    {
        // acceleration
        if (Time.time - _startTime >= accelerationDelay)
        {
            // acceleration
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            acceleration += accAcceleration;
            rigidbody.velocity += transform.forward * acceleration;
        }

        // update bullet path
        if(_bulletPath)
        {
            // set points
            Ray ray = new Ray(transform.position, transform.forward);
            _bulletPath.SetPosition(0, transform.position);
            _bulletPath.SetPosition(1, ray.GetPoint(_RAY_LENGTH));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ignore boundary and other bullets 
        if (other.tag == "Boundary"
            || other.tag == "Bullet"
            || other.tag == "Powerup")
        {
            // do nothing
            return;
        }

        // ignore player (bullets from player)
        else if (other.tag == "Player"
                    && bulletSource == BulletSource.player)
        {
            // do nothing
            return;
        }

        // ignore enemies (bullets from enemies)
        else if (other.tag == "Enemy"
                    && bulletSource == BulletSource.enemy)
        {
            // do nothing
            return;
        }

        // hit anything else Damageable
        else
        {
            // apply damage to it
            Damageable target = other.GetComponent<Damageable>();
            if(target != null)
                target.applyDamage(damage);

            // do things on destroy
            destroy();
        }
    }

    public override void destroy()
    {
        // Sfx
        base.destroy();

        // explosion
        foreach(GameObject obj in explosions)
            Instantiate(obj, transform.position, transform.rotation);

        // destroy game object
        Destroy(gameObject);
    }
}

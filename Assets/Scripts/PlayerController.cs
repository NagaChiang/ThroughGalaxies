using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Boundary
{
    public float xMin, xMax;
    public float zMin, zMax;
}

[System.Serializable]
public struct Weapons
{
    public Weapon Bolt;
    public Weapon Sphere;
    public Weapon Laser;
}

public class PlayerController : Damageable {

    public float moveSpeed;
    public float tiltLimit;
    public GameObject vfxExplosion;
    public Boundary boundary;
    public Weapons weapons;

    private Weapon _currentWeapon;

    void Start()
    {
        // initial properties
        _health = maxHealth;
        _currentWeapon = weapons.Bolt;
    }

    void Update()
    {
        // fire the weapon
        if (Input.GetButton("Fire1"))
            _currentWeapon.fire();
    }

	void FixedUpdate()
    {
        // handle the movements
        move();
    }

    // for other gameObject to apply damage
    public override void applyDamage(float damage)
    {
        // reduce health
        _health -= damage;

        // dead
        if(_health <= 0)
        {
            // destroy the player
            destroy();
        }
    }

    // handle the movement of the plane
    private void move()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        float axisHorizontal = Input.GetAxis("Horizontal");
        float axisVertical = Input.GetAxis("Vertical");

        // update velocity
        Vector3 movement = new Vector3(axisHorizontal, 0.0f, axisVertical);
        rigidbody.velocity = moveSpeed * movement;

        // update position
        rigidbody.position = new Vector3
        (
            Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax),
            rigidbody.position.y,
            Mathf.Clamp(rigidbody.position.z, boundary.zMin, boundary.zMax)
        );

        // update rotation
        rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, rigidbody.velocity.x * -tiltLimit);
    }

    protected override void destroy()
    {
        // explosion vfx
        Instantiate(vfxExplosion, transform.position, transform.rotation);

        // destroy this asteroid
        Destroy(gameObject);
    }
}

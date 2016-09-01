using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Limit
{
    public float min, max;
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
    public float tiltFactor;
    public Limit boundaryX;
    public Limit boundaryZ;
    public Weapons weapons;
    public RadialBar healthCircle;

    private Weapon _currentWeapon;

    new void Start()
    {
        // from Damageable
        base.Start();

        // initial properties
        _currentWeapon = weapons.Bolt;

        // update UI of health
        healthCircle.updateValues(_health, maxHealth);
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

    // add functions to update the UI of health
    public override void applyDamage(float damage)
    {
        // base function
        base.applyDamage(damage);
        
        // update UI of health
        healthCircle.updateValues(_health, maxHealth);
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
            Mathf.Clamp(rigidbody.position.x, boundaryX.min, boundaryX.max),
            rigidbody.position.y,
            Mathf.Clamp(rigidbody.position.z, boundaryZ.min, boundaryZ.max)
        );

        // update rotation
        rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, rigidbody.velocity.x * -tiltFactor);
    }
}

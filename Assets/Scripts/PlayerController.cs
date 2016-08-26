using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Boundary
{
    public float xMin, xMax;
    public float zMin, zMax;
}

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    public float tiltLimit;
    public Boundary boundary;

    private Weapon weapon;

    void Start()
    {
        // get the weapon object in children
        weapon = GetComponentInChildren<Weapon>();
    }

    void Update()
    {
        // fire the weapon
        if (Input.GetButton("Fire1"))
            weapon.fire();
    }

	void FixedUpdate()
    {
        // handle the movements
        move();
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
}

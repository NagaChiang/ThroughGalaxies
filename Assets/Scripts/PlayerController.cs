using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Limit
{
    public float min, max;
}

[System.Serializable]
public struct PlayerWeapons
{
    public PlayerWeapon Bolt;
    public PlayerWeapon Sphere;
    public PlayerWeapon Laser;
}

public class PlayerController : Damageable {

    public int initialLife;
    public float moveSpeed;
    public float tiltFactor;
    public PlayerWeapons weapons;

    [Header("Boundary")]
    public Limit boundaryX;
    public Limit boundaryZ;

    [Header("UI")]
    public HealthBar healthCircle;
    public WeaponBar weaponCircle;

    private PlayerWeapon _currentWeapon;
    private int _remainingLife;

    new void Start()
    {
        // get HUDs
        healthCircle = GameObject.Find("HealthCircle").GetComponent<HealthBar>();
        weaponCircle = GameObject.Find("WeaponCircle").GetComponent<WeaponBar>();
        if (healthCircle == null)
            Debug.LogError("Can not find the health bar.");
        if (weaponCircle == null)
            Debug.LogError("Can not find the weapon bar.");

        // from Damageable
        base.Start();

        // initial properties
        _remainingLife = initialLife;
        loadWeapon(weapons.Bolt);
        healthCircle.update(maxHealth, maxHealth);
    }

    void Update()
    {
        // fire the weapon
        if (Input.GetButton("Fire1"))
            _currentWeapon.fire();

        // handle weapon switching
        handleWeaponSelect();
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
        healthCircle.update(_health, maxHealth);
    }

    public void applyHealing(int healing)
    {
        // healing
        _health += healing;
        if (_health > maxHealth)
            _health = maxHealth;

        // update UI
        healthCircle.update(_health, maxHealth);
    }

    public void addWeaponExp(int exp)
    {
        // add exp to current weapon
        bool isUpgraded = _currentWeapon.addExperience(exp);

        // update UI
        if (isUpgraded)
            weaponCircle.switchWeapon(_currentWeapon);
        else
            weaponCircle.update(_currentWeapon);
    }

    // add max health
    public void addArmor(int armor)
    {
        maxHealth += armor;
        applyHealing(armor);
    }

    public void addLife(int life)
    {
        _remainingLife += life;
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

    // handle the buttons to select weapon
    private void handleWeaponSelect()
    {
        if (Input.GetButtonDown("Weapon1"))
            loadWeapon(weapons.Bolt);

        else if (Input.GetButtonDown("Weapon2"))
            loadWeapon(weapons.Sphere);

        else if (Input.GetButtonDown("Weapon3"))
            loadWeapon(weapons.Bolt);
    }

    private void loadWeapon(PlayerWeapon weapon)
    {
        _currentWeapon = weapon;

        // update UI
        weaponCircle.switchWeapon(weapon);
    }

    protected override void destroy()
    {
        // inform the gameManager that the game is over
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
            Debug.LogError("Can't find the GameManager.");
        gameManager.gameover();

        // from Damageable
        base.destroy();
    }
}

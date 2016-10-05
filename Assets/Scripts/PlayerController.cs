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

    [Header("Player Properties")]
    public int initialLife;
    public float moveSpeed;
    public float tiltFactor;
    public float rateExpToHp;
    public float WeaponGlobalCooldown;
    public PlayerWeapons weapons;
    public GameObject objEngine;

    [Header("Respawn")]
    public float respawnDelay;
    public float immuneDuration;
    public float immuneBlinkInterval;
    public float proportionExpDrop;
    public float extendRangeExpDrop;

    [Header("Boundary")]
    public Limit boundaryX;
    public Limit boundaryZ;

    [Header("Extra Sfx")]
    public AudioClip Clip_Weapon_Switch;

    private HealthBar healthCircle;
    private WeaponBar weaponCircle;
    private PlayerWeapon _currentWeapon;
    private int _remainingLife;
    private bool _isImmune;

    protected override void Start()
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
        _isImmune = false;
        loadWeapon(weapons.Bolt);
        healthCircle.update(maxHealth, maxHealth);
        healthCircle.updateLife(_remainingLife);

        // immunity on respawn
        StartCoroutine(immuneOnRespawn(immuneDuration, immuneBlinkInterval));
    }

    void Update()
    {
        // fire the weapon
        if (Input.GetButton("Fire1") && health > 0)
            _currentWeapon.fire();

        // End of laser
        if (_currentWeapon == weapons.Laser)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                // End laser
                ((LaserWeapon)_currentWeapon).endFire();
            }
        }

        // handle weapon switching
        handleWeaponSelect();
    }

	protected override void FixedUpdate()
    {
        // Damageable
        base.FixedUpdate();

        // handle the movements
        move();
    }

    // add functions to update the UI of health
    public override void applyDamage(int damage)
    {
        if (!_isImmune)
        {
            // base function
            base.applyDamage(damage);

            // update UI of health
            healthCircle.update(health, maxHealth);
        }
    }

    public override void applyHealing(int healing)
    {
        if (healing > 0)
        {
            // healing
            health += healing;
            if (health > maxHealth)
                health = maxHealth;

            // update UI
            healthCircle.update(health, maxHealth);
        }
    }

    public void addWeaponExp(int exp)
    {
        // weapon is not max
        if (!_currentWeapon.isMaxLevel())
        {
            // add exp to current weapon
            bool isUpgraded = _currentWeapon.addExperience(exp);

            // update UI
            if (isUpgraded)
            {
                weaponCircle.switchWeapon(_currentWeapon);

                // Show popup text
                PopupTextManager popupManager = GameObject.FindWithTag("PopupTextManager").GetComponent<PopupTextManager>();
                if (popupManager)
                    popupManager.showMessage("LEVEL UP", transform.position);
            }
            else
                weaponCircle.update(_currentWeapon);
        }
        else
        {
            // exceeding exp would transfer to health
            applyHealing((int)(exp * rateExpToHp));
        }
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
        healthCircle.updateLife(_remainingLife);
    }

    // handle the movement of the plane
    private void move()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        float axisHorizontal = Input.GetAxis("Horizontal");
        float axisVertical = Input.GetAxis("Vertical");

        // update velocity
        Vector3 movement = new Vector3(axisHorizontal, 0.0f, axisVertical);
        if(movement.sqrMagnitude > 1.0f)
            movement.Normalize();
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
        {
            loadWeapon(weapons.Bolt);

            // For laser
            ((LaserWeapon)weapons.Laser).endFire();
        }

        else if (Input.GetButtonDown("Weapon2"))
        {
            loadWeapon(weapons.Sphere);

            // For laser
            ((LaserWeapon)weapons.Laser).endFire();
        }

        else if (Input.GetButtonDown("Weapon3"))
            loadWeapon(weapons.Laser);
    }

    private void loadWeapon(PlayerWeapon weapon)
    {
        // Global cooldown
        if(_currentWeapon && _currentWeapon != weapon)
            _currentWeapon.NextFire = Time.time + WeaponGlobalCooldown;

        // Change weapon
        _currentWeapon = weapon;

        // Sfx
        if (Clip_Weapon_Switch)
            Audio.PlaySfx(Clip_Weapon_Switch);

        // update UI
        weaponCircle.switchWeapon(weapon);
    }

    private IEnumerator respawn()
    {
        // life
        _remainingLife -= 1;
        healthCircle.updateLife(_remainingLife);

        // respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // set initial properties, and turn on renderer and collider
        applyHealing(maxHealth);
        healthCircle.update(health, maxHealth); // why update again?
        gameObject.transform.position = Vector3.zero;
        setVisible(true);
        GetComponent<Collider>().enabled = true;

        // immunity on respawn
        StartCoroutine(immuneOnRespawn(immuneDuration, immuneBlinkInterval));
    }

    private void die()
    {
        // explosion vfx
        Instantiate(vfxExplosion, transform.position, transform.rotation);

        // drop experience as penalty
        if (!_currentWeapon.isMaxLevel())
        {
            int expDrop = _currentWeapon.experience;
            _currentWeapon.experience = 0;
            expDrop = (int)(expDrop * proportionExpDrop);
            weaponCircle.update(_currentWeapon);

            GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            if (gameManager == null)
                Debug.LogError("Can't find the GameManager.");
            else
            {
                Vector3 pos = transform.position;
                float radius = GetComponent<Collider>().bounds.extents.x + extendRangeExpDrop;
                gameManager.dropExperience(pos, radius, expDrop);
            }

            // Show popup text for experience dropped
            PopupTextManager popupManager = GameObject.FindWithTag("PopupTextManager").GetComponent<PopupTextManager>();
            if (popupManager)
                popupManager.showMessage("-" + (proportionExpDrop * 100).ToString() + "% EXP", transform.position);
            else
                Debug.LogError("Can't find the PopupTextManager.");
        }

        // turn off renderer and collider
        setVisible(false);
        GetComponent<Collider>().enabled = false;
        transform.position = new Vector3(0.0f, 10.0f, 0.0f); // place outside
    }

    private IEnumerator immuneOnRespawn(float duration, float interval)
    {
        // set flag to ignore damage
        _isImmune = true;

        // blink
        for(float timeElapsed = 0.0f; timeElapsed < duration; timeElapsed += interval * 2)
        {
            setVisible(false);
            yield return new WaitForSeconds(interval);
            setVisible(true);
            yield return new WaitForSeconds(interval);
        }

        // set flag back to receive damage again
        _isImmune = false;
    }

    private void setVisible(bool isOn)
    {
        // renderer
        gameObject.GetComponent<Renderer>().enabled = isOn;

        // particle systems (engine)
        objEngine.SetActive(isOn);
    }

    public override void destroy()
    {
        // die
        die();

        // get game manager
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
            Debug.LogError("Can't find the GameManager.");

        // check remaining life
        if (_remainingLife > 0)
            StartCoroutine(respawn());
        else
        {
            // inform the gameManager that the game is over
            gameManager.StartCoroutine(gameManager.gameover());

            // Then, wait for game manager to destroy it
        }
    }
}

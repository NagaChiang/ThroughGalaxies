using UnityEngine;
using System.Collections;
using CnControls;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

    private GameManager GameMgr;
    private HealthBar healthCircle;
    private WeaponBar weaponCircle;
    private PlayerWeapon _currentWeapon;
    private Vector3 ControlOffset; // the offset from ship position to control point
    private int _remainingLife;
    private bool _isImmune;
    public bool HasAd { get; private set; } // Unity Ads

    protected override void Start()
    {
        //weapons.Bolt.level = 5;
        //weapons.Sphere.level = 5;
        //weapons.Laser.level = 5;

        // Get GameManager
        GameMgr = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

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
        HasAd = false;
        healthCircle.update(maxHealth, maxHealth);
        healthCircle.updateLife(_remainingLife);

        // Load weapon
        _currentWeapon = weapons.Bolt;
        weaponCircle.switchWeapon(_currentWeapon);

        // immunity on respawn
        StartCoroutine(immuneOnRespawn(immuneDuration, immuneBlinkInterval));
    }

    void Update()
    {
        // fire the weapon
        if (GameMgr.IsMobile)
        {
            // For mobiles
            _currentWeapon.fire();
        }
        else
        {
            // For Windows
            if (Input.GetButton("Fire1") && health > 0)
                _currentWeapon.fire();
        }

        // End of laser
        if (_currentWeapon == weapons.Laser)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                // End laser
                ((LaserWeapon)_currentWeapon).endFire();
            }
        }

        // handle the movements
        move();

        // handle weapon switching
        handleWeaponSelect();
    }

    public void DoRespawn()
    {
        StartCoroutine(respawn());
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

        // Get input
        float axisHorizontal = 0.0f;
        float axisVertical = 0.0f;
        if (GameMgr.IsMobile)
        {
            // For mobiles
            if (Input.touchCount > 0)
            {
                // Raycast to UI to check if it hits button
                Touch touch = Input.GetTouch(0);
                bool hasHitButton = false;

                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = touch.position;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                for (int i = 0; i < results.Count; i++)
                {
                    RaycastResult result = results[i];
                    if (result.gameObject.GetComponent<SimpleButton>() != null)
                    {
                        // Hit button
                        hasHitButton = true;
                        break;
                    }
                }

                // Hasn't hit button
                if (!hasHitButton)
                {
                    Vector3 posWorld = Camera.main.ScreenToWorldPoint(touch.position);
                    posWorld.y = 0.0f;
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Update offset
                        ControlOffset = posWorld - transform.position;
                    }
                    else
                    {
                        // Direction calculated by control point
                        Vector3 controlPoint = transform.position + ControlOffset;
                        axisHorizontal = posWorld.x - controlPoint.x;
                        axisVertical = posWorld.z - controlPoint.z;
                    }
                }
            }
        }
        else
        {
            // Windows
            axisHorizontal = Input.GetAxis("Horizontal");
            axisVertical = Input.GetAxis("Vertical");
        }

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
        if (Input.GetButtonDown("Weapon1")
            || CnInputManager.GetButtonDown("Weapon1"))
        {
            loadWeapon(weapons.Bolt);

            // For laser
            ((LaserWeapon)weapons.Laser).endFire();
        }

        else if (Input.GetButtonDown("Weapon2")
            || CnInputManager.GetButtonDown("Weapon2"))
        {
            loadWeapon(weapons.Sphere);

            // For laser
            ((LaserWeapon)weapons.Laser).endFire();
        }

        else if (Input.GetButtonDown("Weapon3")
            || CnInputManager.GetButtonDown("Weapon3"))
        {
            loadWeapon(weapons.Laser);
        }
    }

    private void loadWeapon(PlayerWeapon weapon)
    {
        // Global cooldown
        if(_currentWeapon && _currentWeapon != weapon)
            weapon.NextFire = Time.time + WeaponGlobalCooldown;

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

        // Sfx
        if (Clip_OnDestroy)
            Audio.PlaySfx(Clip_OnDestroy);

        // drop experience as penalty
        if (!_currentWeapon.isMaxLevel())
        {
            int expDrop = _currentWeapon.experience;
            _currentWeapon.experience = 0;
            expDrop = (int)(expDrop * proportionExpDrop);
            weaponCircle.update(_currentWeapon);

            if (GameMgr == null)
                Debug.LogError("Can't find the GameManager.");
            else
            {
                Vector3 pos = transform.position;
                float radius = GetComponent<Collider>().bounds.extents.x + extendRangeExpDrop;
                GameMgr.dropExperience(pos, radius, expDrop);
            }

            // Show popup text for experience dropped
            PopupTextManager popupManager = GameObject.FindWithTag("PopupTextManager").GetComponent<PopupTextManager>();
            if (popupManager)
                popupManager.showMessage("LOSE " + (proportionExpDrop * 100).ToString() + "% EXP", transform.position);
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
            if (gameManager.IsMobile && !HasAd
                && Advertisement.isInitialized && Advertisement.IsReady())
            {
                // Unity Ads
                HasAd = true;
                gameManager.PromptForExtraLife();
            }
            else
            {
                // inform the gameManager that the game is over
                gameManager.DoGameover();
            }

            // Then, wait for game manager to destroy it
        }
    }
}

using UnityEngine;
using System.Collections;

public abstract class Damageable : SfxBase {

    [Header("Damageable")]
    public int maxHealth;
    public int experience;
    public float healDropRate;
    public bool enabledDropSupply;
    public bool EnabledShakeOnDeath;
    public GameObject vfxExplosion;

    public int health { get; protected set; }

    protected float Difficulty;

    // Blink on low health
    private const float LOW_HEALTH_RATE = 0.25f;
    private float NextColorChangeTime;
    private bool IsNextColorRed;

    // blinking effect on hit
    private Shader _shaderNormal;

    protected override void Start ()
    {
        // Sfx base
        base.Start();

        // initial health
        health = maxHealth;

        // save the normal color
        Material material = GetComponentInChildren<Renderer>().material;
        _shaderNormal = material.shader;
    }

    protected virtual void FixedUpdate()
    {
        // Blink on low health
        if(health < LOW_HEALTH_RATE * maxHealth)
            blinkOnLowHealth();
        else
            GetComponentInChildren<Renderer>().material.color = Color.white;
    }

    // Raising the diffuculty
    public void SetDifficulty(float difficulty)
    {
        // Store the difficulty
        Difficulty = difficulty;

        // Change health and heal to full
        maxHealth = (int)(maxHealth * difficulty);
        health = maxHealth;

        // Reduce the heal drop rate
        healDropRate *= difficulty;
    }

    // taking damage
    public virtual void applyDamage(int damage)
    {
        // check the damage is positive
        if(damage < 0)
        {
            Debug.LogWarning("Invalid damage amount: negative value.");
            return;
        }

        // Avoid applying multiple lethal damage at the same time
        if (health > 0)
        {
            // reduce health
            health -= damage;

            // dead
            if (health <= 0)
            {
                health = 0;

                // destroy this gameObject
                destroy();
                return;
            }

            // blinking effect
            StartCoroutine(blinkOnHit());
        }
    }

    // healing
    public virtual void applyHealing(int healing)
    {
        // check the healing is positive
        if (healing < 0)
        {
            Debug.LogWarning("Invalid healing amount: negative value.");
            return;
        }

        // gain health
        health += healing;

        // health cap
        if (health > maxHealth)
            health = maxHealth;
    }

    // display blinking effects once on hit
    private IEnumerator blinkOnHit()
    {
        float blinkDuration = 0.05f;
        Shader shaderBlink = Shader.Find("FX/Flare");

        // change the material to blink color for a while
        Material material = GetComponentInChildren<Renderer>().material;
        material.shader = shaderBlink;
        yield return new WaitForSeconds(blinkDuration);
        material.shader = _shaderNormal;
    }

    // display blinking effects while health is low
    private void blinkOnLowHealth()
    {
        if (Time.time >= NextColorChangeTime)
        {
            float blinkInterval = 0.15f;
            Color blinkColor = new Color(1.0f, 0.5f, 0.5f);
            Color normalColor = Color.white;

            // change the material to blink color for a while
            Material material = GetComponentInChildren<Renderer>().material;
            if(IsNextColorRed)
            {
                // Red
                material.color = blinkColor;
            }
            else
            {
                // White
                material.color = normalColor;
            }

            // Next color
            IsNextColorRed = !IsNextColorRed;

            // Next color change time
            NextColorChangeTime = Time.time + blinkInterval;
        }
    }

    // things to do once the health drop below 0
    public override void destroy()
    {
        // sfx
        base.destroy();

        // explosion vfx
        Instantiate(vfxExplosion, transform.position, transform.rotation);

        // get game manager
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (gameManager == null)
            Debug.LogError("Can't find the GameManager.");
        else
        {
            // drop experience crystals
            Vector3 pos = transform.position;
            float radius = GetComponent<Collider>().bounds.extents.x;
            if (experience > 0)
                gameManager.dropExperience(pos, radius, experience);

            // drop healings
            if (Random.value <= healDropRate)
                gameManager.dropHealing(pos, radius);

            // drop supply
            if (enabledDropSupply)
                gameManager.dropRandomSupply(pos, radius);

            // Shake on death
            if(EnabledShakeOnDeath)
                gameManager.Camera.SetShake(0.5f, 0.2f, 1.0f);
        }

        // destroy this game object
        Destroy(gameObject);
    }
}

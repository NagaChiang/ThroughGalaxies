using UnityEngine;
using System.Collections;

public abstract class Damageable : MonoBehaviour {

    [Header("Damageable")]
    public GameObject vfxExplosion;
    public int maxHealth;
    public int experience;
    public float healDropRate;
    public bool enabledDropSupply;

    public int health { get; protected set; }

    // blinking effect on hit
    private Shader _shaderNormal;

    // low health blinking coroutine
    private Coroutine _coroutineLowHealthBlink;

    protected virtual void Start ()
    {
        // initial health
        health = maxHealth;

        // save the normal color
        Material material = GetComponentInChildren<Renderer>().material;
        _shaderNormal = material.shader;
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

        // low health
        float proportionHealth = (float)health / maxHealth;
        if (proportionHealth < 0.25f)
        {
            if (_coroutineLowHealthBlink == null)
            {
                // start blinking
                _coroutineLowHealthBlink = StartCoroutine(blinkOnLowHealth());
            }
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

        // remove low health blinking
        float proportionHealth = health / maxHealth;
        if (proportionHealth >= 0.25f)
        {
            // stop blinking
            stopBlinkOnLowHealth();
        }
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
    private IEnumerator blinkOnLowHealth()
    {
        float blinkDuration = 0.15f;
        float blinkInterval = 0.5f;
        Color blinkColor = new Color(1.0f, 0.5f, 0.5f);
        Color normalColor = Color.white;

        // change the material to blink color for a while
        Material material = GetComponentInChildren<Renderer>().material;
        while (true)
        {
            // change color
            material.color = blinkColor;
            yield return new WaitForSeconds(blinkDuration);

            // normal color
            material.color = normalColor;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    protected void stopBlinkOnLowHealth()
    {
        if(_coroutineLowHealthBlink != null)
            StopCoroutine(_coroutineLowHealthBlink);
        GetComponentInChildren<Renderer>().material.color = Color.white;
    }

    // things to do once the health drop below 0
    protected virtual void destroy()
    {
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
        }

        // destroy this game object
        Destroy(gameObject);
    }
}

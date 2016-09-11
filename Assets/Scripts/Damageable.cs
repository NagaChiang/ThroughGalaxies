using UnityEngine;
using System.Collections;

public abstract class Damageable : MonoBehaviour {

    public GameObject vfxExplosion;
    public float maxHealth;
    public int experience;
    public float healDropRate;

    protected float _health;

    // blinking effect on hit
    private Shader _shaderNormal;

    // low health blinking coroutine
    private Coroutine _coroutineLowHealthBlink;

    protected void Start ()
    {
        // initial health
        _health = maxHealth;

        // save the normal color
        Material material = GetComponentInChildren<Renderer>().material;
        _shaderNormal = material.shader;
    }

    // taking damage
    public virtual void applyDamage(float damage)
    {
        // check the damage is positive
        if(damage < 0)
        {
            Debug.LogWarning("Invalid damage amount: negative value.");
            return;
        }

        // reduce health
        _health -= damage;

        // dead
        if (_health <= 0)
        {
            _health = 0;

            // destroy this gameObject
            destroy();
            return;
        }

        // blinking effect
        StartCoroutine(blinkOnHit());

        // low health
        float proportionHealth = _health / maxHealth;
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
    public virtual void applyHealing(float healing)
    {
        // check the healing is positive
        if (healing < 0)
        {
            Debug.LogWarning("Invalid healing amount: negative value.");
            return;
        }

        // gain health
        _health += healing;

        // health cap
        if (_health > maxHealth)
            _health = maxHealth;

        // remove low health blinking
        float proportionHealth = _health / maxHealth;
        if (proportionHealth >= 0.25f)
        {
            if (_coroutineLowHealthBlink != null) // TODO: untested
            {
                // stop blinking
                StopCoroutine(_coroutineLowHealthBlink);
            }
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

    // things to do once the health drop below 0
    protected virtual void destroy()
    {
        // explosion vfx
        Instantiate(vfxExplosion, transform.position, transform.rotation);

        // drop experience crystals
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Vector3 pos = transform.position;
        float radius = GetComponent<Collider>().bounds.extents.x;

        if (gameManager == null)
            Debug.LogError("Can't find the GameManager.");
        else if (experience > 0 && gameManager)
            gameManager.dropExperience(pos, radius, experience);

        // drop healings
        if (Random.value <= healDropRate && gameManager)
            gameManager.dropHealing(pos, radius);

        // destroy this game object
        Destroy(gameObject);
    }
}

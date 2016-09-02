using UnityEngine;
using System.Collections;

public abstract class Damageable : MonoBehaviour {

    public GameObject vfxExplosion;
    public float maxHealth;
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
        Material material = GetComponent<Renderer>().material;
        _shaderNormal = material.shader;
    }

    // taking damage
    public virtual void applyDamage(float damage)
    {
        // reduce health
        _health -= damage;
        
        // blinking effect
        StartCoroutine(blinkOnHit());

        // low health
        float proportionHealth = _health / maxHealth;
        if (proportionHealth <= 0.25f)
        {
            if (_coroutineLowHealthBlink == null)
            {
                // start blinking
                _coroutineLowHealthBlink = StartCoroutine(blinkOnLowHealth());

                // TODO: stop coroutine after healing
            }
        }

        // dead
        if (_health <= 0)
        {
            // destroy this gameObject
            destroy();
        }
    }

    // display blinking effects once on hit
    private IEnumerator blinkOnHit()
    {
        float blinkDuration = 0.05f;
        Shader shaderBlink = Shader.Find("FX/Flare");

        // change the material to blink color for a while
        Material material = GetComponent<Renderer>().material;
        material.shader = shaderBlink;
        yield return new WaitForSeconds(blinkDuration);
        material.shader = _shaderNormal;
    }

    // display blinking effects while health is low
    private IEnumerator blinkOnLowHealth()
    {
        float blinkDuration = 0.15f;
        float blinkInterval = 1.0f;
        Color blinkColor = new Color(1.0f, 0.5f, 0.5f);
        Color normalColor = Color.white;

        // change the material to blink color for a while
        Material material = GetComponent<Renderer>().material;
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

    // things to do once the health below 0
    protected virtual void destroy()
    {
        // explosion vfx
        Instantiate(vfxExplosion, transform.position, transform.rotation);
        
        // destroy this game object
        Destroy(gameObject);
    }
}

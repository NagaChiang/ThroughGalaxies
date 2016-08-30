using UnityEngine;
using System.Collections;

public abstract class Damageable : MonoBehaviour {

    public GameObject vfxExplosion;
    public float maxHealth;
    protected float _health;

    // blinking effect on hit
    private Shader _shaderNormal;

    protected void Start ()
    {
        // initial health
        _health = maxHealth;

        // save the normal color
        Material material = GetComponent<Renderer>().material;
        _shaderNormal = material.shader;
    }

    // taking damage
    public void applyDamage(float damage)
    {
        // reduce health
        _health -= damage;

        // blinking effect
        StartCoroutine(blinkOnHit());

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

    // things to do once the health below 0
    protected virtual void destroy()
    {
        // explosion vfx
        Instantiate(vfxExplosion, transform.position, transform.rotation);
        
        // destroy this game object
        Destroy(gameObject);
    }
}

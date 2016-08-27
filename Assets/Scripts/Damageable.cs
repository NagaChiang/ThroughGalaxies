using UnityEngine;
using System.Collections;

public abstract class Damageable : MonoBehaviour {

    public float maxHealth;
    protected float _health;

	void Start ()
    {
        // initial health
        _health = maxHealth;
    }
	
    public abstract void applyDamage(float damage);
    protected abstract void destroy();
}

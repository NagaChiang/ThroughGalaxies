using UnityEngine;
using System.Collections;

public class AreaOfEffect : SfxBase
{
    public BulletSource bulletSource;
    public float duration;
    public float radius;
    public float damageInterval;
    public int damage;

    protected override void Start()
    {
        // Sfx
        base.Start();

        // apply area damage
        StartCoroutine(damageArea(transform.position, radius, damage, damageInterval, bulletSource));

        // destroy it after a while
        Destroy(gameObject, duration);
    }

    private IEnumerator damageArea(Vector3 center, float rad, int dmg, float interval, BulletSource source)
    {
        while (true)
        {
            // get all the colliders in range
            ArrayList damagedObjIDs = new ArrayList();
            Collider[] hitColliders = Physics.OverlapSphere(center, rad);
            foreach (Collider collider in hitColliders)
            {
                // check null
                if (collider == null)
                    continue;

                // check this game obj has been damaged or not
                int objID = collider.gameObject.GetInstanceID();
                if (damagedObjIDs.Contains(objID))
                    continue;
                else
                    damagedObjIDs.Add(objID);

                // ignore boundary and other bullets 
                if (collider.tag == "Boundary"
                    || collider.tag == "Bullet"
                    || collider.tag == "Powerup")
                {
                    // do nothing
                    continue;
                }

                // ignore player (bullets from player)
                else if (collider.tag == "Player"
                            && bulletSource == BulletSource.player)
                {
                    // do nothing
                    continue;
                }

                // ignore enemies (bullets from enemies)
                else if (collider.tag == "Enemy"
                            && bulletSource == BulletSource.enemy)
                {
                    // do nothing
                    continue;
                }

                // hit anything else Damageable
                else
                {
                    // apply damage to it
                    Damageable target = collider.GetComponent<Damageable>();
                    if (target != null)
                        target.applyDamage(dmg);
                }
            }

            // damage interval
            if (interval <= 0)
                break;
            else
                yield return new WaitForSeconds(interval);
        }
    }
}

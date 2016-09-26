using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

    [Header("Laser")]
    public BulletSource BulletSource;
    public bool isPiercing;
    public float Width;
    public float DamagePerSecond;
    public float DamageInterval;
    public float StartDelay;

    [Header("VFX")]
    public LineRenderer LineLaser;
    public ParticleSystem LaserBurn;
    public LineRenderer LineBulletPath;

    private const float _RAY_LENGTH = 50.0f;
    private float StartTime;
    private float NextDamageTime;
    private LineRenderer BulletPath;

    void Start()
    {
        // Set time after delay
        StartTime = Time.time + StartDelay;
    }

    void Update()
    {
        // After delay
        if (Time.time >= StartTime)
        {
            // Destroy bullet path
            if (BulletPath)
                Destroy(BulletPath);

            // Update VFX of laser
            RaycastHit hitInfo = UpdateVFXPosition(Width, _RAY_LENGTH, LineLaser, LaserBurn);

            // Apply damage to targets
            Vector3 boxCenter;
            float laserLength;
            if (hitInfo.collider)
            {
                // Hit something
                boxCenter = (transform.position + hitInfo.point) / 2.0f;
                laserLength = hitInfo.distance;
            }
            else
            {
                // No hit, user max length instead
                boxCenter = transform.position + (_RAY_LENGTH / 2.0f) * transform.forward;
                laserLength = _RAY_LENGTH;
            }
            UpdateDamage(boxCenter, Width, laserLength);
        }

        // Not start firing
        else
        {
            // Update bullet path
            UpdateBulletPath(_RAY_LENGTH, LineBulletPath);
        }
    }

    private RaycastHit UpdateVFXPosition(float width, float rayMaxLength, LineRenderer lineLaser, ParticleSystem laserBurn)
    {
        // Follow parent
        transform.localPosition = Vector3.zero;

        // Set width and first point
        lineLaser.SetWidth(width, width);
        lineLaser.SetPosition(0, transform.position);

        // Raycast to set length
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, rayMaxLength)
            && !isPiercing)
        {
            // Update line renderer
            lineLaser.SetPosition(1, hitInfo.point);

            // Play particle system to show burning
            if (laserBurn)
            {
                laserBurn.transform.position = hitInfo.point + 5.0f * Vector3.up;
                if (laserBurn.isStopped)
                    laserBurn.Play();
            }
        }
        else
        {
            // Not hitting anything, use max length
            lineLaser.SetPosition(1, transform.position + rayMaxLength * transform.forward);

            // Stop particle system of burning
            if (laserBurn && laserBurn.isPlaying)
            {
                laserBurn.Stop();
                laserBurn.Clear();
            }
        }

        return hitInfo;
    }

    private void UpdateDamage(Vector3 boxCenter, float width, float totalLength)
    {
        // Damage
        if (Time.time >= NextDamageTime)
        {
            ArrayList damagedObjIDs = new ArrayList();
            Collider[] colliders = Physics.OverlapBox(boxCenter, new Vector3(width / 2.0f, 0.0f, totalLength / 2.0f), transform.rotation);
            foreach (Collider collider in colliders)
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
                            && BulletSource == BulletSource.player)
                {
                    // do nothing
                    continue;
                }

                // ignore enemies (bullets from enemies)
                else if (collider.tag == "Enemy"
                            && BulletSource == BulletSource.enemy)
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
                        target.applyDamage((int)(DamagePerSecond * DamageInterval));
                }
            }

            // Update next damage time
            NextDamageTime = Time.time + DamageInterval;
        }
    }

    private void UpdateBulletPath(float rayLength, LineRenderer linePath)
    {
        if (linePath)
        {
            if (!BulletPath)
            {
                // Draw bullet path and attach instance to bullet itself
                BulletPath = (LineRenderer)Instantiate(linePath,
                                                linePath.transform.position,
                                                linePath.transform.rotation);
                BulletPath.transform.SetParent(gameObject.transform);
            }
            else
            {
                // Follow parent
                transform.localPosition = Vector3.zero;

                // Update bullet path
                Ray ray = new Ray(transform.position, transform.forward);
                BulletPath.SetPosition(0, transform.position);
                BulletPath.SetPosition(1, ray.GetPoint(rayLength));
            }
        }
    }
}

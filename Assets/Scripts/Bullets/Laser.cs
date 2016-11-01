using UnityEngine;
using System.Collections;

public class Laser : SfxBase
{
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

    [Header("Extra Sfx")]
    public AudioClip Clip_Charge;
    public AudioClip Clip_Fire;
    public AudioClip Clip_Firing;
    public AudioClip Clip_Burn;

    private int AudioSourceFiringIndex;
    private int AudioSourceBurnIndex;
    private const int NOT_LOOPING_INDEX = -10;
    private bool HasFired;

    private const float _RAY_LENGTH = 30.0f;
    private float StartTime;
    private float NextDamageTime;
    private LineRenderer BulletPath;

    protected override void Start()
    {
        // Sfx
        base.Start();
        AudioSourceFiringIndex = NOT_LOOPING_INDEX;
        AudioSourceBurnIndex = NOT_LOOPING_INDEX;

        // Set time after delay
        StartTime = Time.time + StartDelay;

        // Sfx charge
        if (Clip_Charge)
            Audio.PlaySfx(Clip_Charge);
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

            // Sfx fire
            if(Clip_Fire && !HasFired)
            {
                Audio.PlaySfx(Clip_Fire);
                HasFired = true;
            }

            // Sfx firing
            if (Clip_Firing && AudioSourceFiringIndex == NOT_LOOPING_INDEX)
            {
                AudioSourceFiringIndex = Audio.PlaySfx(Clip_Firing, true);
            }

            // Apply damage to targets
            float laserLength;
            if (hitInfo.collider && !isPiercing)
            {
                // Hit something
                laserLength = hitInfo.distance;
            }
            else
            {
                // No hit, user max length instead
                laserLength = _RAY_LENGTH;
            }
            UpdateDamage(Width, laserLength);
        }

        // Not start firing
        else
        {
            // Update bullet path
            UpdateBulletPath(_RAY_LENGTH, LineBulletPath);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // End the sfx of burning
        if (Clip_Burn && AudioSourceBurnIndex != NOT_LOOPING_INDEX)
        {
            Audio.StopAudioSource(AudioSourceBurnIndex);
            AudioSourceBurnIndex = NOT_LOOPING_INDEX;
        }

        // End sfx of firing
        if (Clip_Burn && AudioSourceFiringIndex != NOT_LOOPING_INDEX)
        {
            Audio.StopAudioSource(AudioSourceFiringIndex);
            AudioSourceFiringIndex = NOT_LOOPING_INDEX;
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

    private void UpdateDamage(float width, float totalLength)
    {
        // Avoid exception
        if (width <= 0 || totalLength <= 0)
            return;

        // Damage
        if (Time.time >= NextDamageTime)
        {
            bool hasHit = false;
            ArrayList damagedObjIDs = new ArrayList();
            Vector3 boxCenter = transform.position;
            Vector3 halfExtents = new Vector3(width / 2.0f, 0.0f, 0.0f);
            RaycastHit[] hits = Physics.BoxCastAll(boxCenter, halfExtents, transform.forward, transform.rotation, totalLength);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider collider = hits[i].collider;

                // check null
                if (collider == null)
                    continue;

                // check this game obj has been damaged or not
                int objID = collider.gameObject.GetInstanceID();
                if (damagedObjIDs.Contains(objID))
                {
                    continue;
                }
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
                    {
                        hasHit = true;
                        target.applyDamage((int)(DamagePerSecond * DamageInterval));
                    }

                    // Sfx of burning
                    if(Clip_Burn && AudioSourceBurnIndex == NOT_LOOPING_INDEX)
                    {
                        AudioSourceBurnIndex = Audio.PlaySfx(Clip_Burn, true);
                    }
                }
            }

            // End the sfx of burning
            if(!hasHit && Clip_Burn && AudioSourceBurnIndex != NOT_LOOPING_INDEX)
            {
                Audio.StopAudioSource(AudioSourceBurnIndex);
                AudioSourceBurnIndex = NOT_LOOPING_INDEX;
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

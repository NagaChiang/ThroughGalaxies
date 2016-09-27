using UnityEngine;
using System.Collections;

public class BoltWeapon : PlayerWeapon {

    public GameObject bulletEnhanced;
    public int sideShotAngle_1;
    public int sideShotAngle_2;

    protected override IEnumerator doFire(float fireOffsetAngle = 0)
    {
        // side fire position
        updateSideFirePosition();

        // fire depending on current level
        switch (level)
        {
            case 1:
                // center only
                Instantiate(bullet, transform.position, transform.rotation);
                break;
            
            case 2:
                // center + 2 angle
                Instantiate(bullet, transform.position, transform.rotation);

                Instantiate(bullet, _posRightFire, Quaternion.AngleAxis(sideShotAngle_2, Vector3.up));
                Instantiate(bullet, _posLeftFire, Quaternion.AngleAxis(-sideShotAngle_2, Vector3.up));
                break;

            case 3:
                // 2 sides shots + 2 shots with angle
                Instantiate(bullet, _posRightFire, transform.rotation);
                Instantiate(bullet, _posLeftFire, transform.rotation);

                Instantiate(bullet, _posRightFire, Quaternion.AngleAxis(sideShotAngle_2, Vector3.up));
                Instantiate(bullet, _posLeftFire, Quaternion.AngleAxis(-sideShotAngle_2, Vector3.up));
                break;

            case 4:
                // 2 sides shots + 4 shots with angle
                Instantiate(bullet, _posRightFire, transform.rotation);
                Instantiate(bullet, _posLeftFire, transform.rotation);

                Instantiate(bullet, _posRightFire, Quaternion.AngleAxis(sideShotAngle_1, Vector3.up));
                Instantiate(bullet, _posLeftFire, Quaternion.AngleAxis(-sideShotAngle_1, Vector3.up));

                Instantiate(bullet, _posRightFire, Quaternion.AngleAxis(sideShotAngle_2, Vector3.up));
                Instantiate(bullet, _posLeftFire, Quaternion.AngleAxis(-sideShotAngle_2, Vector3.up));
                break;

            case 5:
                // center shot + 2 sides shots + 4 shots with angle (higher damage)
                Instantiate(bulletEnhanced, transform.position, transform.rotation);

                Instantiate(bulletEnhanced, _posRightFire, transform.rotation);
                Instantiate(bulletEnhanced, _posLeftFire, transform.rotation);

                Instantiate(bulletEnhanced, _posRightFire, Quaternion.AngleAxis(sideShotAngle_1, Vector3.up));
                Instantiate(bulletEnhanced, _posLeftFire, Quaternion.AngleAxis(-sideShotAngle_1, Vector3.up));

                Instantiate(bulletEnhanced, _posRightFire, Quaternion.AngleAxis(sideShotAngle_2, Vector3.up));
                Instantiate(bulletEnhanced, _posLeftFire, Quaternion.AngleAxis(-sideShotAngle_2, Vector3.up));
                break;

            default:
                Debug.LogWarning("Invalid weapon level: " + level.ToString());
                break;
        }

        yield break;
    }
}

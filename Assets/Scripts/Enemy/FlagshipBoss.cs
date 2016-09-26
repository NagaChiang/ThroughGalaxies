using UnityEngine;
using System.Collections;

// weapon 0: center bubble red
// weapon 1-3: cyan bolt
// weapon 4-7: sphere

public class FlagshipBoss : Boss {

    [Header("Flagship")]
    public float StartDelay;
    public float MovementSpeed;
    public float RotationSpeed;
    public Limit DelayAfterPoint;
    public Vector3[] WayPoints;

    [Header("Weapons")]
    public Weapon[] Lasers;

    public float NextFireTime { get; set; }

    private float NextMoveTime;
    private Vector3 NextPoint;

    protected override void Start()
    {
        base.Start();

        // First waypoint
        NextPoint = WayPoints[0];

        // Start delay
        NextFireTime = Time.time + StartDelay;
    }

    public void UpdatePosition()
    {
        if (Time.time >= NextMoveTime)
        {
            // Move towards next waypoint
            Vector3 direction = Vector3.Normalize(NextPoint - transform.position);
            transform.position += direction * MovementSpeed * Time.deltaTime;

            // Check reaching the waypoint or not
            const float THRESHOLD = 0.1f;
            float distSquare = Vector3.SqrMagnitude(NextPoint - transform.position);
            if (distSquare < THRESHOLD)
            {
                // Randomly choose the next waypoint
                ArrayList listPoints = new ArrayList(WayPoints);
                listPoints.Remove(NextPoint);
                Vector3 point = (Vector3)listPoints[Random.Range(0, listPoints.Count)];
                NextPoint = point;

                // Set delay
                NextMoveTime = Time.time + Random.Range(DelayAfterPoint.min, DelayAfterPoint.max);
            }
        }
    }

    public void FireFrontLasers()
    {
        // Postpone next move time to stay
        NextMoveTime = Time.time + Lasers[0].FireDuration;

        // Fire
        Lasers[0].fire();
        Lasers[1].fire();
        Lasers[10].fire();
    }

    public void FireAllLasers()
    {
        // Postpone next move time to stay
        NextMoveTime = Time.time + Lasers[0].FireDuration;

        // Fire
        foreach (Weapon weapon in Lasers)
        {
            weapon.fire();
        }
    }

    public void UpdateRotation(GameObject target)
    {
        // Check if target is alive
        Vector3 pos = target.transform.position;
        if (target.transform.position.y != 0)
        {
            pos = Vector3.zero;
        }

        // Rotate to look at target
        const float THRESHOLD = 0.0f;
        Vector3 direction = pos - transform.position;
        float step = RotationSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, direction, step, THRESHOLD);
        transform.rotation = Quaternion.LookRotation(newDir, transform.up);
    }

    protected override void InitializeFSM()
    {
        // States and transitions
        Flagship_HighHealthState highHealthState = new Flagship_HighHealthState();
        highHealthState.AddTransition(FSMSystem.Transition.MediumHealth, FSMSystem.StateID.Flagship_MediumHealthState);

        Flagship_MediumHealthState mediumHealthState = new Flagship_MediumHealthState();
        mediumHealthState.AddTransition(FSMSystem.Transition.LowHealth, FSMSystem.StateID.Flagship_LowHealthState);

        Flagship_LowHealthState lowHealthState = new Flagship_LowHealthState();

        // Add to FSM
        Fsm = new FSMSystem();
        Fsm.AddState(highHealthState);
        Fsm.AddState(mediumHealthState);
        Fsm.AddState(lowHealthState);
    }
}

public class Flagship_HighHealthState : FSMSystem.State
{
    public Flagship_HighHealthState()
    {
        ID = FSMSystem.StateID.Flagship_HighHealthState;
    }

    public override void Act(GameObject player, GameObject npc)
    {
        FlagshipBoss flagship = npc.GetComponent<FlagshipBoss>();
        if (flagship)
        {
            // Update transform
            flagship.UpdatePosition();

            // Use weapons
            if(Time.time >= flagship.NextFireTime)
            {
                // Lasers
                flagship.FireFrontLasers();

                // Bubble red
                flagship.weapons[0].fire();
                flagship.StartCoroutine(flagship.delayFire(flagship.weapons[0], 3.0f));
                flagship.StartCoroutine(flagship.delayFire(flagship.weapons[0], 6.0f));

                // Bolt
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[2], player, 7.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[3], player, 8.0f));

                // Sphere
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[4], player, 4.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[7], player, 4.25f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[5], player, 4.5f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[6], player, 4.75f));

                // Set next fire time
                float cooldown = 10.0f;
                flagship.NextFireTime = Time.time + cooldown;
            }
        }
        else
        {
            Debug.LogWarning("This state can only handle Flagship.");
        }
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        FlagshipBoss flagship = npc.GetComponent<FlagshipBoss>();
        if (flagship)
        {
            // Drop to medium health
            float hpProportion = (float)flagship.health / flagship.maxHealth;
            if (hpProportion < 0.6f)
                flagship.SetTransition(FSMSystem.Transition.MediumHealth);
        }
        else
        {
            Debug.LogWarning("This state can only handle Flagship.");
        }
    }
}

public class Flagship_MediumHealthState : FSMSystem.State
{
    public Flagship_MediumHealthState()
    {
        ID = FSMSystem.StateID.Flagship_MediumHealthState;
    }

    public override void Act(GameObject player, GameObject npc)
    {
        FlagshipBoss flagship = npc.GetComponent<FlagshipBoss>();
        if (flagship)
        {
            // Update transform
            flagship.UpdatePosition();

            // Use weapons
            if (Time.time >= flagship.NextFireTime)
            {
                // Lasers
                flagship.FireAllLasers();

                // Bubble red
                flagship.weapons[0].fire();
                flagship.StartCoroutine(flagship.delayFire(flagship.weapons[0], 3.0f));
                flagship.StartCoroutine(flagship.delayFire(flagship.weapons[0], 6.0f));

                // Bolt
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[1], player, 2.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[2], player, 6.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[3], player, 7.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[2], player, 8.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[3], player, 9.0f));

                // Sphere
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[4], player, 4.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[7], player, 4.25f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[5], player, 4.5f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[6], player, 4.75f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[4], player, 8.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[7], player, 8.25f));

                // Set next fire time
                float cooldown = 10.0f;
                flagship.NextFireTime = Time.time + cooldown;
            }
        }
        else
        {
            Debug.LogWarning("This state can only handle Flagship.");
        }
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        FlagshipBoss flagship = npc.GetComponent<FlagshipBoss>();
        if (flagship)
        {
            // Drop to low health
            float hpProportion = (float)flagship.health / flagship.maxHealth;
            if (hpProportion < 0.25f)
                flagship.SetTransition(FSMSystem.Transition.LowHealth);
        }
        else
        {
            Debug.LogWarning("This state can only handle Flagship.");
        }
    }
}

public class Flagship_LowHealthState : FSMSystem.State
{
    public Flagship_LowHealthState()
    {
        ID = FSMSystem.StateID.Flagship_LowHealthState;
    }

    public override void Act(GameObject player, GameObject npc)
    {
        FlagshipBoss flagship = npc.GetComponent<FlagshipBoss>();
        if (flagship)
        {
            // Update transform
            flagship.UpdatePosition();
            flagship.UpdateRotation(player);

            // Use weapons
            if (Time.time >= flagship.NextFireTime)
            {
                // Lasers
                flagship.FireAllLasers();

                // Bubble red
                flagship.weapons[0].fire();
                flagship.StartCoroutine(flagship.delayFire(flagship.weapons[0], 3.0f));
                flagship.StartCoroutine(flagship.delayFire(flagship.weapons[0], 6.0f));

                // Bolt
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[1], player, 2.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[2], player, 6.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[3], player, 7.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[2], player, 8.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[3], player, 9.0f));

                // Sphere
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[4], player, 4.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[7], player, 4.25f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[5], player, 4.5f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[6], player, 4.75f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[4], player, 8.0f));
                flagship.StartCoroutine(flagship.delayAimFire(flagship.weapons[7], player, 8.25f));

                // Set next fire time
                float cooldown = 10.0f;
                flagship.NextFireTime = Time.time + cooldown;
            }
        }
        else
        {
            Debug.LogWarning("This state can only handle Flagship.");
        }
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        // No transition
    }
}
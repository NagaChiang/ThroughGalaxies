using UnityEngine;
using System.Collections;

// weapon 0: center sphere enhanced
// weapon 1: center green bubble
// weapon 2, 3: side sphere (fan out)
// weapon 4, 5: side bolt

public class BattleshipBoss : Boss {

    [Header("Battleship")]
    public float ZMin;
    public float verticalSpeed;
    public float horizontalSpeed;
    public float horiBoostSpeed;

    public float NextMoveTime { get; set; }
    public float NextFireTime { get; set; }

    protected override void InitializeFSM()
    {
        // states and transitions
        Battleship_HighHealthState highHealthState = new Battleship_HighHealthState();
        highHealthState.AddTransition(FSMSystem.Transition.MediumHealth, FSMSystem.StateID.Battleship_MediumHealthState);

        Battleship_MediumHealthState mediumHealthState = new Battleship_MediumHealthState();
        mediumHealthState.AddTransition(FSMSystem.Transition.LowHealth, FSMSystem.StateID.Battleship_LowHealthState);

        Battleship_LowHealthState lowHealthState = new Battleship_LowHealthState();

        // add to FSM
        Fsm = new FSMSystem();
        Fsm.AddState(highHealthState);
        Fsm.AddState(mediumHealthState);
        Fsm.AddState(lowHealthState);
    }
}

public class Battleship_HighHealthState : FSMSystem.State
{
    private BattleshipBoss Battleship;

    public Battleship_HighHealthState()
    {
        ID = FSMSystem.StateID.Battleship_HighHealthState;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        // get battleship
        Battleship = npc.GetComponent<BattleshipBoss>();
        if (Battleship)
        {
            // drop to medium health
            float hpProportion = (float)Battleship.health / Battleship.maxHealth;
            if (hpProportion < 0.6f)
                Battleship.SetTransition(FSMSystem.Transition.MediumHealth);
        }
        else
        {
            Debug.LogWarning("This state can only handle Battleship.");
        }
    }

    public override void Act(GameObject player, GameObject npc)
    {
        // get battleship
        Battleship = npc.GetComponent<BattleshipBoss>();
        if (Battleship)
        {
            // Move to the position
            Vector3 pos = npc.transform.position;
            if (pos.z > Battleship.ZMin)
            {
                float speed = Battleship.verticalSpeed;
                npc.transform.position += speed * Time.deltaTime * npc.transform.forward;
            }

            // Randomly move right or left
            if (Time.time >= Battleship.NextMoveTime)
            {
                // Set next move time
                float duration = Random.Range(0.5f, 2.0f);
                Battleship.NextMoveTime = Time.time + duration;

                // X-axis speed
                Rigidbody rigidbody = npc.GetComponent<Rigidbody>();
                float sign = -Mathf.Sign(pos.x);
                rigidbody.velocity = new Vector3(sign * Battleship.horizontalSpeed, 0.0f, rigidbody.velocity.z);
            }

            // Use weapons
            if (Time.time >= Battleship.NextFireTime)
            {
                // Set cooldown
                float cooldown = 3.5f;
                Battleship.NextFireTime = Time.time + cooldown;

                // Center bubbles
                Battleship.weapons[1].fire();

                // Side bolts
                Battleship.StartCoroutine(Battleship.delayAimFire(Battleship.weapons[4], player, 1.0f));
                Battleship.StartCoroutine(Battleship.delayAimFire(Battleship.weapons[5], player, 1.0f));
            }
        }
        else
        {
            Debug.LogWarning("This state can only handle Battleship.");
        }
    }

    public override void DoOnLeaving()
    {
        // Cancel the horizontal speed
        Rigidbody rigidbody = Battleship.GetComponent<Rigidbody>();
        if (rigidbody)
            rigidbody.velocity = new Vector3(0.0f, 0.0f, rigidbody.velocity.z);

        // Short delay to wait for bubbles gone
        Battleship.NextFireTime = Time.time + 3.0f;
    }
}

public class Battleship_MediumHealthState : FSMSystem.State
{
    public Battleship_MediumHealthState()
    {
        ID = FSMSystem.StateID.Battleship_MediumHealthState;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        // get battleship
        BattleshipBoss battleship = npc.GetComponent<BattleshipBoss>();
        if (battleship)
        {
            // drop to low health
            float hpProportion = (float)battleship.health / battleship.maxHealth;
            if (hpProportion < 0.25f)
                battleship.SetTransition(FSMSystem.Transition.LowHealth);
        }
        else
        {
            Debug.LogWarning("This state can only handle Battleship.");
        }
    }

    public override void Act(GameObject player, GameObject npc)
    {
        // get battleship
        BattleshipBoss battleship = npc.GetComponent<BattleshipBoss>();
        if (battleship)
        {
            // Use weapons
            if (Time.time >= battleship.NextFireTime)
            {
                // Set cooldown
                float cooldown = 2.5f;
                battleship.NextFireTime = Time.time + cooldown;

                // Center sphere
                battleship.weapons[0].fire();

                // Side spheres
                battleship.StartCoroutine(battleship.delayFire(battleship.weapons[2], 0.75f));
                battleship.StartCoroutine(battleship.delayFire(battleship.weapons[3], 0.75f));

                // Side bolts
                battleship.StartCoroutine(battleship.delayAimFire(battleship.weapons[4], player, 1.5f));
                battleship.StartCoroutine(battleship.delayAimFire(battleship.weapons[5], player, 1.5f));
            }
        }
        else
        {
            Debug.LogWarning("This state can only handle Battleship.");
        }
    }
}

public class Battleship_LowHealthState : FSMSystem.State
{
    public Battleship_LowHealthState()
    {
        ID = FSMSystem.StateID.Battleship_LowHealthState;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        // no transition to other states
    }

    public override void Act(GameObject player, GameObject npc)
    {
        // get battleship
        BattleshipBoss battleship = npc.GetComponent<BattleshipBoss>();
        if (battleship)
        {
            // Use weapons
            if (Time.time >= battleship.NextFireTime)
            {
                // Set cooldown
                float cooldown = 3.5f;
                battleship.NextFireTime = Time.time + cooldown;

                // Center sphere
                battleship.weapons[0].fire();

                // Center bubbles
                battleship.weapons[1].fire();

                // Side bolts
                battleship.StartCoroutine(battleship.delayAimFire(battleship.weapons[4], player, 0.5f));
                battleship.StartCoroutine(battleship.delayAimFire(battleship.weapons[5], player, 1.0f));

                // Side spheres
                battleship.StartCoroutine(battleship.delayAimFire(battleship.weapons[2], player, 2.0f));
                battleship.StartCoroutine(battleship.delayAimFire(battleship.weapons[3], player, 2.5f));
            }
        }
        else
        {
            Debug.LogWarning("This state can only handle Battleship.");
        }
    }
}

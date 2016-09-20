using UnityEngine;
using System.Collections;

public class MothershipBoss : Boss {

    [Header("Mothership")]
    public Limit posZ;
    public float HorizontalSpeed;
    public float VerticalSpeed;
    public float VerticalAcc;

    private bool IsAccelerating;
    private float NextMoveTime;

    // forward/backward/dash; level 0, 1, 2
    public void move(int stateLevel)
    {
        // accelreation
        if(IsAccelerating)
        {
            GetComponent<Rigidbody>().velocity += transform.forward * VerticalAcc * Time.deltaTime;
            print(VerticalAcc * Time.deltaTime);
        }

        // make move decision
        if (Time.time >= NextMoveTime)
        {
            // get position and velocity
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            Vector3 pos = transform.position;
            Vector3 vel = rigidbody.velocity;

            // the end of backward
            if ((pos.z >= posZ.max) && (vel.z >= 0))
            {
                // not move horizontally yet
                if (vel.z != 0)
                {
                    float sign = -Mathf.Sign(pos.x);
                    rigidbody.velocity = new Vector3(HorizontalSpeed * sign, 0.0f, 0.0f);
                    
                    // Set next move time
                    float duration = Random.Range(1.0f, 2.0f);
                    NextMoveTime = Time.time + duration;
                }

                // already move horizontally
                else
                {
                    // begin to move forward
                    if (stateLevel == 0)
                        rigidbody.velocity = transform.forward * VerticalSpeed;
                    else if (stateLevel == 1)
                    {
                        float bonus = 5.0f;
                        rigidbody.velocity = transform.forward * VerticalSpeed * bonus;
                    }
                    else if (stateLevel == 2)
                    {
                        // ready to acceleration
                        rigidbody.velocity = Vector3.zero;
                        IsAccelerating = true;
                    }
                }
            }

            // the end of forward
            else if ((pos.z <= posZ.min) && (vel.z <= 0))
            {
                // not stop yet
                if (vel.z != 0)
                {
                    // stop the ship
                    IsAccelerating = false;
                    rigidbody.velocity = Vector3.zero;

                    // Set next move time
                    float duration = Random.Range(0.5f, 1.0f);
                    NextMoveTime = Time.time + duration;
                }

                // already stopped
                else
                {
                    // begin to move backward
                    rigidbody.velocity = transform.forward * -VerticalSpeed;
                }
            }
        }
    }

    protected override void InitializeFSM()
    {
        // states and transitions
        Mothership_HighHealthState highHealthState = new Mothership_HighHealthState();
        highHealthState.AddTransition(FSMSystem.Transition.MediumHealth, FSMSystem.StateID.Mothership_MediumHealthState);

        Mothership_MediumHealthState mediumHealthState = new Mothership_MediumHealthState();
        mediumHealthState.AddTransition(FSMSystem.Transition.LowHealth, FSMSystem.StateID.Mothership_LowHealthState);

        Mothership_LowHealthState lowHealthState = new Mothership_LowHealthState();

        // add to FSM
        Fsm = new FSMSystem();
        Fsm.AddState(highHealthState);
        Fsm.AddState(mediumHealthState);
        Fsm.AddState(lowHealthState);
    }
}

public class Mothership_HighHealthState : FSMSystem.State
{
    public Mothership_HighHealthState()
    {
        ID = FSMSystem.StateID.Mothership_HighHealthState;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        // get battleship
        MothershipBoss mothership = npc.GetComponent<MothershipBoss>();
        if (mothership)
        {
            // drop to medium health
            float hpProportion = (float)mothership.health / mothership.maxHealth;
            if (hpProportion < 0.6f)
                mothership.SetTransition(FSMSystem.Transition.MediumHealth);
        }
        else
        {
            Debug.LogWarning("This state can only handle Mothership.");
        }
    }

    public override void Act(GameObject player, GameObject npc)
    {
        // get mothership
        MothershipBoss mothership = npc.GetComponent<MothershipBoss>();
        if (mothership)
        {
            // control the move
            mothership.move(0);
        }
        else
        {
            Debug.LogWarning("This state can only handle Mothership.");
        }
    }
}

public class Mothership_MediumHealthState : FSMSystem.State
{
    public Mothership_MediumHealthState()
    {
        ID = FSMSystem.StateID.Mothership_MediumHealthState;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        // get battleship
        MothershipBoss mothership = npc.GetComponent<MothershipBoss>();
        if (mothership)
        {
            // drop to medium health
            float hpProportion = (float)mothership.health / mothership.maxHealth;
            if (hpProportion < 0.25f)
                mothership.SetTransition(FSMSystem.Transition.LowHealth);
        }
        else
        {
            Debug.LogWarning("This state can only handle Mothership.");
        }
    }

    public override void Act(GameObject player, GameObject npc)
    {
        // get mothership
        MothershipBoss mothership = npc.GetComponent<MothershipBoss>();
        if (mothership)
        {
            // control the move
            mothership.move(1);
        }
        else
        {
            Debug.LogWarning("This state can only handle Mothership.");
        }
    }
}

public class Mothership_LowHealthState : FSMSystem.State
{
    public Mothership_LowHealthState()
    {
        ID = FSMSystem.StateID.Mothership_LowHealthState;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        // no transition
    }

    public override void Act(GameObject player, GameObject npc)
    {
        // get mothership
        MothershipBoss mothership = npc.GetComponent<MothershipBoss>();
        if (mothership)
        {
            // control the move
            mothership.move(2);
        }
        else
        {
            Debug.LogWarning("This state can only handle Mothership.");
        }
    }
}

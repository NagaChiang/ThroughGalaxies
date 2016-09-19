using UnityEngine;
using System.Collections;

public class MothershipBoss : Boss {

	//[Header("Mothership")]

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
            if (hpProportion < 0.6f)
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

        }
        else
        {
            Debug.LogWarning("This state can only handle Mothership.");
        }
    }
}

using UnityEngine;
using System.Collections;

public class BattleshipBoss : WanderEnemy {

    private FSMSystem Fsm;
    private GameObject Player;

    protected override void Start()
    {
        // from Enemy and Damageable
        base.Start();

        // Initialize FSM
        InitializeFSM();

        // Get player game object
        Player = GameObject.FindWithTag("Player");
        if (Player == null)
            Debug.LogError("Can't find the game object of player.");
    }

    protected override void FixedUpdate()
    {
        // FSM
        Fsm.CurrentState.Reason(Player, GetComponent<BattleshipBoss>());
        Fsm.CurrentState.Act(Player, GetComponent<BattleshipBoss>());
    }

    public void SetTransition(FSMSystem.Transition trans)
    {
        // perform transition
        Fsm.PerformTransition(trans);
    }

    private void InitializeFSM()
    {
        Fsm = new FSMSystem();
    }
}

public class BattleshipLowHealthState : FSMSystem.State
{
    public override void Reason<BattleshipBoss>(GameObject player, BattleshipBoss npc)
    {
        // no transition to other states
    }

    public override void Act<BattleshipBoss>(GameObject player, BattleshipBoss npc)
    {

    }
}

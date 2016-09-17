using UnityEngine;
using System.Collections;

public abstract class Boss : Enemy {

    [Header("FSM")]
    protected FSMSystem Fsm;
    protected GameObject Player;

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
        // from Enemy and Damageable
        base.FixedUpdate();

        // FSM
        if (Player)
        {
            Fsm.CurrentState.Reason(Player, gameObject);
            Fsm.CurrentState.Act(Player, gameObject);
        }
    }

    public void SetTransition(FSMSystem.Transition trans)
    {
        // perform transition
        Fsm.PerformTransition(trans);
    }

    public IEnumerator delayFire(Weapon weapon, float time)
    {
        // delay
        yield return new WaitForSeconds(time);

        // fire
        weapon.fire();
    }

    public IEnumerator delayAimFire(Weapon weapon, Vector3 posTarget, float time)
    {
        // delay
        yield return new WaitForSeconds(time);

        // fire
        weapon.aimFire(posTarget);
    }

    protected abstract void InitializeFSM();
}

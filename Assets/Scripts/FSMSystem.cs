using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

http://wiki.unity3d.com/index.php?title=Finite_State_Machine

A Finite State Machine System based on Chapter 3.1 of Game Programming Gems 1 by Eric Dybsand

Written by Roberto Cezar Bianchini, July 2010
Revised by Naga Chiang, September 2016

*/

public class FSMSystem {

    public enum Transition
    {
        NullTransition = 0,
        LowHealth,
        MediumHealth,
    }

    public enum StateID
    {
        NullStateID = 0,
        Battleship_HighHealthState,
        Battleship_MediumHealthState,
        Battleship_LowHealthState,
        Mothership_HighHealthState,
        Mothership_MediumHealthState,
        Mothership_LowHealthState,
        Flagship_HighHealthState,
        Flagship_MediumHealthState,
        Flagship_LowHealthState,
    }

    public abstract class State
    {
        private Dictionary<Transition, StateID> Map = new Dictionary<Transition, StateID>();
        public StateID ID { get; protected set; }

        public void AddTransition(Transition trans, StateID id)
        {
            // Check if anyone of the args is invalid
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("FSMState ERROR: NullTransition is not allowed.");
                return;
            }

            if (id == StateID.NullStateID)
            {
                Debug.LogError("FSMState ERROR: NullStateID is not allowed.");
                return;
            }

            // Since this is a Deterministic FSM,
            // check if the current transition was already inside the map
            if (Map.ContainsKey(trans))
            {
                Debug.LogError("FSMState ERROR: State " + ID.ToString() + " already has transition " + trans.ToString() + ".");
                return;
            }

            Map.Add(trans, id);
        }

        public void DeleteTransition(Transition trans)
        {
            // Check for NullTransition
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("FSMState ERROR: NullTransition is not allowed.");
                return;
            }

            // Check if the pair is inside the map before deleting
            if (Map.ContainsKey(trans))
            {
                Map.Remove(trans);
            }
            else
            {
                // Not found
                Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + ID.ToString()
                               + " was not on the state's transition list.");
            }
        }

        public StateID GetOutputState(Transition trans)
        {
            // Check if the map has this transition
            if (Map.ContainsKey(trans))
            {
                return Map[trans];
            }
            else
            {
                // Not found
                Debug.LogError("FSMState ERROR: State " + ID.ToString()
                                + " doesn't contain Transition " + trans.ToString() + ".");
                return StateID.NullStateID;
            }
        }

        public virtual void DoOnEntering() { }
        public virtual void DoOnLeaving() { }
        public abstract void Reason(GameObject player, GameObject npc);
        public abstract void Act(GameObject player, GameObject npc);
    }

    public State CurrentState { get; private set; }
    private List<State> States = new List<State>();

    public void AddState(State s)
    {
        // Check for Null reference
        if (s == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed.");
            return;
        }

        // Check for Null state ID
        if(s.ID == StateID.NullStateID)
        {
            Debug.LogError("FSM ERROR: NullStateID is not allowed.");
            return;
        }

        // The first State inserted is also the Initial state.
        if (States.Count == 0)
        {
            States.Add(s);
            CurrentState = s;
            return;
        }

        // Add the state to the List if it's not inside it
        foreach (State state in States)
        {
            if (state.ID == s.ID)
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() +
                               " because it has already been added.");
                return;
            }
        }

        // It's a new state, add it
        States.Add(s);
    }

    public void DeleteState(StateID id)
    {
        // Check for NullState before deleting
        if (id == StateID.NullStateID)
        {
            Debug.LogError("FSM ERROR: NullStateID is not allowed.");
            return;
        }

        // Search the List and delete the state if it's inside it
        foreach (State state in States)
        {
            if (state.ID == id)
            {
                States.Remove(state);
                return;
            }
        }

        // Not found
        Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString()
                       + ". It was not on the list of states.");
    }

    public void PerformTransition(Transition trans)
    {
        // Check for NullTransition before changing the current state
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSM ERROR: NullTransition is not allowed.");
            return;
        }

        // Check if the target state exists
        StateID targetID = CurrentState.GetOutputState(trans);
        if (targetID == StateID.NullStateID)
        {
            Debug.LogError("FSM ERROR: State " + CurrentState.ID.ToString() + " does not have a target state" +
                           " for transition " + trans.ToString() + ".");
            return;
        }

        // Find target state and update the currentState		
        foreach (State state in States)
        {
            if (state.ID == targetID)
            {
                // Leave current state
                CurrentState.DoOnLeaving();

                // Enter target state
                CurrentState = state;
                CurrentState.DoOnEntering();
                break;
            }
        }

    }
}

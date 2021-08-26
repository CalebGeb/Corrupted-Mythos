using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrol State", menuName = "FSM/States/Patrol", order = 2)]
public class patrolState : State
{
    public override State RunCurrentState()
    {
        //Patrol state will walk around once player is within a certian range. Once they are close enough to be detected, and are reachable, the attack state will be entered.
        Debug.Log("I am in patrol state");
        return this;
    }
}

using UnityEngine;
public class PlayerHealState : PlayerGroundedState
{
    public PlayerHealState(Player _player,PlayerStateMachine _stateMachine,string _animBoolName) : base(_player,_stateMachine,_animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
        player.ZeroVelocity();
        if(triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
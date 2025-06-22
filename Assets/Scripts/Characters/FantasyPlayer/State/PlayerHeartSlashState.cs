using UnityEngine;

public class PlayerHeartSlashState : PlayerState
{
    public PlayerHeartSlashState(Player _player,PlayerStateMachine _stateMachine,string _animBoolName) : base(_player,_stateMachine,_animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
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
    public override void Exit()
    {
        base.Exit();
    }

}
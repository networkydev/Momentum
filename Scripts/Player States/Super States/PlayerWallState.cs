using UnityEngine;

namespace Momentum.States 
{
	public class PlayerWallState : PlayerBaseState
	{	
		public PlayerWallState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
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

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			if (_player.CanDoAction.WallJump(this)) { _stateMachine.ChangeState(_player.WallJumpState); return; }
			
			if (_playerData.ShouldHaveGrabStamina) { _player.WallGrabState.ReduceStamina(Time.deltaTime); } // Reduce the stamina when in wall state
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
		}
	}
}
using UnityEngine;

namespace Momentum.States
{
	public class PlayerIdleState : PlayerGroundedState
	{	
		public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			if (_playerData.PreventSlidingOnSlopesWhenIdle && _playerChecks.GroundedCheckCtx.normal != new Vector2(0, 1)) // Prevent Sliding and Ground is not flat
			{ // Set the friction
				_playerReferences.PBC.sharedMaterial.friction = _playerData.SlopeFriction;
				_playerReferences.PBC.enabled = false;
				_playerReferences.PBC.enabled = true;
			}
		}

		public override void Exit()
		{
			base.Exit();
			
			if (_playerData.PreventSlidingOnSlopesWhenIdle)
			{ // Reset friction
				_playerReferences.PBC.sharedMaterial.friction = _playerData.NormalFriction;
				_playerReferences.PBC.enabled = false;
				_playerReferences.PBC.enabled = true;
			}
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			if (!_isExitingState) 
			{
				if (_player.CanDoAction.Move(this)) { _stateMachine.ChangeState(_player.MoveState); return; }
				if (_player.CanDoAction.Crouch(this)) { _stateMachine.ChangeState(_player.CrouchIdleState); return; }
				if (_player.CanDoAction.Sprint(this)) { _stateMachine.ChangeState(_player.SprintState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
		}
	}
}
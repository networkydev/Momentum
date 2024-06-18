using UnityEngine;

namespace Momentum.States 
{
	public class PlayerFlyState : PlayerBaseState
	{
		#region Temporary Values
		
		private float _speedDif;
		private float _targetSpeed;
		private float _finalMovement;
		
		#endregion
		
		public PlayerFlyState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
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

			if (!_isExitingState) 
			{
				if (_player.CanDoAction.Fall(this)) { _stateMachine.ChangeState(_player.FallState); return; }
				if (_player.CanDoAction.Land(this)) { _stateMachine.ChangeState(_player.LandState); return; }
				if (_player.CanDoAction.LedgeHang(this)) { _stateMachine.ChangeState(_player.LedgeHangState); return; }
				if (_player.CanDoAction.Dash(this)) { _stateMachine.ChangeState(_player.DashState); return; }
				if (_player.CanDoAction.Jump(this)) { _stateMachine.ChangeState(_player.JumpState); return; }
				if (_player.CanDoAction.WallJump(this)) { _stateMachine.ChangeState(_player.WallJumpState); return; }
				if (_player.CanDoAction.WallGrab(this)) { _stateMachine.ChangeState(_player.WallGrabState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
			
			if (_playerData.CanMoveInAir) 
			{ // Let player move in the air
				_targetSpeed = InputManager.instance.WASDInput.x * _playerData.MoveSpeed;
				_speedDif = _targetSpeed - _playerReferences.PRB.velocity.x;
				_finalMovement = Mathf.Pow(Mathf.Abs(_speedDif) * _playerData.Acceleration, _playerData.VelocityPower) * Mathf.Sign(_speedDif);
				
				_playerReferences.PRB.velocity += (Time.deltaTime * _finalMovement * Vector2.right);
			}
		}
	}
}
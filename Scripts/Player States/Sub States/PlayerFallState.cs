using UnityEngine;

namespace Momentum.States 
{
	public class PlayerFallState : PlayerBaseState
	{
		#region Temporary Values
		
		private float _speedDif;
		private float _targetSpeed;
		private float _finalMovement;
		
		#endregion
		
		private float _originalGravity;
		
		public PlayerFallState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			_originalGravity = _playerReferences.PRB.gravityScale; // Cache orginal gravity
			_playerReferences.PRB.gravityScale = _playerData.FallGravityMultiplier; //  Multiply the fall gravity multiplier
		}

		public override void Exit()
		{
			base.Exit();
			
			_playerReferences.PRB.gravityScale = _originalGravity; // Reset gravity
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			if (!_isExitingState) 
			{
				if (_player.CanDoAction.Land(this)) { _stateMachine.ChangeState(_player.LandState); return;}
				if (_player.CanDoAction.Fly(this)) { _stateMachine.ChangeState(_player.FlyState); return; }
				if (_player.CanDoAction.LedgeHang(this)) { _stateMachine.ChangeState(_player.LedgeHangState); return; }
				if (_player.CanDoAction.Dash(this)) { _stateMachine.ChangeState(_player.DashState); return; }
				if (_player.CanDoAction.Jump(this)) { _stateMachine.ChangeState(_player.JumpState); return; }
				if (_player.CanDoAction.WallJump(this)) { _stateMachine.ChangeState(_player.WallJumpState); return; }
				if (_player.CanDoAction.WallGrab(this)) { _stateMachine.ChangeState(_player.WallGrabState); return; }
				if (_player.CanDoAction.WallSlide(this)) { _stateMachine.ChangeState(_player.WallSlideState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
			
			// Clamp fall speed
			_playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, Mathf.Clamp(_playerReferences.PRB.velocity.y, _playerData.MaxFallForce, float.MaxValue));
			
			if (_playerData.CanMoveInAir) 
			{ // Let player move in air
				_targetSpeed = InputManager.instance.WASDInput.x * _playerData.MoveSpeed;
				_speedDif = _targetSpeed - _playerReferences.PRB.velocity.x;
				_finalMovement = Mathf.Pow(Mathf.Abs(_speedDif) * _playerData.Acceleration, _playerData.VelocityPower) * Mathf.Sign(_speedDif);
				
				_playerReferences.PRB.velocity += (Time.deltaTime * _finalMovement) * Vector2.right;
			}
		}
	}
}
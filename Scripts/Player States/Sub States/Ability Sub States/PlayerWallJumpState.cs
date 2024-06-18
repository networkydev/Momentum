using UnityEngine;

namespace Momentum.States
{
	public class PlayerWallJumpState : PlayerAbilityState
	{
		public bool CanWallJump { get; private set; }
		
		private float _cooldown;
		private int _facingInteger;
		private float _movementControl;
		private float _timeInState;

		#region Temporary Values
		
		private float _speedDif;
		private float _targetSpeed;
		private float _finalMovement;
		
		#endregion
		
		public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			if (_playerChecks.IsTouchingFacingWall) // Flip sprite if we're facing the wall so it looks like we just jumped off the wall
			{
				_playerChecks.SetFacing(_playerChecks.IsFacing == PlayerChecks.Facing.Right ? PlayerChecks.Facing.Left : PlayerChecks.Facing.Right);
			}
			
			_timeInState = 0f;
		}

		public override void Exit()
		{
			base.Exit();
			
			CanWallJump = false;
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			_timeInState += Time.deltaTime;
			_movementControl = Mathf.Lerp(0, 1, _timeInState / _playerData.ReturnMovementControlTimer); // Progressively give more air control to the player
			
			if (_playerReferences.PRB.velocity.y < _playerData.WallJumpYThresh && _timeInState > _playerData.WallJumpTime && !_isAbilityDone) // If Y velocity is below threshold and we have wall jumped for the minimum amount of time
			{
				_cooldown = _playerData.WallJumpCooldown; // Set cooldown
				_isAbilityDone = true;
			}
			
			if (_player.CanDoAction.Dash(this)) { _stateMachine.ChangeState(_player.DashState); return; }
			
			if (_isAbilityDone && !_isExitingState) 
			{
				if (_player.CanDoAction.LedgeHang(this)) { _stateMachine.ChangeState(_player.LedgeHangState); return; }
				if (_player.CanDoAction.Jump(this)) { _stateMachine.ChangeState(_player.JumpState); return; }
				if (_player.CanDoAction.WallGrab(this)) { _stateMachine.ChangeState(_player.WallGrabState); return; }
				if (_player.CanDoAction.Fall(this)) { _stateMachine.ChangeState(_player.FallState); return; }
				if (_player.CanDoAction.Fly(this)) { _stateMachine.ChangeState(_player.FlyState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
			
			#region Movement
		
			_targetSpeed = InputManager.instance.WASDInput.x * (_playerData.WallJumpMoveSpeed);
			_speedDif = _targetSpeed - (_targetSpeed == 0 ? 0 : _playerReferences.PRB.velocity.x);
			_finalMovement = Mathf.Pow(Mathf.Abs(_speedDif) * _playerData.Acceleration, _playerData.VelocityPower) * Mathf.Sign(_speedDif);
			
			_playerReferences.PRB.velocity += (Time.deltaTime * (_finalMovement * _movementControl)) * Vector2.right;
			
			#endregion
			
			if (_timeInState < _playerData.WallJumpTime)
			{
				UseLinearWallJump();
			}
		}
		
		public void Start() 
		{ // Subscribe and set some variables
			_player.OnUpdateEvent += Update;
			
			CanWallJump = true;
		}
		
		public void Update() 
		{ // Decrease cooldown
			if (_cooldown > 0 && _stateMachine.CurrentState != _player.WallJumpState) { _cooldown -= Time.deltaTime; }
			else if (_cooldown <= 0) { CanWallJump = true; } // If cooldown done, set bool to reflect that
		}

		private void UseLinearWallJump()
		{
			_playerReferences.PRB.velocity = new Vector2(_playerData.LinearWallJumpForce.x * _facingInteger, _playerData.LinearWallJumpForce.y); // Add the force
		}
		
		public void IsFacing(PlayerChecks.Facing facing) 
		{ // Some info about where we're facing
			if (facing == PlayerChecks.Facing.Right) { _facingInteger = -1; } else { _facingInteger = 1;}
		}
	}
}
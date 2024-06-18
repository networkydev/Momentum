using UnityEngine;

namespace Momentum.States
{
	public class PlayerJumpState : PlayerAbilityState
	{
		public float CoyoteTimer { get; private set; }
		public int JumpsLeft { get; private set; }
		public float JumpBuffer { get; private set; }
		public float JumpCoolDown { get; private set; }
		
		private float _initVelocity;
		private float _timeInState;
		
		#region Temporary Values
		
		private float _xSpeed;
		private float _speedDif;
		private float _targetSpeed;
		private float _finalMovement;
		
		#endregion
		
		public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			JumpsLeft--;
			JumpBuffer = 0f; // Reset jump buffer so we don't auto jump after jump is over
			_timeInState = 0f;
			
			_xSpeed = InputManager.instance.IsSprint ? _playerData.SprintSpeed : _playerData.MoveSpeed; // Set the move speed based on if we were sprinting or not
			
			_initVelocity = (2 * _playerData.JumpHeight * _playerData.MoveSpeed) / (_playerData.JumpDistance / 2); // Calculate the initial velocity to add
			_playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, _initVelocity); // Add the inital jump velocity to player
		}

		public override void Exit()
		{
			base.Exit();
			
			JumpCoolDown = _playerData.JumpCoolDown; // Set the cooldown
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			_timeInState += Time.deltaTime;
			
			// Conditions to end jump
			if (_timeInState > _playerData.MinJumpTime && _playerReferences.PRB.velocity.y < _playerData.JumpYThresh) { EndJump(); } // Jumped for minimum required time and passed under Y threshold in velocity
			if (!InputManager.instance.JumpHeld && _timeInState > _playerData.MinJumpTime) { ReleasedJump(); } // Jump released and jumped for minimum required time
			
			if (_player.CanDoAction.Dash(this)) { _stateMachine.ChangeState(_player.DashState); return; } // Always be able to instantly transition to dash
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
			
			#region Movement
			
			// Movement code from Move State
			_targetSpeed = InputManager.instance.WASDInput.x * _xSpeed;
			_speedDif = _targetSpeed - _playerReferences.PRB.velocity.x;
			_finalMovement = Mathf.Pow(Mathf.Abs(_speedDif) * _playerData.Acceleration, _playerData.VelocityPower) * Mathf.Sign(_speedDif);
			
			_playerReferences.PRB.velocity += (Time.deltaTime * _finalMovement) * Vector2.right;
			
			#endregion
		}
		
		public void Start() 
		{
			// Subscribe to actions
			_player.OnUpdateEvent += Update;
			InputManager.instance.JumpPressed += JumpPressed;
			InputManager.instance.JumpReleased += ReleasedJump;
			PlayerGroundedState.Grounded += ResetJumpsLeft;
			
			// Get data from Player Data
			CoyoteTimer = _playerData.CoyoteTime;
			JumpsLeft = _playerData.AmountOfJumps;
		}
		
		public void Update() 
		{ // Update timers
			if (CoyoteTimer > 0) { CoyoteTimer -= Time.deltaTime; }
			if (JumpBuffer > 0) { JumpBuffer -= Time.deltaTime; }
			if (JumpCoolDown > 0) {JumpCoolDown -= Time.deltaTime; }
		}
		
		private void EndJump() // Called to end jump for reasons other than released jump, can be called without variable jump height enabled
		{
			if (_timeInState > _playerData.MinJumpTime) 
			{ // Jump Cut
				_isAbilityDone = true;
				_playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, _playerReferences.PRB.velocity.y > 0.2 ? 1f : _playerReferences.PRB.velocity.y);
                if (_player.CanDoAction.Fall(this)) { _stateMachine.ChangeState(_player.FallState); return; }
                if (_player.CanDoAction.Fly(this)) { _stateMachine.ChangeState(_player.FlyState); return; }
            }
		}
		
		public void ReleasedJump() // Called only when jump is released
		{
			if (_playerData.VariableJumpHeight && _stateMachine.CurrentState == _player.JumpState && _timeInState > _playerData.MinJumpTime) 
			{ // Jump Cut
				_isAbilityDone = true;
				_playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, _playerReferences.PRB.velocity.y > 0.5 ? _playerData.JumpCutVelocity : _playerReferences.PRB.velocity.y);
				if (_player.CanDoAction.Fall(this)) { _stateMachine.ChangeState(_player.FallState); return; }
				if (_player.CanDoAction.Fly(this)) {  _stateMachine.ChangeState(_player.FlyState); return; }
			}
		}
		
		public void JumpPressed() => JumpBuffer = _playerData.JumpBuffer;
		public void ResetJumpTimer() => JumpBuffer = 0;
		public void ResetCoyoteTimer() => CoyoteTimer = _playerData.CoyoteTime;
		public void ResetJumpsLeft() => JumpsLeft = _playerData.AmountOfJumps;
	}
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace Momentum.States
{
	public class PlayerDashState : PlayerAbilityState
	{
		private Vector2 _dashDirection;
		private Vector2 _dashToMouseDirection; // Temp variable pre initialized for the CalculateDash() method
		
		// Keep track of the different variables
		private float _dashRengenerationCooldown;
		private float _dashCooldown;
		private float _dashLength;
		private float _currentStrength;
		private float _dashHangTime;
		private int _dashesLeft;
		private float _dampYToZero = 0f; // Is used for the SmoothDamp function, not of use to us
		private bool _setYVelocityToZero;
		
		public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			_playerChecks.CanFlipPlayer = false;
			_dashDirection = CalculateDash();
			if (_dashDirection.y == 0) { _setYVelocityToZero = true; } else { _setYVelocityToZero = false; } // If doing a horizontal dash set y velocity to zero constantly to negatae gravity
			_dashLength = _playerData.DashLength;
			_dashHangTime = _playerData.DashHangTime;
		}

		public override void Exit()
		{
			base.Exit();
			
			_dashCooldown = _playerData.TimeBetweenDashes;
			_playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, -0.15f);
			_playerChecks.CanFlipPlayer = true;
			_dampYToZero = 0f;
		}
		
		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			if (_player.CanDoAction.WallJump(this)) { _stateMachine.ChangeState(_player.WallJumpState); return; }
			if (_player.CanDoAction.WallGrab(this)) { _stateMachine.ChangeState(_player.WallGrabState); return; }
			
			if (_isAbilityDone)
			{
				if (_dashHangTime > 0) // Do hang time then transition to next state
				{
					_dashHangTime -= Time.deltaTime;
					_playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, Mathf.SmoothDamp(_playerReferences.PRB.velocity.y, 0, ref _dampYToZero, _playerData.DashHangTime));
					if (_setYVelocityToZero) { _playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, 0); }
				} else if(!_isExitingState)
				{
					if (_player.CanDoAction.Crouch(this)) { _stateMachine.ChangeState(_player.CrouchIdleState); return; }
					if (_player.CanDoAction.Land(this)) { _stateMachine.ChangeState(_player.LandState); return; }
					if (_player.CanDoAction.Fly(this)) { _stateMachine.ChangeState(_player.FlyState); return; }
					if (_player.CanDoAction.Fall(this)) { _stateMachine.ChangeState(_player.FallState); return; }
				}
			} else // Set Dash Strength
			{
				_currentStrength = _playerData.DashStrength * Mathf.Clamp(_playerData.DashCurve.Evaluate(_playerData.DashLength - _dashLength), 0, 1);
				_currentStrength = Mathf.Clamp(_currentStrength, 0, _playerData.DashStrength);
			}

			if (_setYVelocityToZero) { _playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, 0); }
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
			
			if (_dashLength > 0) // Decrease Dash length until we can't dash anymore and dash
			{ 
				_dashLength -= Time.deltaTime;
				Dash(_dashDirection);

			} else if (!_isAbilityDone) // If Dash time is up
			{
				_isAbilityDone = true;
				_dashesLeft--;
			}

			if (_setYVelocityToZero) { _playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, 0); }
		}
		
		public void Start() // On Start
		{
			_player.OnUpdateEvent += Update; // Subscribe to Update() so we can run code as if we were a monobehaviour script
			
			_dashRengenerationCooldown = _playerData.DashRegenerationCooldown;
			_dashesLeft = _playerData.AmountOfDashes;
			_dashCooldown = _playerData.TimeBetweenDashes;
			_dashHangTime = _playerData.DashHangTime;
		}
		
		public void Update() 
		{
			if (_dashCooldown > 0) { _dashCooldown -= Time.deltaTime; }
			
			if (_dashesLeft < _playerData.AmountOfDashes && _dashRengenerationCooldown > 0) { _dashRengenerationCooldown -= Time.deltaTime; } // If there are dashes waiting to be regened and regen timer is more than 0, decrease it
			else if (_dashesLeft < _playerData.AmountOfDashes) // If regen timer is up, do below
			{ 
				_dashesLeft++;
				_dashRengenerationCooldown = _playerData.DashRegenerationCooldown;
			}
		}
		
		private Vector2 CalculateDash() 
		{
			if (!_playerData.DashTowardMouse)  // If Dashing towards WASD input
			{
				if (InputManager.instance.WASDInput != Vector2.zero) { return InputManager.instance.WASDInput.normalized; }
				else {
					if (_playerChecks.IsFacing == PlayerChecks.Facing.Right) { return new Vector2(1, 0); }
					else { return new Vector2(-1, 0); }
				}
			} else if (_playerData.DashTowardMouse) // If Dashing towards mouse
			{
				_dashToMouseDirection = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - _player.transform.position;
				_dashToMouseDirection = _dashToMouseDirection.normalized;
				
				return _dashToMouseDirection;
			}
			
			return Vector2.zero;
		}
		
		private void Dash(Vector2 DashDirection) 
		{
			if (DashDirection.x > 0) { _playerChecks.IsFacing = PlayerChecks.Facing.Right; } // Change the direction sprite is facing based on dash direction
			else { _playerChecks.IsFacing = PlayerChecks.Facing.Left; }
			
			_playerReferences.PRB.velocity = DashDirection * _currentStrength; // Extend the normalized dash direction vector by multiplying the desired length (strength)
		}
		
		public bool CanDash() 
		{
			if (_dashesLeft > 0 && _dashCooldown <= 0) { return true; } // If we have dashes left and no cooldown left 
			else { return false; }
		}
		
		public bool ShouldCrouch() {
			RaycastHit2D box; // Temp var
			
			if (_playerData.FixCantUncrouchWhenTouchingWall) // Run some diff code if we need this fix
			{
				box = Physics2D.BoxCast(new Vector2(_player.transform.position.x + _playerData.CrouchBoxColliderOffset.x, _player.transform.position.y + _playerData.StandBoxColliderOffset.y), new Vector2(_playerData.CrouchBoxColliderSize.x, _playerData.StandBoxColliderSize.y) - _playerData.UncrouchCheckShrink, 0f, Vector2.zero, 0, _playerData.CantExistIn);
			} else
			{
				box = Physics2D.BoxCast(new Vector2(_player.transform.position.x, _player.transform.position.y) + _playerData.StandBoxColliderOffset, _playerData.StandBoxColliderSize - _playerData.UncrouchCheckShrink, 0f, Vector2.zero, 0, _playerData.CantExistIn);
			}
			
			if (box.collider == null) { return false; } else { return true; } // If nothing is hit in the boxcast we aren't required to crouch, else we need to crouch
		}
	}
}
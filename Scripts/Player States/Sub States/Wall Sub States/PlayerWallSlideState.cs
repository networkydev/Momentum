using UnityEngine;

namespace Momentum.States 
{
	public class PlayerWallSlideState : PlayerWallState
	{
		private RaycastHit2D _checkDistanceToWall;
		private float _smoothDampVelTemp;
		private float _originalGravity;
		private float _timeInState;
		
		public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			_playerChecks.CanFlipPlayer = false;
			_originalGravity = _playerReferences.PRB.gravityScale; // Cache original gravity
			_playerReferences.PRB.gravityScale = 0; // Set gravity to 0
			_timeInState = 0f;

            // Set box collider properties
            _playerReferences.PBC.size = _playerData.WallBoxColliderSize;
			
			if (_playerChecks.IsFacing == PlayerChecks.Facing.Right) 
			{
				_playerReferences.PBC.offset = _playerData.WallBoxColliderOffset;

				// Check the distance from the player and the wall on X axis
				_checkDistanceToWall = Physics2D.Raycast(new Vector2(_playerReferences.PBC.bounds.center.x + _playerReferences.PBC.bounds.extents.x, _playerReferences.PBC.bounds.center.y - _playerReferences.PBC.bounds.extents.y + ((_playerData.WallCheck2Height / 100) * (_playerReferences.PBC.bounds.extents.y * 2))), Vector2.right, 5, _playerData.WallLayerMask);
                // Move the player to be near the wall based on the distance of the raycast above
                _player.transform.position += new Vector3(_checkDistanceToWall.distance, 0);

            }
            else 
			{
				_playerReferences.PBC.offset = _playerData.WallBoxColliderOffset * new Vector2(-1, 1);

				// Check the distance from the player and the wall on X axis
				_checkDistanceToWall = Physics2D.Raycast(new Vector2(_playerReferences.PBC.bounds.center.x - _playerReferences.PBC.bounds.extents.x, _playerReferences.PBC.bounds.center.y - _playerReferences.PBC.bounds.extents.y + ((_playerData.WallCheck2Height / 100) * (_playerReferences.PBC.bounds.extents.y * 2))), Vector2.left, 5, _playerData.WallLayerMask);
				// Move the player to be near the wall based on the distance of the raycast above
				_player.transform.position -= new Vector3(_checkDistanceToWall.distance, 0);
			}
		}

		public override void Exit()
		{
			base.Exit();
						
			// Reset box collider properties
			if (_playerData.WallBoxColliderSize.x > _playerData.StandBoxColliderSize.x) 
			{
				if (_playerChecks.IsFacing == PlayerChecks.Facing.Right) 
				{
					_player.transform.position += new Vector3((_playerData.WallBoxColliderSize.x - _playerData.StandBoxColliderSize.x) * 0.5f, 0, 0);
				} else 
				{
					_player.transform.position -= new Vector3((_playerData.WallBoxColliderSize.x - _playerData.StandBoxColliderSize.x) * 0.5f, 0, 0);
				}
			}
			
			_playerReferences.PBC.size = _playerData.StandBoxColliderSize;
			_playerReferences.PBC.offset = _playerData.StandBoxColliderOffset;
			_playerReferences.PRB.gravityScale = _originalGravity; // Reset gravity
			_playerChecks.CanFlipPlayer = true;
			
			_smoothDampVelTemp = 0f;
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			_timeInState += Time.deltaTime;

            // Smooth damp the Y velocity so if we were falling the velocity would gradually be reduced to wall slide speed making it natural
            _playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, Mathf.SmoothDamp(_playerReferences.PRB.velocity.y, -_playerData.SlideSpeed, ref _smoothDampVelTemp, _playerData.SlideSpeedSmoothTime));
			
			if (_player.CanDoAction.Dash(this)) { _stateMachine.ChangeState(_player.DashState); return; }
			
			if (!_isExitingState && _timeInState > 0.05f) // Dont exit immediately beacuse if wall box collider size.x is greater than the stand box collider.x player will spasm when trying to switch to slide state
			{
				if (_player.CanDoAction.Fall(this)) { _stateMachine.ChangeState(_player.FallState); return; }
				if (_player.CanDoAction.WallClimb(this)) { _stateMachine.ChangeState(_player.WallClimbState); return; }
				if (_player.CanDoAction.Land(this)) { _stateMachine.ChangeState(_player.LandState); return; }
				if (_player.CanDoAction.WallGrab(this)) { _stateMachine.ChangeState(_player.WallGrabState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
		}
	}
}
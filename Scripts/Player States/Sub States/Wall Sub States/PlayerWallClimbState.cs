using UnityEngine;

namespace Momentum.States 
{
	public class PlayerWallClimbState : PlayerWallState
	{
		private RaycastHit2D _checkDistanceToWall;
		private float _originalGravity;
		
		public PlayerWallClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}
		
		public override void Enter()
		{
			base.Enter();
			
			// Set the move speed upwards
			_playerReferences.PRB.velocity = new Vector2(_playerReferences.PRB.velocity.x, _playerData.WallClimbSpeed * InputManager.instance.WASDInput.y);
			
			_playerChecks.CanFlipPlayer = false;
			_originalGravity = _playerReferences.PRB.gravityScale; // Cache original gravity
			_playerReferences.PRB.gravityScale = 0; // Disable gravity so player keeps moving upwards
			
			// Set box collider properties
			_playerReferences.PBC.size = _playerData.WallBoxColliderSize;
			
			if (_playerChecks.IsFacing == PlayerChecks.Facing.Right) 
			{
				_playerReferences.PBC.offset = _playerData.WallBoxColliderOffset;

				// Check the distance from the player and the wall on X axis
				_checkDistanceToWall = Physics2D.Raycast(new Vector2(_playerReferences.PBC.bounds.center.x + _playerReferences.PBC.bounds.extents.x, _playerReferences.PBC.bounds.center.y - _playerReferences.PBC.bounds.extents.y + ((_playerData.WallCheck2Height / 100) * (_playerReferences.PBC.bounds.extents.y * 2))), Vector2.right, 5, _playerData.WallLayerMask);
				// Move the player to be near the wall based on the distance of the raycast above
				_player.transform.position += new Vector3(_checkDistanceToWall.distance, 0);
			} else 
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
			_playerReferences.PBC.size = _playerData.StandBoxColliderSize;
			_playerReferences.PBC.offset = _playerData.StandBoxColliderOffset;
			
			_playerReferences.PRB.gravityScale = _originalGravity; // Reset gravity
			_playerChecks.CanFlipPlayer = true;
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();

            if (!_isExitingState) 
			{
				if (_player.CanDoAction.WallGrab(this)) { _stateMachine.ChangeState(_player.WallGrabState); return; }
				if (_player.CanDoAction.WallSlide(this)) { _stateMachine.ChangeState(_player.WallSlideState); return; }
				if (_player.CanDoAction.LedgeHang(this)) { _stateMachine.ChangeState(_player.LedgeHangState); return; }
				if (_player.CanDoAction.Fall(this)) { _stateMachine.ChangeState(_player.FallState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
		}
	}
}
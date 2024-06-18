using System.Collections;
using UnityEngine;

namespace Momentum.States
{
	public class PlayerWallGrabState : PlayerWallState
	{
		public Vector3 PlayerPOS { get; private set; }
		public Vector2 BCExtents { get; private set; }
		public RaycastHit2D WallRaycast { get; private set; }
		public float Stamina { get; private set; }
		public PlayerChecks.Facing FacingCache { get; private set; }

        private RaycastHit2D _grabBoxCast;
		private RaycastHit2D _checkDistanceToWall;
		private float _originalGravity;
		
		public PlayerWallGrabState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			_playerChecks.CanFlipPlayer = false;
			_originalGravity = _playerReferences.PRB.gravityScale; // Cache original gravity
			_playerReferences.PRB.gravityScale = 0; // Set gravity to 0
			_playerReferences.PRB.velocity = Vector2.zero;
			
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
			
			// Reset box collider
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
				if (_player.CanDoAction.WallSlide(this)) { _stateMachine.ChangeState(_player.WallSlideState); return; }
				if (_player.CanDoAction.WallClimb(this)) { _stateMachine.ChangeState(_player.WallClimbState); return; }
				if (_player.CanDoAction.Land(this)) { _stateMachine.ChangeState(_player.LandState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
		}	
		
		public bool CanWallGrab() 
		{
			PlayerPOS = _player.transform.position; // Cache Box Collider Position
			WallRaycast = _playerChecks.WallCheckCtx2; // Cache raycast from player checks
			FacingCache = _playerChecks.IsFacing;
			
			if (_playerChecks.IsFacing == PlayerChecks.Facing.Right) 
			{
				_grabBoxCast = Physics2D.BoxCast(new Vector2(PlayerPOS.x + _playerData.WallBoxColliderOffset.x + (_playerData.WallBoxColliderSize.x * 0.5f) + (WallRaycast.distance * 0.5f - 0.01f), PlayerPOS.y + _playerReferences.PData.WallBoxColliderOffset.y), new Vector2(WallRaycast.distance - 0.03f, _playerReferences.PData.WallBoxColliderSize.y) - _playerData.WallGrabCheckShrink, 0f, Vector2.right, 0, _playerData.CantExistIn);
			} else 
			{
				_grabBoxCast = Physics2D.BoxCast(new Vector2(PlayerPOS.x - _playerData.WallBoxColliderOffset.x - (_playerData.WallBoxColliderSize.x * 0.5f) - (WallRaycast.distance * 0.5f - 0.01f), PlayerPOS.y + _playerReferences.PData.WallBoxColliderOffset.y), new Vector2(WallRaycast.distance, _playerReferences.PData.WallBoxColliderSize.y) - _playerData.WallGrabCheckShrink, 0f, Vector2.left, 0, _playerData.CantExistIn);
			}
			
			if (_grabBoxCast.collider == null) { return true; } else { return false; }
		}
		
		public void Start() 
		{
			if (_playerData.ShouldHaveGrabStamina) { PlayerGroundedState.Grounded += ResetStamina; } // Subscribe
			Stamina = _playerData.GrabStamina; // Get variables from player data
		}
		
		public void ResetStamina() { if (_playerData.ShouldHaveGrabStamina) { Stamina = _playerData.GrabStamina; } }
		public void ReduceStamina(float time) { if (_playerData.ShouldHaveGrabStamina) { Stamina -= time; } }
	}
}
using UnityEngine;

namespace Momentum.States 
{
	public class PlayerLedgeClimbState : PlayerBaseState
	{
		public Vector2 LedgeClimbFinishPosition { get; private set; }
		public RaycastHit2D LedgeClimbCheck { get; private set; }
		
		private RaycastHit2D _ledgeHitCollider;
		private Vector2 _ledgeClimbCornerPosition;
		
		public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}
		
		public override void Enter()
		{
			base.Enter();
			
			_playerChecks.CanFlipPlayer = false;
			_playerReferences.PRB.bodyType = RigidbodyType2D.Kinematic;
			_playerReferences.PRB.velocity = Vector2.zero;
			_player.JumpState.ResetJumpTimer(); // If we pressed Jump to climb make sure we don't automatically jump after we complete ledge climb
			
			// Set box collider properties
			_playerReferences.PBC.size = _playerData.LedgeBoxColliderSize;
			
			if (_playerChecks.IsFacing == PlayerChecks.Facing.Left) 
			{ // Make sure the box collider offset x is negative when facing left
				_playerReferences.PBC.offset = _playerData.LedgeBoxColliderOffset * new Vector2(-1, 1);
				_player.transform.position = _ledgeClimbCornerPosition + (_playerData.LedgeClimbOffset * new Vector2(-1, 1));
			} else 
			{ // Else normal
				_playerReferences.PBC.offset = _playerData.LedgeBoxColliderOffset;
				_player.transform.position = _ledgeClimbCornerPosition + _playerData.LedgeClimbOffset;
			}
		}

		public override void Exit()
		{
			base.Exit();
			
			// Reset box collider properties
			_playerReferences.PBC.size = _playerData.StandBoxColliderSize;
			_playerReferences.PBC.offset = _playerData.StandBoxColliderOffset; 
			
			// Set body type to dynamic so we can move again
			_playerReferences.PRB.bodyType = RigidbodyType2D.Dynamic;
			_playerChecks.CanFlipPlayer = true;
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
		}
		
		public void Climb() 
		{	
			_player.transform.position = LedgeClimbFinishPosition - _playerData.StandBoxColliderOffset; // Finish the climb by setting to final position. Called by an animation event
			_stateMachine.ChangeState(_player.LandState);
		}
		
		public bool IsValidPosition() // Called before entering state
		{
			_ledgeHitCollider = _playerChecks.LedgeCheckCtx2; // Cache the hit collider information
			
			if (_ledgeHitCollider.collider == null) { return false;  }
			
			if (_playerChecks.IsFacing == PlayerChecks.Facing.Right) // Calculate the ledge climb finishing position and check if the spot is valid
			{
				_ledgeClimbCornerPosition = new Vector2(_ledgeHitCollider.collider.bounds.center.x - _ledgeHitCollider.collider.bounds.extents.x, _ledgeHitCollider.collider.bounds.center.y + _ledgeHitCollider.collider.bounds.extents.y);
				LedgeClimbFinishPosition = new Vector2(_ledgeClimbCornerPosition.x + (_playerData.StandBoxColliderSize.x * 0.5f), _ledgeClimbCornerPosition.y + (_playerData.StandBoxColliderSize.y * 0.5f));
				LedgeClimbCheck = Physics2D.BoxCast(LedgeClimbFinishPosition + new Vector2(_playerReferences.PData.LedgeBoxColliderOffset.x, _playerReferences.PData.LedgeBoxColliderOffset.y), new Vector2(_playerData.StandBoxColliderSize.x - _playerData.LedgeClimbCheckShrink.x, _playerData.StandBoxColliderSize.y - _playerData.LedgeClimbCheckShrink.y), 0, Vector2.zero, 0, _playerData.CantExistIn);
			} else
			{
				_ledgeClimbCornerPosition = new Vector2(_ledgeHitCollider.collider.bounds.center.x + _ledgeHitCollider.collider.bounds.extents.x, _ledgeHitCollider.collider.bounds.center.y + _ledgeHitCollider.collider.bounds.extents.y);
				LedgeClimbFinishPosition = new Vector2(_ledgeClimbCornerPosition.x - (_playerData.StandBoxColliderSize.x * 0.5f), _ledgeClimbCornerPosition.y + (_playerData.StandBoxColliderSize.y * 0.5f));
				LedgeClimbCheck = Physics2D.BoxCast(LedgeClimbFinishPosition + new Vector2(-_playerReferences.PData.LedgeBoxColliderOffset.x, _playerReferences.PData.LedgeBoxColliderOffset.y), new Vector2(_playerData.StandBoxColliderSize.x - _playerData.LedgeClimbCheckShrink.x, _playerData.StandBoxColliderSize.y - _playerData.LedgeClimbCheckShrink.y), 0, Vector2.zero, 0, _playerData.CantExistIn);
			}
			
			if (LedgeClimbCheck.collider == null) { return true; } else { return false; }
		}
	}
}
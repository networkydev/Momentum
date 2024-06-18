using UnityEngine;

namespace Momentum.States 
{
	public class PlayerLedgeHangState : PlayerBaseState
	{
		public Vector3 HangPosition { get; private set; }
		public RaycastHit2D HangPositionCast { get; private set; }
		
		private RaycastHit2D _ledgeHitCollider;

		public PlayerLedgeHangState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();

			_playerChecks.CanFlipPlayer = false;
			_playerReferences.PRB.bodyType = RigidbodyType2D.Kinematic;
			_playerReferences.PRB.velocity = Vector2.zero;
			
			// Set box collider information
			_playerReferences.PBC.size = _playerData.LedgeBoxColliderSize;
			if (_playerChecks.IsFacing == PlayerChecks.Facing.Left) 
			{ // Make sure the box collider offset x is negative when facing left
				_playerReferences.PBC.offset = _playerData.LedgeBoxColliderOffset * new Vector2(-1, 1);
			} else 
			{ // Else normal
				_playerReferences.PBC.offset = _playerData.LedgeBoxColliderOffset;
			}
			
			_player.transform.position = HangPosition; // Set position to hang position
		}

		public override void Exit()
		{
			base.Exit();
			
			// Reset box collider information
			_playerReferences.PBC.size = _playerData.StandBoxColliderSize;
			_playerReferences.PBC.offset = _playerData.StandBoxColliderOffset;
			
			// Set body type to dynamic so we can move again
			_playerReferences.PRB.bodyType = RigidbodyType2D.Dynamic;
			_playerChecks.CanFlipPlayer = true;
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			if (!_isExitingState) 
			{
				if (_player.CanDoAction.LedgeClimb(this)) { _stateMachine.ChangeState(_player.LedgeClimbState); return; }
				if (_player.CanDoAction.WallSlide(this)) { _stateMachine.ChangeState(_player.WallSlideState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
		}
		
		public void Start() 
		{
			_player.OnUpdateEvent += Update;
		}
		
		public void Update() 
		{
			
		}

		public bool IsValidPosition() // Called before we enter this state
		{	
			_ledgeHitCollider = _playerChecks.LedgeCheckCtx2; // Cache the hit collider information
			
			if (_playerChecks.IsFacing == PlayerChecks.Facing.Right) // Calculate hang position
			{
				_playerChecks.DirectionOfLedgeHangCheck = true;
				HangPosition = new Vector3(_ledgeHitCollider.collider.bounds.center.x - _ledgeHitCollider.collider.bounds.extents.x + _playerData.LedgeHangOffset.x, _ledgeHitCollider.collider.bounds.center.y + _ledgeHitCollider.collider.bounds.extents.y + _playerData.LedgeHangOffset.y);
				HangPositionCast = Physics2D.BoxCast(HangPosition + new Vector3(_playerReferences.PData.LedgeBoxColliderOffset.x, _playerReferences.PData.LedgeBoxColliderOffset.y, 0), new Vector2(_playerData.LedgeBoxColliderSize.x - _playerData.LedgeHangCheckShrink.x, _playerData.LedgeBoxColliderSize.y - _playerData.LedgeHangCheckShrink.y), 0, Vector2.right, 0, _playerData.CantExistIn);
			}
            else
            {
				_playerChecks.DirectionOfLedgeHangCheck = false;
				HangPosition = new Vector3(_ledgeHitCollider.collider.bounds.center.x + _ledgeHitCollider.collider.bounds.extents.x - _playerData.LedgeHangOffset.x, _ledgeHitCollider.collider.bounds.center.y + _ledgeHitCollider.collider.bounds.extents.y + _playerData.LedgeHangOffset.y);
				HangPositionCast = Physics2D.BoxCast(HangPosition + new Vector3(-_playerReferences.PData.LedgeBoxColliderOffset.x, _playerReferences.PData.LedgeBoxColliderOffset.y, 0), new Vector2(_playerData.LedgeBoxColliderSize.x - _playerData.LedgeHangCheckShrink.x, _playerData.LedgeBoxColliderSize.y - _playerData.LedgeHangCheckShrink.y), 0, Vector2.right, 0, _playerData.CantExistIn);
			}
			
			
			if (HangPositionCast.collider == null) { return true; }
			else { return false; }
		}
	}
}
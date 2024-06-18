using UnityEngine;

namespace Momentum.States
{
	public class PlayerCrouchIdleState : PlayerGroundedState
	{
		#region Temporary Values
		
		private RaycastHit2D _uncrouchCheck;
		
		#endregion
		
		public PlayerCrouchIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			// Change our box collider shape and offset
			_playerReferences.PBC.size = _playerData.CrouchBoxColliderSize; 
			_playerReferences.PBC.offset = _playerData.CrouchBoxColliderOffset;
			
			if (_playerData.PreventSlidingOnSlopesWhenIdle && _playerChecks.GroundedCheckCtx.normal != new Vector2(0, 1)) // Should prevent sliding on slopes and the ground we are on is a slope
			{ // Make sure we don't slide
				_playerReferences.PBC.sharedMaterial.friction = _playerData.SlopeFriction;
				_playerReferences.PBC.enabled = false; // Turn the box collider on and off so the slope friction is recognized
				_playerReferences.PBC.enabled = true;
			}
		}

		public override void Exit()
		{
			base.Exit();
			
			// Reset the box collider properties
			_playerReferences.PBC.size = _playerData.StandBoxColliderSize;
			_playerReferences.PBC.offset = _playerData.StandBoxColliderOffset;
			
			if (_playerData.PreventSlidingOnSlopesWhenIdle)
			{ // Reset the friction
				_playerReferences.PBC.sharedMaterial.friction = _playerData.NormalFriction;
				_playerReferences.PBC.enabled = false;
				_playerReferences.PBC.enabled = true;
			}
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			if (!_isExitingState) 
			{
				if (_player.CanDoAction.UnCrouch(this)) { _stateMachine.ChangeState(_player.IdleState); return; }
				if (_player.CanDoAction.Move(this)) { _stateMachine.ChangeState(_player.CrouchMoveState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
		}
		
		public bool CanUncrouch() 
		{
			if (_playerData.FixCantUncrouchWhenTouchingWall) 
			{ // Different options based on if we want the fix or not
				_uncrouchCheck = Physics2D.BoxCast(new Vector2(_player.transform.position.x + _playerData.CrouchBoxColliderOffset.x, _player.transform.position.y + _playerData.StandBoxColliderOffset.y), new Vector2(_playerData.CrouchBoxColliderSize.x, _playerData.StandBoxColliderSize.y) - _playerData.UncrouchCheckShrink, 0f, Vector2.zero, 0, _playerData.CantExistIn);
				// _uncrouchCheck = Physics2D.CapsuleCast(new Vector2(_player.transform.position.x + _playerData.CrouchBoxColliderOffset.x, _player.transform.position.y + _playerData.StandBoxColliderOffset.y), new Vector2(_playerData.CrouchBoxColliderSize.x, _playerData.StandBoxColliderSize.y) - _playerData.UncrouchCheckShrink, CapsuleDirection2D.Vertical, 0f, Vector2.zero, 0, _playerData.CantExistIn);
			} else
			{
				_uncrouchCheck = Physics2D.BoxCast(new Vector2(_player.transform.position.x, _player.transform.position.y) + _playerData.StandBoxColliderOffset, _playerData.StandBoxColliderSize - _playerData.UncrouchCheckShrink, 0f, Vector2.zero, 0, _playerData.CantExistIn);
				// _uncrouchCheck = Physics2D.CapsuleCast(new Vector2(_player.transform.position.x, _player.transform.position.y) + _playerData.StandBoxColliderOffset, _playerData.StandBoxColliderSize - _playerData.UncrouchCheckShrink, CapsuleDirection2D.Vertical, 0f, Vector2.zero, 0, _playerData.CantExistIn);
			}
			
			if (_uncrouchCheck.collider != null) { return false; } else { return true; }
		}
	}
}
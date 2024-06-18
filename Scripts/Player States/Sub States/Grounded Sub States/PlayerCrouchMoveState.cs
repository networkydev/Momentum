using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Momentum.States
{
	public class PlayerCrouchMoveState : PlayerGroundedState
	{
		#region Temporary Values
		
		private float _targetSpeed;
		private float _speedDif;
		private float _finalMovement;
		
		private Quaternion _rotationQuat;
		private Vector3 _directionOfForce;
		private RaycastHit2D _uncrouchCheck;
		
		#endregion

		public PlayerCrouchMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			// Set box collider properties
			_playerReferences.PBC.size = _playerData.CrouchBoxColliderSize;
			_playerReferences.PBC.offset = _playerData.CrouchBoxColliderOffset;
		}

		public override void Exit()
		{
			base.Exit();
			
			// Reset box collider properties
			_playerReferences.PBC.size = _playerData.StandBoxColliderSize;
			_playerReferences.PBC.offset = _playerData.StandBoxColliderOffset;
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			if (!_isExitingState) 
			{
				if (_player.CanDoAction.UnCrouch(this)) { _stateMachine.ChangeState(_player.MoveState); return; }
				if (!_player.CanDoAction.Move(this)) { _stateMachine.ChangeState(_player.CrouchIdleState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
			
			if (_playerData.AddForceParallelToSlope && _playerChecks.GroundedCheckCtx.normal.x % 1 != 0 && _playerChecks.GroundedCheckCtx.normal.y % 1 != 0) // Add Force Parallel to Slope and the ground is not flat
			{ // If we are on slope then apply force parallel
				_rotationQuat = Quaternion.AngleAxis(-90f, Vector3.forward); // Form the rotation Quaternion (you don't need to understand how this works)
				_directionOfForce = _wasdInput.x * (_rotationQuat * _playerChecks.GroundedCheckCtx.normal); // Multiplying the quaternion to the normal of the ground rotates so its parallel to the slope
				
				_playerReferences.PRB.velocity = _directionOfForce * _playerData.CrouchMoveSpeed; // Apply the force in that direction
			} else
			{ // Else apply it on the X axis
				_playerReferences.PRB.velocity = new Vector2(_wasdInput.x * _playerData.CrouchMoveSpeed, _playerReferences.PRB.velocity.y); // Add the force parallel to the X axis
			}
		}

		public bool CanUncrouch() 
		{
			if (_playerData.FixCantUncrouchWhenTouchingWall) 
			{ // Different options whether we want the uncrouch fix or not
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
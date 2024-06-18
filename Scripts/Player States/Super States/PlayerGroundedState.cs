using System;
using UnityEngine;

namespace Momentum.States 
{
	public class PlayerGroundedState : PlayerBaseState
	{
		public static event Action Grounded;
		
		protected Vector2 _wasdInput;
		private RaycastHit2D StickToGround;
		private RaycastHit2D StickToGround2;
		private float _minAmount;
		
		public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
			
			Grounded?.Invoke(); // Invoke our event to let subscribers know we are grounded
		}

		public override void Exit()
		{
			base.Exit();
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			_wasdInput = InputManager.instance.WASDInput; // Cache WASD input for grounded states
			_player.JumpState.ResetCoyoteTimer(); // Continuously reset coyote timer when on ground
			
			// Add Friction
			if (!_player.CanDoAction.Move(this) && _playerReferences.PRB.velocity.x != 0)
			{
				_minAmount = Mathf.Min(Mathf.Abs(_playerReferences.PRB.velocity.x), Mathf.Abs(_playerData.GroundFrictionAmount)); // Check if our velocity, or friction amount is smaller
				_minAmount *= Mathf.Sign(_playerReferences.PRB.velocity.x); // Reapply direction
				_playerReferences.PRB.AddForce(Vector2.right * -_minAmount, ForceMode2D.Impulse); // Reverse the force direction with negative, then multiply by Vector2.right to make sure we dont affect y axis then apply as a impulse to get instantaneous effect
			}
			
			if (!_isExitingState) 
			{
				if (_player.CanDoAction.LedgeHang(this)) { _stateMachine.ChangeState(_player.LedgeHangState); return; }
				if (_player.CanDoAction.Fall(this)) { _stateMachine.ChangeState(_player.FallState); return; }
				if (_player.CanDoAction.Fly(this)) { _stateMachine.ChangeState(_player.FlyState); return; }
				if (_player.CanDoAction.Jump(this)) { _stateMachine.ChangeState(_player.JumpState); return; }
				if (_player.CanDoAction.Dash(this)) { _stateMachine.ChangeState(_player.DashState); return; }
				if (_player.CanDoAction.WallJump(this)) { _stateMachine.ChangeState(_player.WallJumpState); return; }
				if (_player.CanDoAction.WallGrab(this)) { _stateMachine.ChangeState(_player.WallGrabState); return; }
				if (_player.CanDoAction.WallClimb(this)) { _stateMachine.ChangeState(_player.WallClimbState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();

			if (_playerData.PreventBouncingOnSlope) // If disable bouncing
			{
				#region If using CapsuleCollider use this

				// // If we are grounded but above the ground, apply downward force to get the player back to ground
				// StickToGround = Physics2D.Raycast(new Vector2(_playerReferences.PBC.bounds.center.x, _playerReferences.PBC.bounds.center.y - _playerReferences.PBC.bounds.extents.y), Vector2.down, 2f, _playerData.GroundLayerMask);
				// if (StickToGround.distance > 0.05) { _playerReferences.PRB.velocity -= new Vector2(0, _playerData.AntiBounceForce); }
				
				#endregion

				#region For BoxCollider

				// This is kind of hacky since BoxCollider normally doesn't work as well with slopes we fire a raycast from both bottom corners and see which one is closer to the ground, we use that to determine if we should add downward force. If both are the same we do nothing.

				// Bottom Right Corner
				StickToGround = Physics2D.Raycast(new Vector2(_playerReferences.PBC.bounds.center.x + _playerReferences.PBC.bounds.extents.x, _playerReferences.PBC.bounds.center.y - _playerReferences.PBC.bounds.extents.y), Vector2.down, 2f, _playerData.GroundLayerMask);

				// Bottom Left Corner
				StickToGround2 = Physics2D.Raycast(new Vector2(_playerReferences.PBC.bounds.center.x - _playerReferences.PBC.bounds.extents.x, _playerReferences.PBC.bounds.center.y - _playerReferences.PBC.bounds.extents.y), Vector2.down, 2f, _playerData.GroundLayerMask);

				if (StickToGround.distance < StickToGround2.distance && StickToGround.distance > 0.05) { _playerReferences.PRB.velocity -= new Vector2(0, _playerData.AntiBounceForce); }
				if (StickToGround2.distance < StickToGround.distance && StickToGround2.distance > 0.05) { _playerReferences.PRB.velocity -= new Vector2(0, _playerData.AntiBounceForce); }
				
				#endregion
			}
		}
	}
}
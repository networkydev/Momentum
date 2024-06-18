using UnityEngine;

namespace Momentum.States
{
	public class PlayerMoveState : PlayerGroundedState
	{
		#region Temporary Values
		
		private float _speedDif;
		private float _targetSpeed;
		private float _finalMovement;
		
		private Quaternion _rotationQuat;
		private Vector3 _directionOfForce;

		
		#endregion

		public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
		{
		}

		public override void Enter()
		{
			base.Enter();
		}

		public override void Exit()
		{
			base.Exit();
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			if (!_isExitingState)
			{
				if (!_player.CanDoAction.Move(this)) { _stateMachine.ChangeState(_player.IdleState); return; }
				if (_player.CanDoAction.Crouch(this)) { _stateMachine.ChangeState(_player.CrouchMoveState); return; }
				if (_player.CanDoAction.Sprint(this)) { _stateMachine.ChangeState(_player.SprintState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();


			if (_playerData.AddForceParallelToSlope && _playerChecks.GroundedCheckCtx.normal.x % 1 != 0 && _playerChecks.GroundedCheckCtx.normal.y % 1 != 0) // Apply Force Parallel to Ground and ground is not flat
			{
				_rotationQuat = Quaternion.AngleAxis(-90f, Vector3.forward); // Create a quaternion with specific value
				_directionOfForce = _rotationQuat * _playerChecks.GroundedCheckCtx.normal; // Use the quaternion to rotate the the direction vector to be parallel to ground
				
				_targetSpeed = _wasdInput.x * _playerData.MoveSpeed; // Calculate the move speed
				_speedDif = _targetSpeed - _playerReferences.PRB.velocity.x;
				_finalMovement = Mathf.Pow(Mathf.Abs(_speedDif) * _playerData.Acceleration, _playerData.VelocityPower) * Mathf.Sign(_speedDif);
				
				_playerReferences.PRB.velocity += (Vector2) _directionOfForce * (Time.deltaTime * _finalMovement); // Apply move speed parallel to ground using the direction vector
			} else
			{
				_targetSpeed = _wasdInput.x * _playerData.MoveSpeed; // Calculate move speed
				_speedDif = _targetSpeed - _playerReferences.PRB.velocity.x;
				_finalMovement = Mathf.Pow(Mathf.Abs(_speedDif) * _playerData.Acceleration, _playerData.VelocityPower) * Mathf.Sign(_speedDif);
				
				_playerReferences.PRB.velocity += (Time.deltaTime * _finalMovement * Vector2.right); // Apply it
			}
		}
	}
}
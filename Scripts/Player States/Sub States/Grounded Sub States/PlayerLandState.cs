namespace Momentum.States
{
	public class PlayerLandState : PlayerGroundedState
	{
		public PlayerLandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName) : base(player, stateMachine, playerData, playerChecks, playerReferences, animName)
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
				if (_player.CanDoAction.Move(this)) { _stateMachine.ChangeState(_player.MoveState); return; }
				if (!_player.CanDoAction.Move(this)) { _stateMachine.ChangeState(_player.IdleState); return; }
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();
		}
	}
}
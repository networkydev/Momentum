using UnityEngine;

namespace Momentum.States 
{
	public class PlayerBaseState // The script all states inherit from
	{
		// All our references to be used by inherited scripts, filled in by the constructor
		protected Player _player;
		protected PlayerStateMachine _stateMachine;
		protected PlayerData _playerData;
		protected PlayerChecks _playerChecks;
		protected PlayerReferences _playerReferences;

		protected bool _isExitingState; // Makes sure states know not to add more jump force, move speed, ect the frame before we exit that state
		
		private string _animationName; // Our animation will be changed in this script using this string

		public PlayerBaseState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, PlayerChecks playerChecks, PlayerReferences playerReferences, string animName)
		{ // Constructor
			_player = player; // Our Player script itself
			_stateMachine = stateMachine; // Our StateMachine, created in Player.cs
			_playerData = playerData; // Access to our player values [Jump Force, Move Speed, Friction]
			_playerChecks = playerChecks; // Access all of our checks [IsGrounded, TouchingWall, TouchingLedge]
			_playerReferences = playerReferences; // Access our references stores in PlayerReferences.cs
			
			_animationName = animName; // What is the animation name to give our animator to switch to the new animation
		}

		// All of these methods are called in Player.cs by grabbing the current state from StateMachine and running. In inherited scripts t.hey will overwrite these methods and call base.MethodName() to make sure these run

		public virtual void Enter() // Gets called when we enter a state
		{
			_isExitingState = false;
		}

		public virtual void Exit() // Gets called when we leave a state
		{
			_isExitingState = true;
		}

		public virtual void LogicUpdate() // Gets called in Update()
		{
			_playerReferences.PAnimator.Play(_animationName);
			
			// Flip the player sprite according to direction we're facing
			if (_playerChecks.IsFacing == PlayerChecks.Facing.Left) { _playerReferences.PSprite.flipX = true; }
			else if (_playerChecks.IsFacing == PlayerChecks.Facing.Right) { _playerReferences.PSprite.flipX = false; }
		}

		public virtual void PhysicsUpdate() // Gets called in FixedUpdate()
		{
			
		}
	}
}
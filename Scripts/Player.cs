using System;
using UnityEngine;
using Momentum.States;
using TMPro;

namespace Momentum 
{
	public class Player : MonoBehaviour
	{
		public PlayerStateMachine StateMachine { get; private set; } // Manages all our states
		public PlayerActionCheck CanDoAction { get; private set; } // Checks if we can transition to another state
		
		public event Action OnStartEvent; // Calls every method subscribed to this in Start(). We will manually subscribe any states we need to have access to Start in this script
		public event Action OnUpdateEvent; // Calls every method subscribed to this in Update(). States will subscribe to this on their own within their script using the Start() method
		
		private PlayerReferences _playerReferences;

		#region Player States
		
		public PlayerIdleState IdleState { get; private set; }
		public PlayerMoveState MoveState { get; private set; }
		public PlayerCrouchIdleState CrouchIdleState { get; private set; }
		public PlayerCrouchMoveState CrouchMoveState { get; private set; }
		public PlayerSprintState SprintState { get; private set; }
		public PlayerLandState LandState { get; private set; }
		
		public PlayerJumpState JumpState { get; private set; }
		public PlayerDashState DashState { get; private set; }
		public PlayerWallJumpState WallJumpState { get; private set; }
		
		public PlayerLedgeHangState LedgeHangState { get; private set; }
		public PlayerLedgeClimbState LedgeClimbState { get; private set; }
		public PlayerFallState FallState { get; private set; }
		public PlayerFlyState FlyState { get; private set; }
		
		public PlayerWallGrabState WallGrabState { get; private set; }
		public PlayerWallSlideState WallSlideState { get; private set; }
		public PlayerWallClimbState WallClimbState { get; private set; }
		
		#endregion

		#region Animation Strings
		
		[field: SerializeField] public string AnimIdle { get; private set; } = "Idle";
		[field: SerializeField] public string AnimMove { get; private set; } = "Walk";
		[field: SerializeField] public string AnimSprint { get; private set; } = "Run";
		[field: SerializeField] public string AnimCrouchIdle { get; private set; } = "Crouch";
		[field: SerializeField] public string AnimCrouchMove { get; private set; } = "Crawl";
		[field: SerializeField] public string AnimLand { get; private set; } = "Land";

		[field: SerializeField] public string AnimJump { get; private set; } = "JumpRise";
		[field: SerializeField] public string AnimDash { get; private set; } = "Dash";
		[field: SerializeField] public string AnimWallJump { get; private set; } = "WallJump";

		[field: SerializeField] public string AnimWallGrab { get; private set; } = "WallClimbIdle";
		[field: SerializeField] public string AnimWallSlide { get; private set; } = "WallSlide";
		[field: SerializeField] public string AnimWallClimb { get; private set; } = "WallClimb";

		[field: SerializeField] public string AnimFall { get; private set; } = "JumpFall";
		[field: SerializeField] public string AnimFly { get; private set; } = "JumpRise";
		[field: SerializeField] public string AnimLedgeHang { get; private set; } = "LedgeHang";
		[field: SerializeField] public string AnimLedgeClimb { get; private set; } = "LedgeClimb";
		
		#endregion

		private void Awake()
		{
			_playerReferences = GetComponent<PlayerReferences>(); // Get the _playerReferences reference cached
			
			StateMachine = new PlayerStateMachine(_playerReferences); // Make a new State Machine
			CanDoAction = new PlayerActionCheck(_playerReferences, this); // Make a new ActionCheck
		}
		
		private void Start() 
		{
			// Make all the states
			// Pass references to this script, the state machine, player data, player checks, and player references for other references (like rigidbody), and the string for the animation name that is present in the animation controller
			IdleState = new PlayerIdleState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimIdle);
			MoveState = new PlayerMoveState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimMove);
			CrouchIdleState = new PlayerCrouchIdleState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimCrouchIdle);
			CrouchMoveState = new PlayerCrouchMoveState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimCrouchMove);
			SprintState = new PlayerSprintState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimSprint);
			LandState = new PlayerLandState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimLand);
			
			JumpState = new PlayerJumpState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimJump);
			DashState = new PlayerDashState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimDash);
			WallJumpState = new PlayerWallJumpState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimWallJump);
			
			LedgeHangState = new PlayerLedgeHangState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimLedgeHang);
			LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimLedgeClimb);
			FallState = new PlayerFallState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimFall);
			FlyState = new PlayerFlyState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimFly);
			
			WallGrabState = new PlayerWallGrabState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimWallGrab);
			WallSlideState = new PlayerWallSlideState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimWallSlide);
			WallClimbState = new PlayerWallClimbState(this, StateMachine, _playerReferences.PData, _playerReferences.PChecks, _playerReferences, AnimWallClimb);
			
			// Subscribe States to Start()
			OnStartEvent += JumpState.Start;
			OnStartEvent += DashState.Start;
			OnStartEvent += WallGrabState.Start;
			OnStartEvent += WallJumpState.Start;
			
			StateMachine.Initialize(IdleState);
			
			_playerReferences.PBC.sharedMaterial.friction = _playerReferences.PData.NormalFriction; // Set friction to normal friction
			
			OnStartEvent?.Invoke(); // Call the OnStartEvent for the states if not null
		}

		private void Update()
		{
			StateMachine.CurrentState.LogicUpdate(); // Call the Logic Update of the current state

			OnUpdateEvent?.Invoke(); // Call the OnUpdateEvent
		}

		private void FixedUpdate() 
		{
			StateMachine.CurrentState.PhysicsUpdate(); // Call the Physics Update of the current state
		}
		public void debug(RaycastHit2D r) {  Debug.Log(r); }
		public void LedgeClimbAnimationFinished() { LedgeClimbState.Climb(); } // Called by animation event to trigger ledge climb after the animation finishes
	}
}
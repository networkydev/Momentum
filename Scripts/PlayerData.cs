using UnityEngine;

namespace Momentum 
{
	// Scriptable Object
	// This class is for data values such as movement speed, jump force and anything that can be set to change the players behaviour
	[CreateAssetMenu(menuName = "Player Data")]
	public class PlayerData : ScriptableObject
	{
		[Header("Enable Actions")] // Enable if the player can do certain actions like move, jump, sprint, ledge climb, ect.
		
		public bool Move = true;
		public bool Sprint = true;
		public bool Crouch = true;
		public bool Jump = true;
		public bool Dash = true;
		public bool LedgeHang = true;
		public bool LedgeClimb = true;
		public bool WallGrab = true;
		public bool WallClimb = true;
		public bool WallSlide = true;
		public bool WallJump = true;

		[Header("Debug")]

		public bool StateChangeDebug = false;
		
		[Space(10)]
		
		[Tooltip("Show/Hide Grounded Box Check Gizmo (Only works in editor not in build versions)")]
		public bool GroundedCheckGizmo = false;
		[Tooltip("Show/Hide Uncrouch gizmo (Only works in editor not in build versions)")]
		public bool UncrouchCheckGizmo = false;
				
		[Space(10)]
		
		[Tooltip("Show/Hide Ledge check gizmo (Only works in editor not in build versions)")]
		public bool LedgeCheckGizmo = false;
		[Tooltip("Show/Hide the ledge climb gizmo (Only works in editor not in build versions)")]
		public bool LedgeClimbCheckGizmo = false;
		[Tooltip("Show/Hide the ledge hang gizmo (Only works in editor not in build versions)")]
		public bool LedgeHangCheckGizmo = false;
				
		[Space(10)]
		
		[Tooltip("Show/Hide Wall Check Gizmo (Only works in the editor and not in build versions)")]
		public bool WallCheckGizmo = false;
		[Tooltip("Show/Hide wall grab gizmo (Only works in editor not in build versions)")]
		public bool WallGrabCheckGizmo = false;
		
		
		
		[Header("LayerMask")]
		
		[Tooltip("What is considered ground? Used to determine when the player is grounded")]
		public LayerMask GroundLayerMask;
		[Tooltip("What is considered a wall? Usually the same as what is considered ground")]
		public LayerMask WallLayerMask;
		[Tooltip("What can our player not exist in? This is used for ray/box casts to determine if the player will end up in solid objects when changing box collider properties or setting transform.position. Will most likely be combination of wall & grounded layer mask")]
		public LayerMask CantExistIn;
		


		[Header("Box Collider")]

		[Tooltip("Box collider info to set when in crouch move or crouch idle. See documentation for more details")]
		public Vector2 StandBoxColliderOffset = new Vector2(-0.02f, -0.19f);
		[Tooltip("Box collider info to set when in crouch move or crouch idle. See documentation for more details")]
		public Vector2 StandBoxColliderSize = new Vector2(0.82f, 1.78f);
		
		[Space(10)]
		
		[Tooltip("Box collider info to set when in crouch move or crouch idle. See documentation for more details")]
		public Vector2 CrouchBoxColliderOffset = new Vector2(0.02f, -0.44f);
		[Tooltip("Box collider info to set when in crouch move or crouch idle. See documentation for more details")]
		public Vector2 CrouchBoxColliderSize = new Vector2(0.75f, 1.28f);
			
		[Space(10)]

		[Tooltip("Box collider information to set when in ledge hang and climb, see documentation for more details")]
		public Vector2 LedgeBoxColliderOffset = new Vector2(0f, 0.66f);
		[Tooltip("Box collider information to set when in ledge hang and climb, see documentation for more details")]
		public Vector2 LedgeBoxColliderSize = new Vector2(0.38f, 1.25f);
		
		[Space(10)]
		
		[Tooltip("Box collider information to set when in wall state. See deocumentation for more information")]
		public Vector2 WallBoxColliderOffset = new Vector2(0, 0.64f);
		[Tooltip("Box collider information to set when in wall state. See deocumentation for more information")]
		public Vector2 WallBoxColliderSize = new Vector2(0.35f, 1.13f);
		


		[Header("Movement")]
		
		[Tooltip("The length of the grounded boxcast on the y axis. Smaller is generally better ~0.1 should be good")]
		public float GroundedCheckBoxLength = 0.08f;
		[Tooltip("This sligtly shrinks the boxcast on the x axis to make it so the player cant touch the wall and keep jumping. Roughly ~0.01 is good")]
		public float GroundedCheckBoxShrinkX = 0.01f;
		[Tooltip("Base Move Speed")]
		public float MoveSpeed = 6f;
		[Tooltip("Replaces MoveSpeed when in Sprint state")]
		public float SprintSpeed = 8.26f;
		[Tooltip("Crouch move speed. Set using rigidBody.velocity")]
		public float CrouchMoveSpeed = 2.5f;
		
		[Space(10)]
		
		[Tooltip("Multiplied to the Base Speed")]
		public float Acceleration = 6f;
		[Tooltip("Lower it is, the longer it takes us to accelerate, when at 1 this varaible doesn't do anything")]
		[Range(0, 1.0f)] public float VelocityPower = 1f;
		[Tooltip("Higher value -> Faster Stop, Lower Value -> Skating on Ice Feel")]
		[Range(0.0f, 5f)] public float GroundFrictionAmount = 1.52f;



		[Header("Jump")]
		
		[Tooltip("Should we have jump cuts?")]
		public bool VariableJumpHeight = true;
		[Tooltip("When set to 1, you can only jump when grounded, when set to 2 or higher number it represents the number of times you can jump before having to touch the ground to regain jumps")]
		[Range(1, 5)] public int AmountOfJumps = 1;
		[Tooltip("Higher value results in a higher jump")]
		public float JumpHeight = 0.95f;
		[Tooltip("Higher value results in a lower jump")]
		public float JumpDistance = 2.6f;
		[Tooltip("When below this threshold, naturally transition out of jump")]
		public float JumpYThresh = 5f;
		
		[Space(10)]
		
		[Tooltip("Amount of time after hitting jump key that we can still automatically jump when other requirements are met (queues the jump for this many seconds)")]
		public float JumpBuffer = 0.2f;
		[Tooltip("The amount of time after leaving the ground that we are still able to jump. Only applies when AmountOfJumps is set to 1")]
		public float CoyoteTime = 0.2f;
		
		[Space(10)]
		
		[Tooltip("Number of seconds the player has to wait before being able to jump again. Setting to 0.2f prevents double or triple jumping when spamming space with coyote jump enabled")]
		public float JumpCoolDown = 0.2f;
		[Tooltip("Minimum time player has to be in jump before they can transition out")]
		public float MinJumpTime = 0f;
		[Tooltip("When releasing jump, if we can jump cut what to set the y velocity to")]
		public float JumpCutVelocity = 0.75f;



		[Header("Crouch")]
		
		[Tooltip("Sets the uncrouch check width to the width of crouch box collider x instead of stand box collider x. When touching wall and crouched, if unable to crouch then make this true")]
		public bool FixCantUncrouchWhenTouchingWall = true;
		
		[Space(10)]
		
		[Tooltip("Shrinks the uncrouch check in x and y. Set this to something small (0.05, 0.05) to avoid being unable to uncrouch in tight spaces")]
		public Vector2 UncrouchCheckShrink = new Vector2(0.05f, 0.05f);
		
		
		
		[Header("Ledge")]
		
		// Instead of Ledge actions having a custom layer mask this will run off the WallLayerMask since ledge climbing should be done on walls. Read WallLayerMask comment for more info
		
		[Tooltip("Skip ledge hang and automatically ledge climb")]
		public bool AutoLedgeClimb = false;	
					
		[Space(10)]
		
		[Tooltip("The length in the x axis that the ledge checks should extend out from the player")]
		public float LedgeCheckLength = 0.1f;
		[Tooltip("Negative values bring the ray cast lower from the top of the player box collider. This must be false for the player to be able to ledge hang")]
		public float LedgeCheck1OffsetY = 0;
		[Tooltip("MAKE SURE this value is more negative than ledge check 1 offset y. Negative values bring the ray cast lower from the top of the player box collider. This must be true for the player to be able to ledge hang")]
		public float LedgeCheck2OffsetY = -0.45f;
			
		[Space(10)]
		
		[Tooltip("Shrink the ledge climb check to avoid not being able to climb in tight spaces. (0.05, 0.05) is usually good")]
		public Vector2 LedgeClimbCheckShrink = new Vector2(0.05f, 0.05f);
		[Tooltip("Shrink the ledge hang check to avoid not being able to hang in tight spaces. (0.05, 0.05) is usually good")]
		public Vector2 LedgeHangCheckShrink = new Vector2(0.05f, 0.05f);
		
		[Space(10)]
		
		[Tooltip("Relative position to set from the corner of object you are hanging on during ledge climb animation, see documentation for more details")]
		public Vector2 LedgeClimbOffset = new Vector2(-0.2f, -1.23f);
		[Tooltip("Relative position to set from the corner of object you are hanging on during ledge hang animation, see documentation for more details")]
		public Vector2 LedgeHangOffset = new Vector2(-0.2f, -1.23f);
		
		
		
		[Header("Air")]
		
		[Tooltip("Should the player be able to add force on the x axis when in air? Recommended: true")]
		public bool CanMoveInAir = true;
		[Tooltip("Multiply the default gravity by this number when falling. Reverts back to default after exiting fall state")]
		public float FallGravityMultiplier = 2.3f;
		[Tooltip("Clamp the maximum fall velocity")]
		public float MaxFallForce = -20.52f;
		
		
		
		[Header("Slope")]
		
		[Tooltip("Adds force parallel to the slope while moving for consistent move speed")]
		public bool AddForceParallelToSlope = true;
		
		[Space(10)]
		
		[Tooltip("Adds some force downward while on a slope to avoid bouncing. Turn on if you experience any bounces on slopes")]
		public bool PreventBouncingOnSlope = false;
		[Tooltip("Applies amount of force downward when on a slope and not idle to prevent player bouncing. Only works when \"Prevent Bouncing On Slope\" is enabled")]
		public float AntiBounceForce = 1.5f;
		
		[Space(10)]
		
		[Tooltip("Disables and Renables boxcollider for during 1 frame when on slope and grounded")]
		public bool PreventSlidingOnSlopesWhenIdle = true;
		[Tooltip("Set this to 0 to prevent player sticking to walls. Boxcollider must have Physics Material")]
		public float NormalFriction = 0f;
		[Tooltip("Set this to around 50 to disable sliding on slopes. Boxcollider must have Physics Material")]
		public float SlopeFriction = 100f;
		
		
		
		[Header("Dash")]
		
		[Tooltip("When false, player dashes in the direction of WASD input. When true dashes toward the mouse direction")]
		public bool DashTowardMouse = true;
		[Tooltip("The number of dashes you can perform before they need to regenerate again")]
		public int AmountOfDashes = 1;
		[Tooltip("The curve will only be evaluated up to X = DashLength, Y Axis = percentage of max speed player is at capped at 1 which is 100% and 0 is 0%. See documentation")]
		public AnimationCurve DashCurve;
		[Tooltip("Amount of force to apply when dashing")]
		public float DashStrength = 17f;
		[Tooltip("How long to keep applying DashForce. The higher this is the more floaty and out of control the player feels")]
		public float DashLength = 0.1f;
		[Tooltip("After DashLength finishes the amount of time we have to ease from dash force to 0")]
		public float DashHangTime = 0.2f;
		[Tooltip("Time player has to wait before using another dash. If set to 0, when player dashes all dashes will be used up")]
		public float TimeBetweenDashes = 0.25f;
		[Tooltip("Amount of time it takes for used dashes to be avaliable to use again")]
		public float DashRegenerationCooldown = 0.35f;
		
		
		
		[Header("Wall")]
		
		[Tooltip("Shrink the wall grab check by this value. If x is set to 0, we will be unable to climb")]
		public Vector2 WallGrabCheckShrink = new Vector2(0.05f, 0.05f);
		
		[Space(10)]
		
		[Tooltip("The length in the x axis the wall check should extend out from the player")]
		public float WallCheckDistance = 0.1f;
		[Tooltip("Turn on wall check gizmo to view, all 3 have to touch the wall in order for player to be considered touching wall. Calculated in percents. 100% is top most of player height. 0% is at the bottom of the player")]
		[Range(0f, 100f)] public float WallCheck1Height = 81.4f;
		[Tooltip("Turn on wall check gizmo to view, all 3 have to touch the wall in order for player to be considered touching wall. Calculated in percents. 100% is top most of player height. 0% is at the bottom of the player")]
		[Range(0f, 100f)] public float WallCheck2Height = 60.5f;
		[Tooltip("Turn on wall check gizmo to view, all 3 have to touch the wall in order for player to be considered touching wall. Calculated in percents. 100% is top most of player height. 0% is at the bottom of the player")]
		[Range(0f, 100f)] public float WallCheck3Height = 40.5f;
			
		[Space(10)]
			
		[Tooltip("Should there be a time limit for how long we can grab/climb for?")]
		public bool ShouldHaveGrabStamina = false;
		[Tooltip("The amount of time we can grab/climb for. Timer keeps ticking when in wall states, resets once we leave wall state")]
		public float GrabStamina = 3f;
		[Tooltip("Velocity to set when climbing")]
		public float WallClimbSpeed = 3f;
		[Tooltip("Velocity to set when sliding")]
		public float SlideSpeed = 5f;
		[Tooltip("Amount of time we have to ease the player to the slide speed velocity")]
		public float SlideSpeedSmoothTime = 0.3f;
		
		
		
		[Header("Wall Jump")]
		
		[Tooltip("When checking if we can wall jump, should the player have to face the wall in order to be able to wall jump?")]
		public bool WallCheckBothSidesForWallJump = true;
		
		[Space(10)]
		
		[Tooltip("How fast the player can move in air while doing wall jump")]
		public float WallJumpMoveSpeed = 5f;
		[Tooltip("The time it takes for the player to regain control of player in air after wall jumping. Higher values avoid player being able to climb a single wall by just wall jumping")]
		public float ReturnMovementControlTimer = 0.15f;
		[Tooltip("Minimum amount of time we stay in wall jump for")]
		public float WallJumpTime = 0.06f;
		[Tooltip("When below this threshold, naturally transition out of jump")]
		public float WallJumpYThresh = 1.3f;
		[Tooltip("Time player has to wait before wall jumping again. Recommended 0.1")]
		public float WallJumpCooldown = 0.1f;
		[Tooltip("Amount of force to apply in x and y axis when executing a linear wall jump")]
		public Vector2 LinearWallJumpForce = new Vector2(6, 5);
	}
}
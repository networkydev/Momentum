using System;
using UnityEngine;

namespace Momentum 
{
	public class InputManager : MonoBehaviour
	{
		public static InputManager instance; // Creates one instance for everyone to use
		private InputMaster _controls; // Reference to the InputMaster script where our controls are stored
		
		#region Player Input Map
		// Avalible for the rest of the classes to use (Mainly should be used by PlayerAction as a front end for other classes)

		public Vector2 WASDInput { get; private set; }

		[NonSerialized] public Action JumpPressed; // Public so other classes can subscribe to this. Tagged to not show up in inspector
		[NonSerialized] public Action JumpReleased;
		
		public bool JumpHeld { get; private set; }
		public bool IsSprint { get; private set; }
		public bool IsDash { get; private set; }
		public bool IsCrouch { get; private set; }
		public bool IsGrab { get; private set; }
		public bool IsAttack { get; private set; }

		#endregion
		
		private void Awake()
		{
			// The Singleton Pattern makes sure there is only one instance of this class
			#region Singleton
			
			if (instance == null) // If no instance has been created
			{
				instance = this; // Make the instance
				_controls = new InputMaster();
				DontDestroyOnLoad(gameObject); // Make sure object this script is attached to is not destoryed when scene is switched
			}
			else // If there is an instance existing
			{
				Destroy(gameObject); // Get rid of it
				return; // Close method
			}

			#endregion
		}
		
		private void Update()
		{
			#region Read Input

			WASDInput = _controls.Player.MovementControls.ReadValue<Vector2>();
			
			if (_controls.Player.Jump.WasPressedThisFrame()) { JumpPressed?.Invoke(); } // THe ? makes sure the varaible isn't null before invoking
			if (_controls.Player.Jump.WasReleasedThisFrame()) { JumpReleased?.Invoke(); }
			JumpHeld = _controls.Player.Jump.inProgress;
			
			// Set bools
			if (_controls.Player.Sprint.IsPressed()) { IsSprint = true; } else if (!_controls.Player.Sprint.IsPressed()) { IsSprint = false; }
			if (_controls.Player.Dash.IsPressed()) { IsDash = true; } else if (!_controls.Player.Dash.IsPressed()) { IsDash = false; }
			if (_controls.Player.Crouch.IsPressed()) { IsCrouch = true; } else if (!_controls.Player.Crouch.IsPressed()) { IsCrouch = false; }
			if (_controls.Player.Grab.IsPressed()) { IsGrab = true; } else if (!_controls.Player.Grab.IsPressed()) { IsGrab = false; }
			
			#endregion
		}

		#region Enable/Disable Input

		private void OnEnable() // Disable and enable the controls key mappings when parent gameobject is disabled or enabled to prevent errors
		{
			_controls.Enable();
		}

		private void OnDisable()
		{
			_controls.Disable();
		}
		
		#endregion
	}
}

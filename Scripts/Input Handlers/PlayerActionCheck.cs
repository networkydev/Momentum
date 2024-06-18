using Momentum.States;
using System.Diagnostics;

namespace Momentum 
{
	public class PlayerActionCheck
	{
		private PlayerReferences _playerReferences;
		private Player _player;
		
		public PlayerActionCheck(PlayerReferences References, Player Player) // Contains transition checks for our player states
		{
			_playerReferences = References;
			_player = Player;
		}
		
		public bool Move(PlayerBaseState state) 
		{
			if (_playerReferences.PData.Move && InputManager.instance.WASDInput.x != 0) { return true; } // If Move is enabled and we arent moving in the X axis

			return false;
		}
		
		public bool Sprint(PlayerBaseState state) 
		{
			if (_playerReferences.PData.Sprint && InputManager.instance.IsSprint && Move(state)) { return true; } // If Sprint is enabled and input sprint is pressed and we are able to preform the move action

			return false;
		}
		
		public bool UnCrouch(PlayerBaseState state) 
		{
			if (!Crouch(state) && _player.CrouchIdleState.CanUncrouch()) { return true; } // We are not able to prefrom the crouch action and the CanUncrouch method returns true
			
			return false;
		}
		
		public bool Crouch(PlayerBaseState state) 
		{
			if (state == _player.DashState) // If we are Dash state
			{
				if (_playerReferences.PData.Crouch && _playerReferences.PChecks.IsGrounded && _player.DashState.ShouldCrouch()) { return true; } // If crouch is enabled and we are grounded and DashState.ShouldCrouch returns true
				else { return false; }
			}
			
			// For all other states
			if (_playerReferences.PData.Crouch && InputManager.instance.IsCrouch) { return true; } // Crouch is enabled and we are pressing the crouch input
			
			return false;
		}
		
		public bool Jump(PlayerBaseState state) 
		{
			if (state == _player.CrouchIdleState || state == _player.CrouchMoveState)
			{
				if (_player.CrouchIdleState.CanUncrouch() && _playerReferences.PData.Jump && _playerReferences.PData.AmountOfJumps == 1 && _player.JumpState.JumpCoolDown <= 0 && _player.JumpState.JumpBuffer > 0 && _player.JumpState.CoyoteTimer > 0 // Jump is enabled and the total jump amount is 1 and cooldown is finished and the jump input has been pressed recently (within JumpBuffer) and we have left the ground within _playerReferences.PData.CoyoteTime seconds ago OR
					|| _player.CrouchIdleState.CanUncrouch() && _playerReferences.PData.Jump && _playerReferences.PData.AmountOfJumps != 1 && _player.JumpState.JumpsLeft > 0 && _player.JumpState.JumpBuffer > 0) // Jump is enabled and amount of jumps is not 1 and we have jumps left and we just recently pressed the jump
				{
					return true;
				} else { return false; }
            }

			if (_playerReferences.PData.Jump && _playerReferences.PData.AmountOfJumps == 1 && _player.JumpState.JumpCoolDown <= 0 && _player.JumpState.JumpBuffer > 0 && _player.JumpState.CoyoteTimer > 0 // Jump is enabled and the total jump amount is 1 and cooldown is finished and the jump input has been pressed recently (within JumpBuffer) and we have left the ground within _playerReferences.PData.CoyoteTime seconds ago OR
				|| _playerReferences.PData.Jump && _playerReferences.PData.AmountOfJumps != 1 && _player.JumpState.JumpsLeft > 0 && _player.JumpState.JumpBuffer > 0) // Jump is enabled and amount of jumps is not 1 and we have jumps left and we just recently pressed the jump
			{
				return true;
			}

			
			return false;
		}
		
		public bool Dash(PlayerBaseState state) 
		{
			if (_playerReferences.PData.Dash && InputManager.instance.IsDash && _player.DashState.CanDash() && _player.CrouchIdleState.CanUncrouch()) // Dash is enabled and dash input is pressed and DashState.CanDash returns true and we arent crouched
			{
				return true;
			}
			
			return false;
		}
		
		public bool WallGrab(PlayerBaseState state) 
		{
			if (state == _player.WallClimbState) // We are in Wall Climb state
			{
				if (_playerReferences.PData.WallGrab && InputManager.instance.WASDInput.y < 1) { return true; } // Wall Grab is enabled and Y input is greater than 1
				else { return false; }
			}
			
			if (_playerReferences.PData.WallGrab && _playerReferences.PData.ShouldHaveGrabStamina && InputManager.instance.IsGrab && _playerReferences.PChecks.IsTouchingFacingWall && _player.WallGrabState.CanWallGrab() && _player.WallGrabState.Stamina > 0 // Wall Grab is enabled and wall grab stamina is enabled and grab input is pressed and we are touching the wall we are facing and WallGrabState.CanWallGrab returns true and we have enough stamina OR
				|| _playerReferences.PData.WallGrab && !_playerReferences.PData.ShouldHaveGrabStamina && InputManager.instance.IsGrab && _playerReferences.PChecks.IsTouchingFacingWall && _player.WallGrabState.CanWallGrab()) // Wall Grab is enabled and we don't have stamina enabled and input grab is pressed and touching the facing wall and WallGrabState.CanWallGrab returns true
			{
				return true;
			} 
			
			return false;
		}
		
		public bool WallSlide(PlayerBaseState state) 
		{
			if (state.GetType().IsSubclassOf(typeof(PlayerWallState))) // Fancy way of saying is the current state we are in a subclass (inherits from) PlayerWallState
			{
				if (_playerReferences.PData.WallSlide && InputManager.instance.WASDInput.y < 0 // Wall Slide is enabled and Y input is down OR
					|| _playerReferences.PData.WallSlide && _playerReferences.PData.ShouldHaveGrabStamina && _player.WallGrabState.Stamina <= 0)  { return true; } // Wall Slide is enabled and stamina is eanbled and there is no stamina left
				else { return false; }
			} else if (state == _player.FallState)
			{
				if (_playerReferences.PData.WallSlide && _playerReferences.PChecks.IsTouchingFacingWall) { return true; } // Wall Slide is enabled and touching the wall we are facing
				else { return false; }
			} else if (state == _player.LedgeHangState) 
			{
				if (_playerReferences.PData.WallSlide && InputManager.instance.WASDInput.y < 0) { return true; } // Wall slide is enabled and Y input is down
				else { return false; }
			}
			
			// Only those states should be able to transition to wall slide
			return false;
		}
		
		public bool WallClimb(PlayerBaseState state) 
		{
			if (state.GetType().IsSubclassOf(typeof(PlayerGroundedState))) // If is subclass of Grounded
			{
				if (_playerReferences.PData.WallClimb && _playerReferences.PChecks.IsTouchingFacingWall && InputManager.instance.WASDInput.y > 0) { return true;} // Wall Climb enabled and touching the wall we are facing and Y input is up
				else { return false;}
			}
			
			if (_playerReferences.PData.WallClimb && _playerReferences.PData.ShouldHaveGrabStamina && InputManager.instance.WASDInput.y > 0 && _player.WallGrabState.Stamina > 0 // Wall Climb enabled and stamina is enabled and Y input is up and we have stamina
				|| _playerReferences.PData.WallClimb && !_playerReferences.PData.ShouldHaveGrabStamina && InputManager.instance.WASDInput.y > 0) // Wall Climb is enabled and stamina is not enabled and y input is up
			{
				return true;
			}

			return false;
		}
		
		public bool WallJump(PlayerBaseState state) 
		{
			if (state == _player.CrouchIdleState || state == _player.CrouchMoveState)
			{
				if (_playerReferences.PData.WallJump && _player.JumpState.JumpBuffer > 0 && _playerReferences.PChecks.IsTouchingAnyWall && _player.CrouchIdleState.CanUncrouch()) // Wall Jump is enabled and jump was pressed recently and we are touching A wall
				{
					// Send some facing information to Wall Jump State so we know if we need to flip the sprite
					if (_playerReferences.PChecks.IsTouchingFacingWall) { _player.WallJumpState.IsFacing(_playerReferences.PChecks.IsFacing); }
					else { _player.WallJumpState.IsFacing(_playerReferences.PChecks.IsFacing == PlayerChecks.Facing.Right ? PlayerChecks.Facing.Left : PlayerChecks.Facing.Right); }

					return true;
				} else { return false; }
            }

			if (_playerReferences.PData.WallJump && _player.JumpState.JumpBuffer > 0 && _playerReferences.PChecks.IsTouchingAnyWall) // Wall Jump is enabled and jump was pressed recently and we are touching A wall
			{
				// Send some facing information to Wall Jump State so we know if we need to flip the sprite
				if (_playerReferences.PChecks.IsTouchingFacingWall) { _player.WallJumpState.IsFacing(_playerReferences.PChecks.IsFacing); }
				else { _player.WallJumpState.IsFacing(_playerReferences.PChecks.IsFacing == PlayerChecks.Facing.Right ? PlayerChecks.Facing.Left : PlayerChecks.Facing.Right); }
				
				return true;
			}
			
			return false;
		}
		
		public bool LedgeHang(PlayerBaseState state) 
		{
			if (_playerReferences.PData.LedgeHang && _playerReferences.PChecks.IsTouchingLedge && _player.LedgeHangState.IsValidPosition()) { return true; } // Ledge Hang is enabled and we are touching a ledge and the ledge is in a valid position

			return false;
		}
		
		public bool LedgeClimb(PlayerBaseState state) 
		{
			if (state == _player.LedgeHangState)
			{
				if (_playerReferences.PData.LedgeClimb && InputManager.instance.WASDInput.y > 0 && _player.LedgeClimbState.IsValidPosition() // Ledge Climb is enabled and Y input is up and the ledge is climbable
					|| _playerReferences.PData.LedgeClimb && InputManager.instance.JumpHeld && _player.LedgeClimbState.IsValidPosition() // Ledge Climb is enabled and jump is held and ledge is climbable
					|| _playerReferences.PData.LedgeClimb && _player.LedgeClimbState.IsValidPosition() && _playerReferences.PData.AutoLedgeClimb) { return true; } // Ledge Climb is enabled and Ledge is valid position and AutoLedgeClimb is enabled
				else { return false; }
			}
			
			return false;
		}
		
		public bool Fly(PlayerBaseState state) 
		{
			if (state.GetType().IsSubclassOf(typeof(PlayerWallState))) // Is the state we are in subclass of wall state
			{
				if (!_playerReferences.PChecks.IsGrounded && _playerReferences.PRB.velocity.y > 0 && !_playerReferences.PChecks.IsTouchingFacingWall) { return true; } // Not grounded and Y velocity is less than 0 and not touching the wall we are facing
				else { return false; }
			} else if (state.GetType().IsSubclassOf(typeof(PlayerGroundedState))) // Is the state we are in a subclass of grounded state
			{
				if (!_playerReferences.PChecks.IsGrounded && _playerReferences.PRB.velocity.y > 0) { return true; } // Not grounded and y velocity is less than 0
				else { return false; }
			}
			
			if (_playerReferences.PRB.velocity.y > 0) { return true; } // Else Y velocity is less than 0
			
			return false;
		}
		
		public bool Fall(PlayerBaseState state) 
		{
			if (state == _player.WallSlideState) 
			{
				if (!_playerReferences.PChecks.IsTouchingFacingWall) { return true; } // We aren't touching the wall we are facing
				else { return false; }
			} else if (state == _player.WallClimbState) 
			{
				if (!_playerReferences.PChecks.IsTouchingFacingWall) { return true; } // We aren't touching the wall we are facing
				else { return false; }
			} else if (state.GetType().IsSubclassOf(typeof(PlayerWallState))) // Is the state we are in subclass of wall state
			{
				if (!_playerReferences.PChecks.IsGrounded && _playerReferences.PRB.velocity.y <= 0 && !_playerReferences.PChecks.IsTouchingFacingWall) { return true; } // Not grounded and velocity is less than or equal to 0 and we aren't touching the wall we are facing
				else { return false; }
			} else if (state.GetType().IsSubclassOf(typeof(PlayerGroundedState))) // Is the state we are in subclass of grounded state
			{
				if (!_playerReferences.PChecks.IsGrounded && _playerReferences.PRB.velocity.y < 0) { return true; } // Not grounded and y velocity is less than 0
				else { return false; }
			}
			
			if (_playerReferences.PRB.velocity.y <= 0) { return true; } // If none of the above, we are below Y velocity of 0
			
			return false;
		}
		
		public bool Land(PlayerBaseState state) // Transition to Land State
		{
			if (state == _player.DashState) 
			{
				if (_playerReferences.PChecks.IsGrounded && !_player.DashState.ShouldCrouch()) { return true; } // We are grounded and we should not crouch
				else { return false; }
			} else if (state == _player.WallGrabState) 
			{
				if (InputManager.instance.WASDInput.y < 0 && _playerReferences.PChecks.IsGrounded) { return true; } // Y input is down and we are grounded
				else { return false; }
			}
			
			// For all other states
			if (_playerReferences.PChecks.IsGrounded) { return true; } // If we are grounded
			
			return false;
		}
	}
}
using UnityEngine;
using UnityEngine.UIElements;

namespace Momentum 
{
	public class PlayerChecks : MonoBehaviour
	{
		// References
		private PlayerReferences _playerReferences;
		private Player _player;

		// [System.NonSerialized] is to make sure the public variables aren't accessible from the inspector
		[System.NonSerialized] public bool CanFlipPlayer = true;
		[System.NonSerialized] public bool DirectionOfLedgeHangCheck = true; // true = right, false = left. Just makes sure that the ledge hang check isnt updating to where the player is facing but where the player was facing when the ledge hang check's IsValidPosition() was called
		[System.NonSerialized] public Facing IsFacing = Facing.Right; // Using enum Facing, gives current direction character is facing. Is Facing.None if idle
		
		public bool IsGrounded { get; private set; } // True if Grounded, false if not
		public bool IsTouchingFacingWall { get; private set; } // True if Touching Wall, false if not
		public bool IsTouchingAnyWall { get; private set; } // True if Touching Wall, false if not
		public bool IsTouchingLedge { get; private set; } // If this is true we are in a climbable position near ledge, false if not
		
		private Vector3 _bcExtents; // Cache BoxCollider2D extents
		private Vector3 _bcPos; // Cache BoxCollider2D center
		
		// CTX = Context
		#region CTX
		
		public RaycastHit2D GroundedCheckCtx  { get; private set; } // Grounded Check boxcast context
		
		private RaycastHit2D _wallCheckCtx1; // These will face the direction the player is facing
		public RaycastHit2D WallCheckCtx2 { get; private set; }
		private RaycastHit2D _wallCheckCtx3;
		
		private RaycastHit2D _wallCheckCtx1Mirror; // These will always face the opposite direction the character is facing
		private RaycastHit2D _wallCheckCtx2Mirror;
		private RaycastHit2D _wallCheckCtx3Mirror;
		
		private RaycastHit2D _ledgeCheckCtx; // Ledge Check raycast context
		public RaycastHit2D LedgeCheckCtx2 { get; private set; } // This is the second raycast to check if we can ledge climb - set to public so states can access what platform we hit.

		#endregion

		private void Awake()
		{
			// Get References
			_playerReferences = GetComponent<PlayerReferences>();
			_player = GetComponent<Player>();
		}
		
		private void Update()
		{
			StartFrameCache(); // Cache the Box Colliuder Info
			
			IsFacing = CheckFaceDirection(); // Set the Facing and Grounded bools using methods
			IsGrounded = CheckIfGrounded();
			IsTouchingLedge = CheckIfTouchingLedge();
			
			// Do some special code so that we can output 2 different values from just one method
			// Learn about it --> https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/out-parameter-modifier
			CheckIfTouchingWall(out bool facingWallTemp, out bool anyWallTemp); // Create a temp var for both out fields
			IsTouchingFacingWall = facingWallTemp; // Set manually, because cannot have a out variable be a property
			IsTouchingAnyWall = anyWallTemp;
		}

		private void StartFrameCache() // Values we want to cache in this script. Its slightly more preformant and easier to read
		{
			_bcPos = _playerReferences.PBC.bounds.center; // Get Center of BoxCollider
			_bcExtents = _playerReferences.PBC.bounds.extents; // Get extents
		}

		private Facing CheckFaceDirection() // Check what direction player is facing
		{
			if (InputManager.instance.WASDInput.x < 0 && CanFlipPlayer) { return Facing.Left; }
			else if (InputManager.instance.WASDInput.x > 0 && CanFlipPlayer) { return Facing.Right; }
			else { return IsFacing; }	
		}
		
		public void SetFacing(Facing facing) 
		{
			IsFacing = facing;
		}

		private bool CheckIfGrounded()
		{
			GroundedCheckCtx = Physics2D.BoxCast(new Vector2(_bcPos.x, _bcPos.y - _bcExtents.y - (_playerReferences.PData.GroundedCheckBoxLength * 0.5f)), new Vector2((_bcExtents.x * 2) - _playerReferences.PData.GroundedCheckBoxShrinkX, _playerReferences.PData.GroundedCheckBoxLength), 0f, Vector2.down, 0, _playerReferences.PData.GroundLayerMask);

			if (GroundedCheckCtx == true) { return true; } else { return false; }
		}

		private void CheckIfTouchingWall(out bool TouchingFacingWall, out bool TouchingWallOnEitherSide)
		{
			if (IsFacing == Facing.Right)
			{
				_wallCheckCtx1 = Physics2D.Raycast(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))), Vector2.right, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
				WallCheckCtx2 = Physics2D.Raycast(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))), Vector2.right, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
				_wallCheckCtx3 = Physics2D.Raycast(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))), Vector2.right, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);

				if (_playerReferences.PData.WallCheckBothSidesForWallJump) 
				{
					_wallCheckCtx1Mirror = Physics2D.Raycast(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))), Vector2.left, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
					_wallCheckCtx2Mirror = Physics2D.Raycast(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))), Vector2.left, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
					_wallCheckCtx3Mirror = Physics2D.Raycast(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))), Vector2.left, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
				}
			}
			else // IsFacing == Facing.Left
			{
				_wallCheckCtx1 = Physics2D.Raycast(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))), Vector2.left, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
				WallCheckCtx2 = Physics2D.Raycast(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))), Vector2.left, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
				_wallCheckCtx3 = Physics2D.Raycast(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))), Vector2.left, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);

				if (_playerReferences.PData.WallCheckBothSidesForWallJump) 
				{
					_wallCheckCtx1Mirror = Physics2D.Raycast(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))), Vector2.right, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
					_wallCheckCtx2Mirror = Physics2D.Raycast(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))), Vector2.right, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
					_wallCheckCtx3Mirror = Physics2D.Raycast(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))), Vector2.right, _playerReferences.PData.WallCheckDistance, _playerReferences.PData.WallLayerMask);
				}
			}
			
			
			if (_wallCheckCtx1 && WallCheckCtx2 && _wallCheckCtx3) { TouchingFacingWall = true; } else { TouchingFacingWall = false; }
			if (TouchingFacingWall || _wallCheckCtx1Mirror && _wallCheckCtx2Mirror && _wallCheckCtx3Mirror) { TouchingWallOnEitherSide = true; } else { TouchingWallOnEitherSide = false; }
		}

		private bool CheckIfTouchingLedge()
		{
			if (IsFacing == Facing.Right)
			{
				_ledgeCheckCtx = Physics2D.Raycast(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y + _bcExtents.y + _playerReferences.PData.LedgeCheck1OffsetY), Vector2.right, _playerReferences.PData.LedgeCheckLength, _playerReferences.PData.WallLayerMask);
				LedgeCheckCtx2 = Physics2D.Raycast(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y + _bcExtents.y + _playerReferences.PData.LedgeCheck2OffsetY), Vector2.right, _playerReferences.PData.LedgeCheckLength, _playerReferences.PData.WallLayerMask);
			}
			else // IsFacing == Facing.Left
			{
				_ledgeCheckCtx = Physics2D.Raycast(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y + _bcExtents.y + _playerReferences.PData.LedgeCheck1OffsetY), Vector2.left, _playerReferences.PData.LedgeCheckLength, _playerReferences.PData.WallLayerMask);
				LedgeCheckCtx2 = Physics2D.Raycast(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y + _bcExtents.y + _playerReferences.PData.LedgeCheck2OffsetY), Vector2.left, _playerReferences.PData.LedgeCheckLength, _playerReferences.PData.WallLayerMask);
			}

			if (_ledgeCheckCtx == false && LedgeCheckCtx2 == true) { return true; } else { return false; }
		}

		// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDrawGizmos.html
		private void OnDrawGizmos() // Method that draws our visualizations of boxcasts & other things. Only works in editor not in build versions
		{
			if (Application.isPlaying) // Make sure we are in play mode in the editor so that our references like _playerReferences.PData work
			{
				if (_playerReferences.PData.GroundedCheckGizmo) // Checks if this gizmo is enabled
				{
					if (IsGrounded) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; } // Sets the color of the Grounded BoxCast Gizmo
					Gizmos.DrawWireCube(new Vector3(_bcPos.x, _bcPos.y - _bcExtents.y - (_playerReferences.PData.GroundedCheckBoxLength / 2)), new Vector3(_bcExtents.x * 2 - _playerReferences.PData.GroundedCheckBoxShrinkX, _playerReferences.PData.GroundedCheckBoxLength));
				}

				if (_playerReferences.PData.WallCheckGizmo) // Checks if this gizmo is enabled
				{
					if (IsTouchingFacingWall == true) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; } // Changes color if the cast hit/not hit
					if (IsFacing == Facing.Right) // Different functionality if facing a certain direction
					{
						Gizmos.DrawLine(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x + _bcExtents.x + (_wallCheckCtx1.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx1.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))));
						Gizmos.DrawLine(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x + _bcExtents.x + (WallCheckCtx2.distance == 0 ? _playerReferences.PData.WallCheckDistance : WallCheckCtx2.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))));
						Gizmos.DrawLine(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x + _bcExtents.x + (_wallCheckCtx3.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx3.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))));
						
						// Draw the mirror wall checks that are opposite to the above gizmos
						if (_playerReferences.PData.WallCheckBothSidesForWallJump) 
						{
							if (_wallCheckCtx2Mirror) { Gizmos.color = Color.gray; } else { Gizmos.color = Color.black; } // Changes color if the cast hit/not hit
								
							Gizmos.DrawLine(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x - _bcExtents.x - (_wallCheckCtx1Mirror.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx1Mirror.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))));
							Gizmos.DrawLine(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x - _bcExtents.x - (_wallCheckCtx2Mirror.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx2Mirror.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))));
							Gizmos.DrawLine(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x - _bcExtents.x - (_wallCheckCtx3Mirror.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx3Mirror.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))));
						}
					}
					else // Facing Left
					{
						Gizmos.DrawLine(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x - _bcExtents.x - (_wallCheckCtx1.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx1.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))));
						Gizmos.DrawLine(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x - _bcExtents.x - (WallCheckCtx2.distance == 0 ? _playerReferences.PData.WallCheckDistance : WallCheckCtx2.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))));
						Gizmos.DrawLine(new Vector2(_bcPos.x - _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x - _bcExtents.x - (_wallCheckCtx3.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx3.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))));
					
						if (_playerReferences.PData.WallCheckBothSidesForWallJump) 
						{
							if (_wallCheckCtx2Mirror) { Gizmos.color = Color.gray; } else { Gizmos.color = Color.black; } // Changes color if the cast hit/not hit
							
							Gizmos.DrawLine(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x + _bcExtents.x + (_wallCheckCtx1Mirror.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx1Mirror.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck1Height / 100) * (_bcExtents.y * 2))));
							Gizmos.DrawLine(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x + _bcExtents.x + (_wallCheckCtx2Mirror.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx2Mirror.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck2Height / 100) * (_bcExtents.y * 2))));
							Gizmos.DrawLine(new Vector2(_bcPos.x + _bcExtents.x, _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))), new Vector2(_bcPos.x + _bcExtents.x + (_wallCheckCtx3Mirror.distance == 0 ? _playerReferences.PData.WallCheckDistance : _wallCheckCtx3Mirror.distance), _bcPos.y - _bcExtents.y + ((_playerReferences.PData.WallCheck3Height / 100) * (_bcExtents.y * 2))));
						}
					}
				}

				if (_playerReferences.PData.LedgeCheckGizmo) // Checks if this gizmo is enabled
				{
					if (_ledgeCheckCtx == true) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; } // Changes color if the cast hit/not hit
					if (IsFacing == Facing.Right) // Different functionality if facing a certain direction
					{
						Gizmos.DrawRay(new Vector3(_bcPos.x + _bcExtents.x, _bcPos.y + _bcExtents.y + _playerReferences.PData.LedgeCheck1OffsetY), Vector3.right * (_ledgeCheckCtx.distance == 0 ? _playerReferences.PData.LedgeCheckLength : _ledgeCheckCtx.distance));
					}
					else // Facing Left
					{
						Gizmos.DrawRay(new Vector3(_bcPos.x - _bcExtents.x, _bcPos.y + _bcExtents.y + _playerReferences.PData.LedgeCheck1OffsetY), Vector3.left * (_ledgeCheckCtx.distance == 0 ? _playerReferences.PData.LedgeCheckLength : _ledgeCheckCtx.distance));
					}
					
					if (LedgeCheckCtx2 == true) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; } // Changes color if the cast hit/not hit
					if (IsFacing == Facing.Right) 
					{
						Gizmos.DrawRay(new Vector3(_bcPos.x + _bcExtents.x, _bcPos.y + _bcExtents.y + _playerReferences.PData.LedgeCheck2OffsetY), Vector3.right * (LedgeCheckCtx2.distance == 0 ? _playerReferences.PData.LedgeCheckLength : LedgeCheckCtx2.distance));
					} else 
					{
						Gizmos.DrawRay(new Vector3(_bcPos.x - _bcExtents.x, _bcPos.y + _bcExtents.y + _playerReferences.PData.LedgeCheck2OffsetY), Vector3.left * (LedgeCheckCtx2.distance == 0 ? _playerReferences.PData.LedgeCheckLength : LedgeCheckCtx2.distance));
					}
				}
				
				if (_playerReferences.PData.LedgeHangCheckGizmo) 
				{
					if (_player.LedgeHangState.HangPositionCast.collider == null) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; } // Changes color if the cast hit/not hit
					
					if (DirectionOfLedgeHangCheck) { Gizmos.DrawWireCube(_player.LedgeHangState.HangPosition + new Vector3(_playerReferences.PData.LedgeBoxColliderOffset.x, _playerReferences.PData.LedgeBoxColliderOffset.y, 0), new Vector2(_playerReferences.PData.LedgeBoxColliderSize.x - _playerReferences.PData.LedgeHangCheckShrink.x, _playerReferences.PData.LedgeBoxColliderSize.y - _playerReferences.PData.LedgeHangCheckShrink.y)); }
					else { Gizmos.DrawWireCube(_player.LedgeHangState.HangPosition + new Vector3(-_playerReferences.PData.LedgeBoxColliderOffset.x, _playerReferences.PData.LedgeBoxColliderOffset.y, 0), new Vector2(_playerReferences.PData.LedgeBoxColliderSize.x - _playerReferences.PData.LedgeHangCheckShrink.x, _playerReferences.PData.LedgeBoxColliderSize.y - _playerReferences.PData.LedgeHangCheckShrink.y)); }
				}
				
				if (_playerReferences.PData.LedgeClimbCheckGizmo) 
				{
					if (_player.LedgeClimbState.LedgeClimbCheck.collider == null) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; } // Changes color if the cast hit/not hit
					
					Gizmos.DrawWireCube(_player.LedgeClimbState.LedgeClimbFinishPosition, new Vector2(_playerReferences.PData.StandBoxColliderSize.x - _playerReferences.PData.LedgeClimbCheckShrink.x, _playerReferences.PData.StandBoxColliderSize.y - _playerReferences.PData.LedgeClimbCheckShrink.y));
				}
				
				if (_playerReferences.PData.UncrouchCheckGizmo && (_player.StateMachine.CurrentState == _player.CrouchIdleState || _player.StateMachine.CurrentState == _player.CrouchMoveState)) // Only display if we're in a crouch state
				{
					if (_player.CrouchIdleState.CanUncrouch()) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; } // If can uncrouch then green color, if not red

					if (_playerReferences.PData.FixCantUncrouchWhenTouchingWall) 
					{
						Gizmos.DrawWireCube(new Vector2(_player.transform.position.x + _playerReferences.PData.CrouchBoxColliderOffset.x, _player.transform.position.y + _playerReferences.PData.StandBoxColliderOffset.y), new Vector2(_playerReferences.PData.CrouchBoxColliderSize.x, _playerReferences.PData.StandBoxColliderSize.y));
					} else 
					{
						Gizmos.DrawWireCube(new Vector2(_player.transform.position.x, _player.transform.position.y) + _playerReferences.PData.StandBoxColliderOffset, _playerReferences.PData.StandBoxColliderSize);
					}
				}
				
				if (_playerReferences.PData.WallGrabCheckGizmo) 
				{
					Gizmos.color = Color.white; // One color if hit or not hit
					if (_player.WallGrabState.FacingCache == Facing.Right) 
					{
                        Gizmos.DrawWireCube(new Vector2(_player.WallGrabState.PlayerPOS.x + _playerReferences.PData.WallBoxColliderOffset.x + (_playerReferences.PData.WallBoxColliderSize.x * 0.5f) + (_player.WallGrabState.WallRaycast.distance * 0.5f), _player.WallGrabState.PlayerPOS.y + _playerReferences.PData.WallBoxColliderOffset.y), new Vector2(_player.WallGrabState.WallRaycast.distance, _playerReferences.PData.WallBoxColliderSize.y) - _playerReferences.PData.WallGrabCheckShrink);
					} else 
					{
                        Gizmos.DrawWireCube(new Vector2(_player.WallGrabState.PlayerPOS.x - _playerReferences.PData.WallBoxColliderOffset.x - (_playerReferences.PData.WallBoxColliderSize.x * 0.5f) - (_player.WallGrabState.WallRaycast.distance * 0.5f), _player.WallGrabState.PlayerPOS.y + _playerReferences.PData.WallBoxColliderOffset.y), new Vector2(_player.WallGrabState.WallRaycast.distance, _playerReferences.PData.WallBoxColliderSize.y) - _playerReferences.PData.WallGrabCheckShrink);
					}
                }
            }
        }
        public enum Facing // Critical facing enum
        {
            Right,
            Left
        }
	}
}
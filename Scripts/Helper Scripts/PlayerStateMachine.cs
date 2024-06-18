using System.Diagnostics;
using UnityEngine;
using Momentum.States;

namespace Momentum 
{
	public class PlayerStateMachine // Manage states
	{
		private PlayerReferences _playerReferences;
		
		public PlayerStateMachine(PlayerReferences PlayerRefs) 
		{
			_playerReferences = PlayerRefs;
		}
		
		public PlayerBaseState CurrentState { get; private set; }

		public void Initialize(PlayerBaseState initialState) // Call this method in Player script during start to set the first state
		{
			CurrentState = initialState;
			CurrentState.Enter();
		}

		public void ChangeState(PlayerBaseState nextState) // Call whenever you need to change states
		{
			if (_playerReferences.PData.StateChangeDebug) { DebugStateChange(nextState); }
			
			CurrentState.Exit();
			CurrentState = nextState;
			CurrentState.Enter();
		}
		
		private void DebugStateChange(PlayerBaseState next) // Debugging what state changed to what state at what time
		{
			StackTrace stackTrace = new StackTrace(1);
			
			UnityEngine.Debug.Log("FROM --> " + stackTrace.GetFrame(1).GetMethod().ReflectedType.Name + " TO --> " + next + "   ||   [Time] " + Time.time);
		}
	}
}
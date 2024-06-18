using UnityEngine;

namespace Momentum 
{
	public class PlayerReferences : MonoBehaviour // A script that contains all our references to avoid having too many references in each file
	{ // Note that some scripts will still init their own references for ease of use
		[field: SerializeField] public PlayerData PData { get; private set; } // Plug this in from the inspector
		public Animator PAnimator { get; private set; }
		public SpriteRenderer PSprite { get; private set; }
		public Rigidbody2D PRB { get; private set; }
		public BoxCollider2D PBC { get; private set; }
		// public CapsuleCollider2D PBC { get; private set; }
		public PlayerChecks PChecks { get; private set; }

		private void Awake()
		{ // Set the references so they aren't null reference
			PAnimator = GetComponent<Animator>();
			PChecks = GetComponent<PlayerChecks>();
			PSprite = GetComponent<SpriteRenderer>();
			PBC = GetComponent<BoxCollider2D>();
			// PBC = GetComponent<CapsuleCollider2D>();
			PRB = GetComponent<Rigidbody2D>();
		}
	}
}
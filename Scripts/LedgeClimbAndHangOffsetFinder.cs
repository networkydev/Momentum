using UnityEngine;

namespace Momentum
{
	public class LedgeClimbAndHangOffsetFinder : MonoBehaviour // Helps you find your ledge hang and climb offets
	{
		private BoxCollider2D bc;
		[SerializeField] private GameObject player;
		[Tooltip("This is which corner you want the coords of")]
		[SerializeField] private Corner corner = Corner.topRight;
		private Vector2 cornerPosition;
		private Vector2 offsetOfPlayerToCorner;
		[Tooltip("Debug.Logs the offset value if set to true")]
		[SerializeField] private bool ShowOffsetValue = true;
		
		private void Awake()
		{
			bc = gameObject.GetComponent<BoxCollider2D>();
		}
		
		private void Update()
		{
			if (ShowOffsetValue) 
			{
				if (corner == Corner.topRight) 
				{
					cornerPosition = new Vector2(bc.bounds.center.x + bc.bounds.extents.x, bc.bounds.center.y + bc.bounds.extents.y);
					offsetOfPlayerToCorner = (Vector2)player.transform.position - cornerPosition;
					offsetOfPlayerToCorner.x *= -1;
				} else if (corner == Corner.topLeft)
				{
					cornerPosition = new Vector2(bc.bounds.center.x - bc.bounds.extents.x, bc.bounds.center.y + bc.bounds.extents.y);
					offsetOfPlayerToCorner = (Vector2)player.transform.position - cornerPosition;
				}
				
				Debug.Log(offsetOfPlayerToCorner);
			}
		}
		
		enum Corner 
		{
			topRight,
			topLeft, 
		}
	}
}
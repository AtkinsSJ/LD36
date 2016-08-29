using UnityEngine;
using System.Collections;

public class FloorBehaviour : MonoBehaviour
{
	public GameBoardBehaviour board;

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Ball"))
		{
			board.OnBallReachedBucket(collision.GetComponent<MarbleBehaviour>());
		}
	}
}

using UnityEngine;
using System.Collections;

public class BallSpawnerBehaviour : MonoBehaviour
{
	public GameBoardBehaviour board;
	public MarbleBehaviour ballPrefab;
	private AudioSource sound;

	public void Awake()
	{
		sound = GetComponent<AudioSource>();
	}

	// Pass in a ball to reuse it, or null to create a new one.
	public void SpawnBall(MarbleBehaviour existingBall)
	{
		MarbleBehaviour ball = existingBall ?? Instantiate(ballPrefab);

		ball.AppearAt(new Vector3(transform.position.x, transform.position.y + 0.00f, 0));
		//sound.Play();
	}

	//public void OnMouseUpAsButton()
	//{
	//	SpawnBall(null);
	//}
}

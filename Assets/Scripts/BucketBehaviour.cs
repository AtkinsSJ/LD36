using UnityEngine;
using System.Collections;

public class BucketBehaviour : MonoBehaviour
{
	public Sprite[] sprites;

	public GameBoardBehaviour board;
	public bool hasReceivedMarble = false;

	private Animator animator;
	private ParticleSystem particles;
	private AudioSource sound;

	private MarbleBehaviour caughtMarble;

	// Use this for initialization
	void Start()
	{
		animator = GetComponent<Animator>();
		animator.SetBool("isDown", false);

		particles = GetComponentInChildren<ParticleSystem>();
		sound = GetComponentInChildren<AudioSource>();
	}

	public void Init(int spriteNumber)
	{
		GetComponentInChildren<SpriteRenderer>().sprite = sprites[spriteNumber % sprites.Length];
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Ball"))
		{
			// We caught a ball!
			if (!hasReceivedMarble)
			{
				hasReceivedMarble = true;
				animator.SetBool("isDown", true);
				sound.Play();

				caughtMarble = collision.GetComponent<MarbleBehaviour>();
				caughtMarble.gameObject.SetActive(false);
			}
		}
	}

	public void OnDownAnimationFinished()
	{
		particles.Play();
		board.OnBallReachedBucket(caughtMarble);
	}
}

using UnityEngine;
using System.Collections;
using System;

public class MarbleBehaviour : MonoBehaviour
{
	private Rigidbody2D body;
	private Animator animator;

	// Use this for initialization
	void Awake ()
	{
		body = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void AppearAt(Vector3 spawnPos)
	{
		gameObject.SetActive(true);
		body.isKinematic = true;
		body.velocity = new Vector2();
		transform.position = spawnPos;
		animator.Play("Appear");
	}

	public void MakeKinematic()
	{
		body.isKinematic = false;
	}
}

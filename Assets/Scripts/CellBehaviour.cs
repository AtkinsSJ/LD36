using UnityEngine;

public enum CellType
{
	L,
	X,
	Straight,
	YLeft,
	YRight,
	CrowFoot,
	ThreeWay,
	DeadEnd,

	COUNT
}

// This is actually the WRAPPER for the cell.
public class CellBehaviour : MonoBehaviour
{
	// Inner cell! (The actual collision and sprite, etc)
	public Transform[] cellPrefabs;
	public CellType type;
	public ParticleSystem particlesPrefab;

	private int rotation = 0;
	private Transform childCell;
	private Animator animator;
	public bool isRotating;
	private ParticleSystem particles;
	private AudioSource sound;

	private bool mouseOver = false;

	public void Init(CellType myType)
	{
		this.type = myType;

		while (transform.childCount > 0)
		{
			DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
		}
		childCell = (Transform)Instantiate(cellPrefabs[(int)type], transform, false);
		particles = (ParticleSystem) Instantiate(particlesPrefab, new Vector3(transform.position.x, transform.position.y, 0.5f), Quaternion.identity, transform);
	}

	void Start()
	{
		animator = GetComponent<Animator>();
		childCell = GetComponentInChildren<SpriteRenderer>().transform;
		particles = GetComponentInChildren<ParticleSystem>();
		sound = GetComponentInChildren<AudioSource>();
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 0.5f);
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, 0.5f);
	}

	public void OnMouseEnter()
	{
		mouseOver = true;
	}

	public void OnMouseExit()
	{
		mouseOver = false;
	}

	private void Rotate(bool clockwise)
	{
		if (isRotating)
		{
			Debug.Log("Already rotating!");
			return;
		}
		else
		{
			Debug.Log("Starting rotation");
			isRotating = true;
			sound.Play();

			if (clockwise)
			{
				animator.SetTrigger("Rotate CW");
			}
			else
			{
				animator.SetTrigger("Rotate CCW");
			}
		}
	}

	public void DoneRotating()
	{
		Debug.Log("Ending rotation");
		isRotating = false;

		// Snap to a 60-degree increment because the animation is freaky
		float zRot = childCell.transform.rotation.eulerAngles.z;
		zRot = Mathf.Round(zRot / 60.0f) * 60f;
		childCell.transform.rotation = Quaternion.Euler(0, 0, zRot);

		particles.Play();
	}

	// Update is called once per frame
	void Update()
	{
		if (mouseOver)
		{
			if (Input.GetMouseButtonUp(0))
			{
				Rotate(false);
			}
			else if (Input.GetMouseButtonUp(1))
			{
				Rotate(true);
			}
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameBoardBehaviour : MonoBehaviour
{
	public int width = 10, height = 10;
	public GameObject cellPrefab;
	public new Camera camera;
	public float radius = 0.5f;

	public GameObject leftWallPrefab, rightWallPrefab;
	public GameObject floorPrefab, ceilingPrefab;

	public GameObject spawnerPrefab;
	public GameObject bucketPrefab;

	public BucketBehaviour[] buckets;
	public BallSpawnerBehaviour[] spawners;

	public bool isPlaying = false;
	public float playTime = 0;
	public Text playTimeText;
	public Text pedestalCountText;

	public GameObject overlay;
	public GameObject gameOver;
	public GameObject beforePlayingOverlay;
	public Text winTimeText;

	private AudioSource music;

	private void PutCell(float x, float y)
	{
		Vector3 position = new Vector3(x, y, 0);
		GameObject cell = (GameObject)Instantiate(cellPrefab, position, Quaternion.Euler(0, 0, Random.Range(0, 6) * 60f), this.transform);
		cell.GetComponent<CellBehaviour>().Init((CellType)Random.Range(0, (int)CellType.COUNT));
	}

	public void GenerateChildren()
	{
		// Clear existing children
		while (transform.childCount > 0)
		{
			DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
		}
		spawners = null;
		buckets = null;

		// Create new ones!
		float yOffset = radius * 2;
		float xOffset = Mathf.Sqrt(3 * radius * radius);

		int bucketSprite = 0;

		for (int x = 0; x < width; x++)
		{
			if (x % 2 == 1)
			{
				// Extra bottom tile every second column
				PutCell(x * xOffset, -radius);

				// Also, a spawner at the top
				Vector3 position = new Vector3(x * xOffset, (height + 0.5f) * yOffset, 2);
				Instantiate(spawnerPrefab, position, Quaternion.identity, this.transform);

				// Bucket at the bottom
				position = new Vector3(x * xOffset, -yOffset * 1.9f, -0.5f);
				GameObject bucket = (GameObject) Instantiate(bucketPrefab, position, Quaternion.identity, this.transform);
				bucket.GetComponentInChildren<BucketBehaviour>().Init(bucketSprite++);
			}
			else
			{
				// Floor!
				Vector3 position = new Vector3(x * xOffset, -yOffset, 2);
				Instantiate(floorPrefab, position, Quaternion.identity, this.transform);

				// Ceiling!
				position = new Vector3(x * xOffset, height * yOffset, 2);
				Instantiate(ceilingPrefab, position, Quaternion.identity, this.transform);
			}

			for (int y = 0; y < height; y++)
			{
				PutCell(x * xOffset, y * yOffset + ((x % 2 == 1) ? radius : 0));
			}
		}

		// Left wall
		for (int y = 0; y < height + 1; y++)
		{
			Vector3 position = new Vector3(-0.525f, y * yOffset - 0.5f, 2);
			Instantiate(leftWallPrefab, position, Quaternion.identity, this.transform);
		}

		// Right wall
		for (int y = 0; y < height + 1; y++)
		{
			Vector3 position = new Vector3(width * xOffset - 0.345f, y * yOffset - 0.5f, 2);
			Instantiate(rightWallPrefab, position, Quaternion.identity, this.transform);
		}

		// Center the camera
		camera.transform.position = new Vector3(3.015f, //(width - 4f) * xOffset * 0.5f,
			2.1f, //(height - 0.5f) * yOffset * 0.5f,
			-10);

		InitSpawnersAndBuckets();
	}

	void Start()
	{
		GenerateChildren();

		pedestalCountText.text = string.Format("{0}/{1}", 0, buckets.Length);
		RefreshTimer();
		overlay.SetActive(true);
		beforePlayingOverlay.SetActive(true);
		gameOver.SetActive(false);

		music = GetComponent<AudioSource>();
	}

	private void RefreshTimer()
	{
		int minutes = (int)playTime / 60;
		float seconds = playTime % 60f;
		playTimeText.text = string.Format("{0}:{1:00.00}", minutes, seconds);
		winTimeText.text = string.Format("You have won in {0}:{1:00.00}!", minutes, seconds);
	}

	public void Update()
	{
		if (isPlaying)
		{
			playTime += Time.deltaTime;

			RefreshTimer();
		}
	}

	public void BeginGame()
	{
		GenerateChildren();

		// start timer
		playTime = 0;

		// Clear marbles and release the first one
		var marbles = FindObjectsOfType<MarbleBehaviour>();
		foreach(var marble in marbles)
		{
			Destroy(marble.gameObject);
		}

		spawners[Random.Range(0, spawners.Length)].SpawnBall(null);

		isPlaying = true;
		overlay.SetActive(false);
	}

	private void InitSpawnersAndBuckets()
	{
		// Locate spawners and buckets
		spawners = GetComponentsInChildren<BallSpawnerBehaviour>();
		buckets = GetComponentsInChildren<BucketBehaviour>();

		foreach (var spawner in spawners)
		{
			spawner.board = this;
		}
		foreach (var bucket in buckets)
		{
			bucket.board = this;
		}
	}

	public void OnBallReachedBucket(MarbleBehaviour ball)
	{
		// Check if we've won!
		int fullBucketsCount = 0;
		foreach (var bucket in buckets)
		{
			if (bucket.hasReceivedMarble)
			{
				fullBucketsCount++;
			}
		}

		pedestalCountText.text = string.Format("{0}/{1}", fullBucketsCount, buckets.Length);

		if (fullBucketsCount == buckets.Length)
		{
			Debug.Log("YOU WINS!");
			isPlaying = false;
			overlay.SetActive(true);
			beforePlayingOverlay.SetActive(false);
			gameOver.SetActive(true);
		}
		else
		{
			Debug.Log("Not won yet. :(");
			spawners[Random.Range(0, spawners.Length)].SpawnBall(ball);
		}
	}

	public void OpenLudumDarePage()
	{
		Application.OpenURL("http://ludumdare.com/compo/ludum-dare-36/?action=preview&uid=1049");
	}

	public void ToggleAudio()
	{
		AudioListener al = Camera.main.GetComponent<AudioListener>();
		AudioListener.volume = AudioListener.volume > 0.5f ? 0 : 1;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameBoardBehaviour))]
public class GameBoardEditor : Editor
{
	SerializedProperty widthProp, heightProp;
	SerializedProperty radiusProp;

	void OnEnable()
	{
		widthProp = serializedObject.FindProperty("width");
		heightProp = serializedObject.FindProperty("height");
		radiusProp = serializedObject.FindProperty("radius");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("cellPrefab"), new GUIContent("Cell Prefab"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("camera"), new GUIContent("Camera"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("leftWallPrefab"), new GUIContent("Left Wall"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("rightWallPrefab"), new GUIContent("Right Wall"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnerPrefab"), new GUIContent("Spawner"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("bucketPrefab"), new GUIContent("Bucket"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("floorPrefab"), new GUIContent("Floor"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("ceilingPrefab"), new GUIContent("Ceiling"), false);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("playTimeText"), new GUIContent("Play Time Text"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("pedestalCountText"), new GUIContent("Pedestals Counter"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("overlay"), new GUIContent("Overlay"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("gameOver"), new GUIContent("Game Over Wrapper"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("beforePlayingOverlay"), new GUIContent("Before Playing Wrapper"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("winTimeText"), new GUIContent("Win Time Text"), false);

		EditorGUILayout.IntSlider(widthProp, 1, 20, new GUIContent("Width"));
		EditorGUILayout.IntSlider(heightProp, 1, 20, new GUIContent("Height"));

		EditorGUILayout.Slider(radiusProp, 0.1f, 2f, new GUIContent("Radius"));

		if (GUILayout.Button("Generate Children"))
		{
			((GameBoardBehaviour)target).GenerateChildren();
		}

		serializedObject.ApplyModifiedProperties();
	}
}
#endif
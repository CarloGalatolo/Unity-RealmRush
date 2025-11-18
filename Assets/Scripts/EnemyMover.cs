using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


[RequireComponent(typeof(Enemy))]
public class EnemyMover : MonoBehaviour
{
	// [SerializeField] List<Tile> path = new List<Tile>();

	[Tooltip("Measured in Tiles per second.")]
	[SerializeField][Range(0, 5)] float speed = 1;


	Enemy enemy;
	Pathfinder pathfinder;
	GridManager gridManager;



	void OnEnable()
	{
		// FindPath();	// Replaced by Pathfinder.cs.
		ReturnToStart();
		StartCoroutine(FollowPathRoutine());
	}


	void Awake()
	{
		enemy = GetComponent<Enemy>();	// Required.
		
		pathfinder = FindFirstObjectByType<Pathfinder>();
		Assert.IsNotNull(pathfinder, "EnemyMover.Awake(): pathfinder not found in scene");

		gridManager = FindObjectOfType<GridManager>();
		Assert.IsNotNull(gridManager, "EnemyMover.Awake(): gridManager not found in scene.");
	}


	// void FindPath()
	// {
	// 	path.Clear();

	// 	GameObject pathFolder = GameObject.FindGameObjectWithTag("Path");

	// 	foreach (Transform child in pathFolder.transform)
	// 	{
	// 		Tile waypoint = child.GetComponent<Tile>();

	// 		if (waypoint != null)
	// 		{
	// 			path.Add(waypoint);
	// 		}
	// 		else
	// 		{
	// 			Debug.LogWarning("Found a tile that's not a Wayponint inside the Path!");
	// 		}
	// 	}
	// }


	void GoalReached()
	{
		enemy.StealGold();
		gameObject.SetActive(false);
	}


	IEnumerator FollowPathRoutine()
	{
		foreach (Node node in pathfinder.Path)
		{
			Vector3 startPosition = this.transform.position;
			Vector3 endPosition = gridManager.GetPositionFromCoordinates(node.coordinates);
			float travelPercent = 0;

			transform.LookAt(endPosition);

			while (travelPercent <= 1)
			{
				travelPercent += speed * Time.deltaTime;
				this.transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
				yield return null;
			}
		}

		GoalReached();
	}


	void ReturnToStart()
	{
		if (pathfinder.Path.Count == 0)
		{
			return;
		}
		
		// transform.position = path[0].transform.position;
		transform.position = gridManager.GetPositionFromCoordinates(pathfinder.Path[0].coordinates);
	}
}
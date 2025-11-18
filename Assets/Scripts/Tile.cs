using UnityEngine;
using UnityEngine.Assertions;



public class Tile : MonoBehaviour
{
	[SerializeField] Tower tower;

	[SerializeField] bool isPlaceable;
	public bool IsPlaceable => isPlaceable;

	GridManager gridManager;
	Pathfinder pathfinder;

	Vector2Int coordinates = new Vector2Int();



	void Awake()
	{
		Assert.IsNotNull(tower, "Tile.Awake(): tower not found in prefab.");

		gridManager = FindObjectOfType<GridManager>();
		Assert.IsNotNull(gridManager, "Tile.Awake(): gridManager not found in scene.");

		pathfinder = FindFirstObjectByType<Pathfinder>();
		Assert.IsNotNull(pathfinder, "Tile.Awake(): pathfinder not found in scene");
	}


	void Start()
	{
		if (gridManager != null)
		{
			coordinates = gridManager.GetCoordinatesFromPosition(this.transform.position);

			if (!isPlaceable)
               {
				gridManager.BlockNode(coordinates);
               }
		}
	}


	void OnMouseDown()
	{
		if (!(gridManager.TryGetNode(coordinates) is Node node) || !node.isWalkable || pathfinder.WillBlockPath(coordinates))
		{
			return;
		}

		bool isPlaced = tower.CreateTower(transform.position);
		isPlaceable = !isPlaced;
		gridManager.BlockNode(coordinates);
	}
}

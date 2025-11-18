using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
	[SerializeField] Vector2Int gridSize;
	[Tooltip("Should match UnityEditor.EditorSnapSettings.")]
	[SerializeField] int unityGridSize = 10;
     public int UnityGridSize => unityGridSize;

	Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
	public Dictionary<Vector2Int, Node> Grid { get { return grid; } }


     void Awake()
	{
		CreateGrid();
	}


	void CreateGrid()
	{
		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2Int coordinates = new Vector2Int(x, y);
				Node node = new Node(coordinates, true);
				grid.Add(coordinates, node);

				// Debug.Log($"{node.coordinates} isPath = {node.isPath}");
			}
		}
	}


	// public void BlockNode(Vector2Int coordinates)
	// {
	// 	if (grid.ContainsKey(coordinates))
	// 	{
	// 		grid[coordinates].isWalkable = false;
	// 	}
	// }
	public void BlockNode(Vector2Int coordinates)
	{
		if (TryGetNode(coordinates) is Node node)
		{
			node.isWalkable = false;
		}
	}


	public void BlockNode(Vector3 position)
	{
		BlockNode(GetCoordinatesFromPosition(position));
	}


	public void ResetNodes()
	{
		foreach (KeyValuePair<Vector2Int, Node> entry in grid)
		{
			entry.Value.connectedTo = null;
			entry.Value.isExplored = false;
			entry.Value.isPath = false;
		}
	}


	public Vector2Int GetCoordinatesFromPosition(Vector3 position)
	{
		Vector2Int coordinates = new Vector2Int();

		coordinates.x = Mathf.RoundToInt(position.x / UnityGridSize);
		coordinates.y = Mathf.RoundToInt(position.z / UnityGridSize);

		return coordinates;
	}
	

	public Vector3 GetPositionFromCoordinates(Vector2Int coordinates)
     {
		Vector3 position = new Vector3();

		position.x = coordinates.x * UnityGridSize;
		position.z = coordinates.y * UnityGridSize;

		return position;
     }


	// L'ho fatta io questa funzione?
	public Node TryGetNode(Vector2Int coordinates)
	{
		if (!grid.ContainsKey(coordinates))
		{
			return null;
		}

		return grid[coordinates];
	}
}

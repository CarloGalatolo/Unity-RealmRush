using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;


public class Pathfinder : MonoBehaviour
{
	[SerializeField] Vector2Int startCoordinates;
	[SerializeField] Vector2Int destinationCoordinates;

	GridManager gridManager;
	Node startNode;
	Node destinationNode;
	Node currentSearchNode;

	Queue<Node> frontier = new Queue<Node>();
	Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

	// State
	readonly Vector2Int[] DIRECTIONS = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
     List<Node> path = new List<Node>();     // Per la versione di BuildPath() personalizzata ricosriva.
	public List<Node> Path => path;


	void Awake()
	{
		gridManager = FindObjectOfType<GridManager>();
		Assert.IsNotNull(gridManager, "Pathfinder.Awake(): gridManager not found in the scene.");
	}


	void Start()
	{
		startNode = gridManager.TryGetNode(startCoordinates);
		destinationNode = gridManager.TryGetNode(destinationCoordinates);
		
		GetNewPath();
	}


	// public List<Node> GetNewPath()
	// {
	// 	gridManager.ResetNodes();
	// 	BreadthFirstSearch();
	// 	return BuildPath();
	// }


	// Ricorsiva
	public void GetNewPath()
	{
		path.Clear();
		gridManager.ResetNodes();
		currentSearchNode = startNode;
		BreadthFirstSearch();
		BuildPath(path, destinationNode);
	}


	void BreadthFirstSearch()
	{
		frontier.Clear();
		reached.Clear();

		bool isRunning = true;

		frontier.Enqueue(currentSearchNode);
		reached.Add(startCoordinates, currentSearchNode);

		while (frontier.Count > 0 && isRunning)
		{
			currentSearchNode = frontier.Dequeue();
			currentSearchNode.isExplored = true;
			ExploreNeighbors();
			if (currentSearchNode.coordinates == destinationCoordinates)
			{
				isRunning = false;
			}
		}
	}


	void ExploreNeighbors()
	{
		List<Node> neighbors = new List<Node>();

		foreach (Vector2Int direction in DIRECTIONS)
		{
			Node neighbor = gridManager.TryGetNode(currentSearchNode.coordinates + direction);

			if (neighbor != null)
			{
				neighbors.Add(neighbor);
				neighbor.isExplored = true;
				// currentSearchNode.isPath = true;	// Pathfinder.cs must find the path, not this.
			}
		}

		foreach (Node neighbor in neighbors)
		{
			if (!reached.ContainsKey(neighbor.coordinates) && neighbor.isWalkable)
			{
				neighbor.connectedTo = currentSearchNode;
				reached.Add(neighbor.coordinates, neighbor);
				frontier.Enqueue(neighbor);
			}
		}
	}


	// Versione del corso, iterativa.
	List<Node> BuildPath()
	{
		List<Node> path = new List<Node>();
		Node currentNode = destinationNode;

		path.Add(currentNode);
		currentNode.isPath = true;

		while (currentNode.connectedTo != null)
		{
			currentNode = currentNode.connectedTo;

			path.Add(currentNode);
			currentNode.isPath = true;
		}

		path.Reverse();

		return path;
	}


	/// <summary>
	/// Versione ricorsiva personalizzata.
	/// </summary>
	/// <param name="outPath">Out parameter, il membro path di questa classe.</param>
	/// <param name="destination">Nodo destinazione, passare destinationNode per il primo ciclo ricorsivo.</param>
	void BuildPath(List<Node> outPath, Node destination)
	{
		Node currentNode = destination;
		outPath.Insert(0, currentNode);	// Adds as first element, shifting all other elements forward. This way I don't need to reverse the list afterwards.
		currentNode.isPath = true;

		if (currentNode.connectedTo is Node next)
		{
			BuildPath(outPath, next);
		}
	}


	public bool WillBlockPath(Vector2Int coordinates)
	{
		if (!gridManager.Grid.ContainsKey(coordinates))
		{
			return false;
		}

		bool previousIsWalkable = gridManager.Grid[coordinates].isWalkable;

		gridManager.Grid[coordinates].isWalkable = false;
		GetNewPath();
		gridManager.Grid[coordinates].isWalkable = previousIsWalkable;

		if (path.Count <= 1)
		{
			GetNewPath();
			return true;
		}

		return false;
	}
}
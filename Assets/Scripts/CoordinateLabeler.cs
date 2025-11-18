using TMPro;
using UnityEngine;
using UnityEngine.Assertions;


/// <summary>
/// WARNING: This script uses some Editor functionalities. Move it to an Editor folder if you want to build an executable. It's not really necessary in game anyway.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class CoordinateLabeler : MonoBehaviour
{
	[SerializeField] Color defaultColor = Color.white;
	[SerializeField] Color blockedColor = Color.gray;
	[SerializeField] Color exploredColor = Color.yellow;
	[SerializeField] Color pathColor = new Color(1, 0.5f, 0);	// Orange.

	GridManager gridManager;
	TMP_Text label;

	string tileName;
	Vector2Int coordinates = new Vector2Int();



	void Awake()
	{
		UnityEditor.EditorSnapSettings.move = new Vector3(10, 10, 10);

		gridManager = FindObjectOfType<GridManager>();
		Assert.IsNotNull(gridManager, "CoordinateLabel.Awake(): gridManager not found in the scene.");

		label = GetComponent<TMP_Text>();
		Assert.IsNotNull(label, "CoordinateLabel.Awake(): label not found in prefab.");

		// label.enabled = !Application.isPlaying;

		DisplayCoordinates();    // Call only once at game start but update only in editor.
	}


	void Start()
	{
		tileName = GetComponentInChildren<TileName>().Name;
	}


	void Update()
	{
		// if (!Application.isPlaying)   // Execute only in Editor mode.
		// {
			DisplayCoordinates();
			UpdateName();
		// }

		SetLabelColor();
		ToggleLabels();
	}


	void DisplayCoordinates()
	{
		if (gridManager == null)
		{
			return;
		}
		
		coordinates.x = Mathf.RoundToInt(transform.parent.position.x / gridManager.UnityGridSize);
		coordinates.y = Mathf.RoundToInt(transform.parent.position.z / gridManager.UnityGridSize);
		label.text = coordinates.ToString();
	}


	void UpdateName()
	{
		transform.parent.name = coordinates.ToString() + " " + tileName + "Tile";
	}


	void ToggleLabels()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			label.enabled = !label.IsActive();
		}
	}


	void SetLabelColor()
	{
		if (gridManager == null)
		{
			return;
		}

		Node node = gridManager.TryGetNode(coordinates);

		if (node == null)
		{
			return;
		}

		if (!node.isWalkable)
		{
			label.color = blockedColor;
		}
		else if (node.isPath)
		{
			label.color = pathColor;
		}
		else if (node.isExplored)
		{
			label.color = exploredColor;
		}
		else
		{
			label.color = defaultColor;
		}
	}
}

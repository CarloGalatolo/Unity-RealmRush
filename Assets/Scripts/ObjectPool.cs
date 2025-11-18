using System.Collections;
using UnityEngine;


public class ObjectPool : MonoBehaviour
{
	[SerializeField] GameObject enemyPrefab;
	[SerializeField][Range(0, 50)] uint poolSize = 5;
	[SerializeField][Range(0.1f, 30)] float spawnTimer = 1;

	GameObject[] pool;



	void Awake()
	{
		PopulatePool();
	}


	void Start()
	{
		// StartCoroutine(SpawnEnemyRoutine());
	}


	void PopulatePool()
	{
		pool = new GameObject[poolSize];

		for (int i = 0; i < pool.Length; i++)
		{
			pool[i] = Instantiate(enemyPrefab, this.transform);
			pool[i].SetActive(false);
		}
	}
	

	void EnableObjectInPool()
	{
		foreach (Transform child in this.transform)
		{
			if (!child.gameObject.activeInHierarchy)
			{
				child.gameObject.SetActive(true);
				return;
			}
		}
	}


	IEnumerator SpawnEnemyRoutine()
	{
		float timer = 0;

		while (true)
		{
			timer += Time.deltaTime;

			if (timer >= spawnTimer)
			{
				timer = 0;

				EnableObjectInPool();
			}

			yield return null;
		}
	}
}

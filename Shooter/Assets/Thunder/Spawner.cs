using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private Transform parent;

	public float maxSize, minSize;
	public float maxDuration;

	private List<GameObject> spawnedObjs;
	
	private void Start()
	{
		spawnedObjs = new List<GameObject>();
	}

	public void TriggerAction()
	{
		GameObject obj = Instantiate(prefab, transform);
		obj.transform.position = transform.position;
		float size = Random.Range(minSize,maxSize);
		obj.transform.localScale = Vector3.one * size;
		spawnedObjs.Add(obj);
	}

	public void Despawn()
	{
		GameObject lastObj = spawnedObjs[0];
		lastObj.GetComponent<ActionHandler>().TriggerEvent.Invoke();
		lastObj.transform.SetParent(parent);
		spawnedObjs.Remove(lastObj);

		StartCoroutine(RemoveObj(lastObj, Random.Range(1, maxDuration)));
	}

	IEnumerator RemoveObj(GameObject objToDestroy, float secs)
	{
		float count = 0;
		float startScale = objToDestroy.transform.localScale.x;
		while (count < secs)
		{
			float scale = Mathf.Lerp(startScale, 0, count / secs);
			//Debug.Log(scale);
			objToDestroy.transform.localScale = Vector3.one * scale;
			count += Time.deltaTime;
			yield return null;
		}
		Destroy(objToDestroy);
		yield return 0;
	}
}

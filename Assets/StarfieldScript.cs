using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarfieldScript : MonoBehaviour {
	public GameObject star;
	public int count = 100;

	void Start() {
		Bounds bounds = GetComponent<Collider>().bounds;
		Vector3 max = bounds.max;
		Vector3 min = bounds.min;

		for (int i = 0; i < count; i++) {
			GameObject newStar = Instantiate(star, bounds.min + new Vector3(bounds.extents.x * Random.value, bounds.extents.y * Random.value, bounds.extents.z * Random.value) * 2, Quaternion.identity);
			newStar.transform.parent = transform;
		}
	}
}

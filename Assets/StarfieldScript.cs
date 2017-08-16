using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarfieldScript : MonoBehaviour {
	public GameObject Star;
	public int count = 100;
	public float speed = 5;

	Bounds bounds;
	List<Transform> stars;

	void Start() {
		bounds = GetComponent<Collider>().bounds;

		stars = new List<Transform>();

		for (int i = 0; i < count; i++) {
			GameObject star = Instantiate(Star, bounds.min + new Vector3(bounds.extents.x * Random.value, bounds.extents.y * Random.value, bounds.extents.z * Random.value) * 2, Quaternion.identity);
			star.transform.parent = transform;
			stars.Add(star.transform);
		}
	}

	void Update() {
		foreach (Transform star in stars) {
			star.position -= new Vector3(0, 0, speed);
			if (star.position.z < bounds.min.z) {
				star.position += new Vector3(0, 0, bounds.extents.z * 2);
			}
		}
	}
}

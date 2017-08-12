using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAnimationScript : MonoBehaviour {
	public void Kill() {
		GetComponent<Animator>().SetBool("dead", true);
	}

	public bool isDead() {
		return GetComponent<Animator>().GetBool("dead");
	}
}

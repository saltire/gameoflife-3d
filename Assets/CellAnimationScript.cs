using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAnimationScript : MonoBehaviour {
	Animator animator;

	public void SkipToFull() {
		GetComponent<Animator>().SetBool("full", true);
	}

	void Start() {
		animator = GetComponent<Animator>();
	}

	public void Kill() {
		animator.SetBool("dead", true);
	}

	public bool IsDead() {
		return animator.GetBool("dead");
	}
}

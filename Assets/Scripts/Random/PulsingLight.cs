using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingLight : MonoBehaviour {

  public float animSpeed = 1f;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Animator>().speed = animSpeed;
	}

	// Update is called once per frame
	void Update () {

	}
}

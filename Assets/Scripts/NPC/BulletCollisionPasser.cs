using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollisionPasser : MonoBehaviour {

  public Bullet parent;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

  void OnTriggerEnter(Collider other){
    parent.OnChildTriggerEnter(other, GetComponent<Collider>());
  }
}

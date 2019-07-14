using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour {

  public bool grounded = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

  void OnTriggerEnter(Collider other){
    if (other.tag != "Player" && other.tag != "Bullet"){
      grounded = true;
      if(other.tag == "Platform"){
        transform.parent.transform.parent = other.transform.parent;
      }
    }
  }

  void OnTriggerExit(Collider other){
    if (other.tag != "Player" && other.tag != "Bullet"){
      if(other.tag == "Environment" || other.tag == "Platform"){
        grounded = false;
      }
      if(other.tag == "Platform"){
        transform.parent.transform.parent = null;
      }
    }
  }
}

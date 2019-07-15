using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour {

  public bool isGrounded = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

  void OnTriggerEnter(Collider other){
    if (other.tag != "Player"){
      isGrounded = true;
    }
  }

  void OnTriggerExit(Collider other){
    if (other.tag != "Player"){
      if(other.tag == "Environment" || other.tag == "Gravity"){
        isGrounded = false;
      }
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
Will rotate around an anchor point probably above the players head. Will do a
sort of spiral.
*/
public class PlayerCamera : MonoBehaviour {

  public Transform anchor; // focus camera on this point always
  public float sensitivity = 0.1f;
  public float scaleY = 1f; // change height cam can do
  public float scaleZ = 1f; // determines distance behind

  // what portion cam can go each extreme
  public float maxUp = 0.9f;
  public float maxDown = 0.8f;

  private float theta;

	// Use this for initialization
	void Start () {
        theta = 2*Mathf.PI; // initial rotation value

        // put camera behind anchor (at distance)
        transform.rotation = anchor.rotation;
        transform.position = anchor.position;
        transform.Translate(0,0,-scaleZ*theta*Mathf.Cos(theta));
	}

	// Update is called once per frame
	void Update () {
        // update position on path based on mouse movement
        float dTheta = -Input.GetAxis("Mouse Y")*sensitivity;
        theta += dTheta;

        // limit rotation
        if(theta > (2*Mathf.PI) + (maxUp*(Mathf.PI/2))){
          theta = (2*Mathf.PI) + (maxUp*(Mathf.PI/2));
        }else if(theta < (2*Mathf.PI) - (maxDown*(Mathf.PI/2))){
          theta = (2*Mathf.PI) - (maxDown*(Mathf.PI/2));
        }

        float y = scaleY*theta*Mathf.Sin(theta);
        float z = -scaleZ*theta*Mathf.Cos(theta);

        // set new position
        transform.rotation = anchor.rotation;
        transform.position = anchor.position;
        transform.Translate(0,Mathf.Max(y, -2.5f),z);

        // always look at anchor
        transform.LookAt(anchor, anchor.up);
    }
}

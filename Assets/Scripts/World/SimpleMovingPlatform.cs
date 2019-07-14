using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovingPlatform : MonoBehaviour {

  public Transform point1;
  public Transform point2;
  public float speed;

  private Vector3 origin;
  private Vector3 target;
  private float progress = 0f;
  private float distance; // total distance between points

	// Use this for initialization
	void Start () {
    origin = point1.position;
    target = point2.position;
    distance = Vector3.Distance(origin, target);
	}

	// Update is called once per frame
	void Update () {
    Vector3 pos = Vector3.Lerp(origin, target, progress);
    transform.position = pos;

    progress += Time.deltaTime * speed / distance;
    // made it to target, switch
    if(progress > 1f){
      progress = 0f;
      // swap
      Vector3 tmp = origin;
      origin = target;
      target = tmp;
    }
	}
}

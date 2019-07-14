using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

  public float speed;
  public float damage;
  public Vector3 direction;

  public MeshRenderer body;

	// Use this for initialization
	void Start () {
    // face to target
    transform.rotation = Quaternion.LookRotation(direction);

    Color[] blColors = new Color[6];
    blColors[0] = Color.blue;
    blColors[1] = Color.cyan;
    blColors[2] = Color.green;
    blColors[3] = Color.magenta;
    blColors[4] = Color.red;
    blColors[5] = Color.yellow;

    body.material.SetColor("_Color", blColors[Random.Range(0, blColors.Length)]);
	}

	// Update is called once per frame
	void Update () {
    // move to target
    transform.Translate(Vector3.Normalize(direction)*speed*Time.deltaTime, Space.World);
	}

  public void OnChildTriggerEnter(Collider other, Collider child){
    if(other.tag == "Player" && other.GetComponent<Health>() != null){
      other.GetComponent<Health>().curHealth -= damage;
    }
    child.enabled = false; // so cant hit more
    child.GetComponent<MeshRenderer>().enabled = false;
  }
}

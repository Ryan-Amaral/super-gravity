using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAgent : MonoBehaviour{

    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start(){
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){

    }

    void FixedUpdate(){
        Vector3 gravityVector = GravityField.instance.GetGravity(transform.position);
        rigidbody.AddForce(gravityVector*rigidbody.mass);
        Debug.Log(gravityVector);
    }
}

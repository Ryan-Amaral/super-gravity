using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAgent : MonoBehaviour{

    // Start is called before the first frame update
    void Start(){

    }

    // Update is called once per frame
    void Update(){

    }

    void FixedUpdate(){
        Vector3 gravityVector = GravityField.instance.GetGravity(transform.position);
        GetComponent<Rigidbody>().AddForce(gravityVector);
        //Debug.Log(gravityVector);
    }
}

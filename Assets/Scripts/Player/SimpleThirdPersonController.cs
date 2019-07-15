using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleThirdPersonController : MonoBehaviour
{

    public float speed = 5f; // walking around movement speed
    public float jumpPower = 5f;
    public float airMovement = 0.5f; // discount to movement speed when airborn

    public GroundChecker groundChecker;

    // Start is called before the first frame update
    void Start(){

    }

    // Update is called once per frame
    void Update(){

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        Vector3 movement = Vector3.ClampMagnitude(new Vector3(x,0,z), speed);

        if(groundChecker.isGrounded){
            transform.Translate()
        }
        // do jump
        if(groundChecker.isGrounded && Input.GetKeyDown(KeyCode.Space)){
            rigidbody.AddForce(Vector3.up*jumpPower);
        }

        // move

    }
}

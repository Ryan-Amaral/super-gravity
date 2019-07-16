using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleThirdPersonController : MonoBehaviour
{

    public float speed = 5f; // walking around movement speed
    public float jumpPower = 5f;
    public float airMovement = 0.5f; // discount to movement speed when airborn
    public float rotateSensitivity = 0.5f; // player rotation speed

    public GroundChecker groundChecker;
    public GravityAgent gravityAgent;

    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start(){
        rigidbody = GetComponent<Rigidbody>();
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update(){
        // release or trap mouse
        if(Input.GetKeyDown(KeyCode.Escape)){
            //Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }else if(Input.GetMouseButtonDown(0)){
            //Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        Move();
        Rotate();
    }

    /*
    Walking and jumping.
    */
    void Move(){
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;
        Vector3 movement = Vector3.ClampMagnitude(
                (new Vector3(x,0,z))*Time.deltaTime, speed);

        if(groundChecker.isGrounded){
            // do jump
            if(Input.GetKeyDown(KeyCode.Space)){
                rigidbody.AddForce((Vector3.up+movement)*jumpPower);

                Debug.Log("JUmp");
            }
        }else{
            movement *= airMovement; // discount movement when airborn
        }

        transform.Translate(movement);
    }

    /*
    Rotates the player according to gravity vector, and then user rotation.
    */
    void Rotate(){
        // be in line with gravity vector
        Vector3 gravVec = gravityAgent.gravityVector;
        Vector3 myForward = Quaternion.AngleAxis(90, transform.right) * gravityAgent.gravityVector;;

        transform.LookAt(new Vector3(transform.position.x,
                                     transform.position.y + myForward.y,
                                     transform.position.z + myForward.z),
                         -gravVec);

        // rotate from player input
        transform.Rotate(transform.up * Input.GetAxis("Mouse X") * rotateSensitivity);
    }
}

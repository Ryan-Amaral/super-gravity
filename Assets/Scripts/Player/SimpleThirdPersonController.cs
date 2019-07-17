using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleThirdPersonController : MonoBehaviour
{

    public float speed = 5f; // walking around movement speed
    public float jumpPower = 5f;
    public float airMovement = 0.5f; // discount to movement speed when airborn
    public float rotateSensitivity = 0.5f; // player rotation speed
    public float gravityRotateSpeed = 0.1f; // speed to align to gravity vector

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
                rigidbody.AddForce((transform.up+movement)*jumpPower);
            }
        }else{
            movement *= airMovement; // discount movement when airborn
        }

        transform.Translate(movement);
    }

    /*
    Aligns the player's down axis with the gravity vector without modifying the
    player's x rotation, and then modifies the x rotation based on input.
    */
    void Rotate(){
        Vector3 gravVec = gravityAgent.gravityVector;

        float targetRotX = Vector3.SignedAngle(-transform.up, gravVec, transform.right);
        float targetRotZ = Vector3.SignedAngle(-transform.up, gravVec, transform.forward);

        float rotX = Mathf.SmoothStep(0f, targetRotX, gravityRotateSpeed);
        float rotY = Input.GetAxis("Mouse X") * rotateSensitivity;
        float rotZ = Mathf.SmoothStep(0f, targetRotZ, gravityRotateSpeed);

        transform.Rotate(rotX, rotY, rotZ);
    }
}

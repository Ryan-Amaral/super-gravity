using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

  public float rotateSensitivity = 0.5f;

  // things needed for movement of player
  public float speed = 5.0f;
  public float jumpPower = 8.0f;
  public float jetPackPower = 0.21f;
  public float jetPackLength = 1.5f;
  public GroundChecker groundChecker;
  public ParticleSystem jetPackExhaust;
  private Rigidbody rb;
  private bool jetPacking = false;
  private float jpUse = 0f;

  // Things needed for gun stuff
  public Transform realBulletSpawn;
  public Transform visualBulletSpawn;
  public Transform camera;
  public MeshRenderer bulletLine;
  public float bulletLineTime = 0.05f;
  public float damage = 25f;
  public GameObject hitMarkerVis;
  private float hitMarkerTime = 0f;
  //private bool readyToFire = true;
  private float bulletLineCountDown = 0f;
  private float bll = 50f; // bullet line length
  private Color[] blColors;

	// Use this for initialization
	void Start () {
    rb = GetComponent<Rigidbody>();
    jetPackExhaust.Stop();

    bulletLine.gameObject.transform.localScale = new Vector3(0.1f, bll, 0.05f);

    blColors = new Color[6];
    blColors[0] = Color.blue;
    blColors[1] = Color.cyan;
    blColors[2] = Color.green;
    blColors[3] = Color.magenta;
    blColors[4] = Color.red;
    blColors[5] = Color.yellow;
	}

	// Update is called once per frame
	void Update () {
    // first deal with player rotation
    transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotateSensitivity);

    // deal with walking, jumping, jetpacking
    PlayerMovement();

    // deal with gun stuff
    DoGunStuff();
	}

  void PlayerMovement(){
    // can do flat movement anytime
		float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
    float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
    Vector3 movement = Vector3.ClampMagnitude(new Vector3(x,0,z), speed);
    transform.Translate(movement);

    if(groundChecker.grounded && !jetPacking && Input.GetKeyDown(KeyCode.Space) &&
        gameObject.GetComponent<Rigidbody>().velocity.y <= 0.1f){
      // do a jump
      rb.AddForce(new Vector3(0,jumpPower,0), ForceMode.Impulse);
      jpUse = jetPackLength;
    }else if(!groundChecker.grounded && Input.GetKeyDown(KeyCode.Space)){
      // start using jet pack
      jetPacking = true;
      if(jpUse > 0f){
        jetPackExhaust.Play();
      }
    }else if(groundChecker.grounded && jetPacking && !Input.GetKey(KeyCode.Space)){
      // stop jet pack
      jetPacking = false;
      jpUse = jetPackLength;
    }else if(jetPacking && jpUse > 0){
      // continue to jet pack
      if(Input.GetKey(KeyCode.Space)){
        rb.AddForce(new Vector3(0,jetPackPower,0), ForceMode.Impulse);
        jpUse -= Time.deltaTime;
        if(jpUse < 0f){
          jetPacking = false;
          jetPackExhaust.Stop();
        }
      }else{
        jetPackExhaust.Stop();
      }
    }
  }

  void DoGunStuff(){
    if(Input.GetMouseButtonDown(0)){
      Vector3 hitPos = fireBullet();
      showBulletLine(hitPos);
    }

    if(bulletLineCountDown > 0){
      bulletLineCountDown -= Time.deltaTime;
      if(bulletLineCountDown < 0){
        bulletLine.enabled = false;
      }
    }

    if(hitMarkerTime > 0){
      hitMarkerTime -= Time.deltaTime;
      if(hitMarkerTime <= 0){
        hitMarkerVis.SetActive(false);
      }
    }
  }

  // returns position of hit
  Vector3 fireBullet(){
    RaycastHit[] hits =
      Physics.RaycastAll(realBulletSpawn.position,
        (realBulletSpawn.position - camera.position),
        60f,
        ~(1<<9));

    if(hits.Length > 0){
      RaycastHit closestHit = hits[0];
      float closestDist = Vector3.Distance(hits[0].point, realBulletSpawn.position);

      // get closest
      for(int i = 1; i < hits.Length; i++){
        float dist = Vector3.Distance(hits[i].point, realBulletSpawn.position);
        if(dist < closestDist){
          closestHit = hits[i];
          closestDist = dist;
        }
      }
      // damage closest if applicable
      GameObject gobj = closestHit.transform.gameObject;
      if(gobj.tag == "Enemy"){
        gobj.GetComponent<Health>().curHealth -= damage;
        HitMark();
      }

      return closestHit.point;
    }
    // otherwise just make point a distance out
    else{
      // return point bll out
      return realBulletSpawn.position + Vector3.Normalize(realBulletSpawn.position - camera.position)*bll;
    }
  }

  // show bullet line as random color lazer
  void showBulletLine(Vector3 hitPos){
    setBulletLinePosition(hitPos);
    bulletLine.enabled = true;
    bulletLine.material.SetColor("_Color", blColors[Random.Range(0, blColors.Length)]);
    bulletLineCountDown = bulletLineTime;
  }

  void setBulletLinePosition(Vector3 hitPos){
    // make bullet line come from gun, and go to target hit
    bulletLine.gameObject.transform.position = visualBulletSpawn.position;
    bulletLine.gameObject.transform.LookAt(hitPos);
    bulletLine.gameObject.transform.Rotate(Vector3.right*90);
    float dist = Vector3.Distance(hitPos, visualBulletSpawn.position);
    bulletLine.gameObject.transform.Translate(Vector3.up*(dist/2));
    bulletLine.transform.localScale = new Vector3(0.2f, dist/2, 0.1f);
  }

  // signify a hit by showing the hitmarker and a noise
  void HitMark(){
    hitMarkerVis.SetActive(true);
    hitMarkerTime = 0.1f;
    // and do noise
  }
}

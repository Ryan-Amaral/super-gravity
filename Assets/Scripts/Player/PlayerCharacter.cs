using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour {

  public Health pHealth;
  public Transform spawnPoint;
  public Transform checkPointSpawn;
  public GameObject overlay;
  public AudioClip deathSound;

  private bool dead;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
    if(pHealth.curHealth <= 0 && !dead){
      dead = true;
      StartCoroutine(Die());
    }

    if(transform.position.y < -20){
      pHealth.curHealth = 0;
    }
	}

  IEnumerator Die(){
    // show red overlay
    overlay.SetActive(true);
    gameObject.GetComponent<PlayerControl>().enabled = false;
    // play death sound
    //audio.PlayOneShot(deathSound);

    yield return new WaitForSeconds(3);

    // stop red overlay
    overlay.SetActive(false);

    // respawn
    transform.position = spawnPoint.position;
    transform.eulerAngles = spawnPoint.eulerAngles;

    GameManager.UnkillEnemies();

    pHealth.curHealth = pHealth.maxHealth;
    gameObject.GetComponent<PlayerControl>().enabled = true;
    dead = false;
  }

  void OnTriggerEnter(Collider other){
    // die if touch death floor instantly
    if(other.tag == "DeathFloor"){
      pHealth.curHealth = 0;
    }

    // go to next level if touch portal
    if(other.tag == "Portal"){
      SceneManager.LoadScene("lvl2");
    }

    // player made it to CheckPoint
    if(other.tag == "CheckPoint" && spawnPoint != checkPointSpawn){
      spawnPoint = checkPointSpawn;
      pHealth.curHealth = pHealth.maxHealth;
    }
  }
}

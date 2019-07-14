using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

  public static GameObject Player;
  public GameObject player;
  private static GameObject[] enemies;

  void Awake(){
    Player = player;
  }

	// Use this for initialization
	void Start () {
    Cursor.lockState = CursorLockMode.Locked;
    enemies = GameObject.FindGameObjectsWithTag("Enemy");
	}

	// Update is called once per frame
	void Update () {

	}

  public static void UnkillEnemies(){
    for(int i = 0; i < enemies.Length; i++){
      GameObject enemy = enemies[i];
      enemy.GetComponent<Health>().curHealth = enemy.GetComponent<Health>().maxHealth;
      enemy.SetActive(true);
    }
  }
}

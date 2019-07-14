using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

  public float speed; // speed of movement
  public float attackRange; // distance to player before can attack
  public float wanderRange; // distance to wander from origin
  public float heightRange; // amount to vary heigh
  public float shotDelay;
  public Bullet bullet;
  public Health health;
  public ParticleSystem explosion;

  private Vector3 origin;

  private Vector3 curDirection;

  private float shotTimer;

  private float deadTimer = -1f;

	// Use this for initialization
	void Start () {
    origin = transform.position;
    curDirection = Vector3.Normalize(new Vector3(Random.Range(-1,1),
                                                 Random.Range(-1,1),
                                                 Random.Range(-1,1)));
	}

	// Keep on moving until out of range
	void Update () {
    Move();

    GameObject player = GameManager.Player;
    if(player != null && shotTimer <= 0 &&
        Vector3.Distance(player.transform.position, transform.position) < attackRange){
      Shoot(player.transform.position);
    }

    if(shotTimer > 0){
      shotTimer -= Time.deltaTime;
    }

    if(health.curHealth <= 0 && deadTimer < 0){
      Die();
    }

    if(deadTimer > 0){
      deadTimer -= Time.deltaTime;
      if(deadTimer <= 0){
        gameObject.SetActive(false);
      }
    }
	}

  void Shoot(Vector3 target){
    Vector3 shotDirection = Vector3.Normalize(target - transform.position);
    Bullet newBullet = (Bullet)Instantiate(bullet, transform.position, new Quaternion());
    Destroy(newBullet.gameObject, 2); // destroy bullet after 2s if no collision
    newBullet.direction = shotDirection;
    shotTimer = shotDelay;
  }

  void Move(){
    float distOrigin = Vector2.Distance(new Vector2(origin.x, origin.z),
                                        new Vector2(transform.position.x, transform.position.z));

    // change direction at boundary
    if(distOrigin > wanderRange){
      // move in new direction
      Vector2 newPoint = Random.insideUnitCircle * wanderRange;
      Vector3 newTarget = new Vector3(newPoint.x + origin.x,
                           Random.Range(-heightRange, heightRange) + origin.y,
                           newPoint.y + origin.z);
      curDirection = Vector3.Normalize(newTarget - transform.position);
    }

    float distVert = Mathf.Abs(origin.y - transform.position.y);
    // if touch ceiling or floor flip vertical direction
    if(distVert > heightRange){
      curDirection.y = -curDirection.y;
    }

    // move in the wanted direction
    transform.Translate(curDirection*speed);
  }

  void Die(){
    explosion.Play();
    deadTimer = 0.35f;
  }
}

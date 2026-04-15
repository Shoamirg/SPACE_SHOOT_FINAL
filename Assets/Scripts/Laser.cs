using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8;
    [SerializeField]
    private float _turnSpeed = 180f;
    [SerializeField]
    private float _lifeTime = 6f;
    [SerializeField]
    private float _enemyID; //0 = player_1, 1 = player_2, 2 = enemy_1, 3 = enemy_2. 
    private bool _isEnemyLaser = false;
    Rigidbody2D rb;
    Transform target;                                  
    float lifeTimer;
    void Start()
    {
        GameObject closest = FindClosestEnemy();
        rb = GetComponent<Rigidbody2D>();

        lifeTimer = _lifeTime;
        if (closest != null && _enemyID == 3 && _isEnemyLaser == false)
        {
            target = closest.transform;
        }
        if (_enemyID == 3 && _isEnemyLaser == true)
        {
            // If no explicit target set, try to find the Player by tag
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player) target = player.transform;
            }

            rb.velocity = transform.up * _speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (_enemyID)
        {
            case 0:
                MoveUp();
                break;
            case 1:
                MultiShot();
                break;
            case 2:
                MoveDown();
                break;
            case 3:
                HomingMissile();
                break;


        }
    }

    public void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 9f)
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -9f)
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void MultiShot()
    {
        transform.Translate(transform.up * _speed * Time.deltaTime);
        if (transform.position.y < -9f || transform.position.y > 9f)
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void HomingMissile()
    {
        lifeTimer -= Time.fixedDeltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
            return;
        }
        if (target == null)
        {
            rb.velocity = transform.up * _speed;
            return;
        }
        Vector2 toTarget = (Vector2)(target.position - transform.position);
        if (toTarget.sqrMagnitude < 0.0001f)
        {
            rb.velocity = transform.up * _speed;
            return;
        }
        Vector2 forward = transform.up;

        // Compute signed angle (degrees) between forward and direction to target
        float angleToTarget = Vector2.SignedAngle(forward, toTarget);

        // Limit rotation this physics step by turnSpeed * deltaTime
        float maxRotationThisStep = _turnSpeed * Time.deltaTime;
        float rotation = Mathf.Clamp(angleToTarget, -maxRotationThisStep, maxRotationThisStep);

        // Apply rotation around Z
        transform.Rotate(Vector3.forward, rotation);

        // Update velocity to match new forward direction
        rb.velocity = transform.up * _speed;

        if (transform.position.y > 7f || transform.position.y < -7f||transform.position.x>10f||transform.position.x<-10f)
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaserSimple()
    {
        _enemyID = 2;
    }

    public void AssignHomingMissile()
    {
        _enemyID = 3;
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
        gameObject.tag = "Enemy Laser";
    }
    
    public bool ShowLaserType()
    {
        return _isEnemyLaser;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if(player != null)
            {
                player.Damage();
                Destroy(this.gameObject);
            }
        }
        else if (other.tag == "Enemy" && _isEnemyLaser == false)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            Spawn spawn = GameObject.Find("Spawn Manager").GetComponent<Spawn>();
            if (enemy != null)
            {
                spawn.EnemyCount();
                enemy.EnemyDeath();
                Destroy(this.gameObject);
            }
        }
        else if (other.tag == "Powerup" && _isEnemyLaser == true)
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

        else if(other.tag == "Boss" && _isEnemyLaser == false)
        {
            Boss boss = other.GetComponent<Boss>();
            if(boss != null)
            {
                boss.Damage();
                Destroy(this.gameObject);
            }
        }
    }
}

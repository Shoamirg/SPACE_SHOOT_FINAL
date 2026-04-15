// Author: Shoamir Shorustamov
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _enemyPoints = 10;
    [SerializeField]
    private GameObject[] _enemyLaser;
    [SerializeField]
    private GameObject _enemyShield;
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private Transform _firePointBack;
    private Transform _playerLocation;
    [SerializeField]
    private float _enemyMovementID = 0;
    [SerializeField]
    private float _enemyShootingID = 0;
    [SerializeField]
    private float _xBorders;
    [SerializeField]
    private float _yBorders;
    private AudioSource _audioSource;
    [SerializeField]
    private float _aggroRadius = 4f;
    [SerializeField]
    private float _ramSpeed = 6f;
    private float _ramTimer = 0f;
    private float _returnToNormalAfter = 0.5f;
    private float _canFire = -1f;
    private float _fireRate = 3f;
    private float _horizontalSpeed = 2f;
    private float _direction = -1f; 
    private float _timer = 0f;
    private float _interval = 1.5f;
    private float _detectionRadius = 5f;
    private float _dodgeSpeed = 5f;
    private float _dodgeTime = 0.5f;
    private bool _canShoot = true;
    [SerializeField]
    private bool _canDodge = false;
    [SerializeField]
    private bool _canRam = false;
    Player _player;
    Animator _enemyExplosion;
    
    // Start is called before the first frame update
    void Start()
    {
        
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _playerLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if(_player == null)
        {
            Debug.LogError("_player is NULL");
        }
        if(_playerLocation == null)
        {
            Debug.Log("_playerLocation is NULL");
        }
        _enemyExplosion = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        Shooting();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Spawn spawn = GameObject.Find("Spawn Manager").GetComponent<Spawn>();

        if (other.tag == "Player")
            {
                if (_enemyShield != null)
                {
                spawn.EnemyCount();
                Destroy(_enemyShield);
                    _player.Damage();
                return;
                }
                else
                {
                spawn.EnemyCount();
                _player.Damage();
                    _enemyExplosion.SetTrigger("OnEnemyDeath");
                    _speed = 0;
                    _horizontalSpeed = 0;
                    _ramSpeed = 0;
                    _audioSource.Play();
                    Destroy(gameObject.GetComponent<Collider2D>());
                    _canShoot = false;
                    Destroy(this.gameObject, 2.5f);
            }
            }
            else if (other.tag == "Laser")
            {
                if (_enemyShield != null)
                {
                Destroy(_enemyShield);
                    Destroy(other.gameObject);
                    return;
                }

            }       


    }


    public void CalculateMovement()
    {
        if (transform.position.y < -_yBorders)
        {
            transform.position = new Vector3(Random.Range(-9f, 9f), _yBorders, 0);
        }
        if(transform.position.x > _xBorders)
        {
            transform.position = new Vector2(_xBorders, transform.position.y);
            _direction *= -1f;
        }
        else if(transform.position.x < -_xBorders)
        {
            transform.position = new Vector2(-_xBorders, transform.position.y);
            _direction *= -1f;
        }
        if (_playerLocation != null && _canRam == true && _playerLocation.position.y < transform.position.y)
        {
            float distToPlayer = Vector2.Distance(transform.position, _playerLocation.position);
            if (distToPlayer <= _aggroRadius)
            {
                // Ram toward player
                Vector2 toPlayer = (_playerLocation.position - transform.position).normalized;
                transform.Translate(toPlayer * _ramSpeed * Time.deltaTime, Space.World);

                // optional: keep ramming for a short burst (prevents jittering when inside aggressively)
                _ramTimer += Time.deltaTime;
                if (_ramTimer >= _returnToNormalAfter)
                {
                    _ramTimer = 0f;
                    // you can optionally flip _enemyMovementID or change state here
                }
                return; // skip the normal movement switch while ramming
            }
        }
        switch (_enemyMovementID)
        {
            case 0:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                break;
            case 1:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                transform.Translate(Vector3.right*_direction * _horizontalSpeed * Time.deltaTime);
                _timer += Time.deltaTime;
                if (_timer >= _interval)
                {
                    _direction *= -1f; 
                    _timer = 0f;
                }
                break;
            case 2:
                MoveSmart();
                break;
            
              
        }
       
    }

    public void Shooting()
    {
        if (Time.time > _canFire && _canShoot == true)
        {

            _fireRate = Random.Range(4f, 6f);
            _canFire = Time.time + _fireRate;
            switch (_enemyShootingID)
            {
                case 0:
                    ShootSingle();
                    break;
                case 1:
                    ShootDouble();
                    break;
                case 2:
                    ShootHomingMissile();
                    break;
                case 3:
                    ShootSmart();
                    break;
            }
        }
    }

    public void ShootSingle()
    {
        GameObject enemyLaser = Instantiate(_enemyLaser[0], _firePoint.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
            lasers[i].AssignEnemyLaserSimple();
        }

    }

    public void ShootDouble()
    {
        GameObject enemyLaser = Instantiate(_enemyLaser[1], transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
            lasers[i].AssignEnemyLaserSimple();
        }
    }

    public void ShootHomingMissile()
    {
        GameObject enemyLaser = Instantiate(_enemyLaser[2], _firePoint.position,transform.rotation);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
            lasers[i].AssignHomingMissile();          
            lasers[i].transform.eulerAngles = new Vector3(0,0,180);
        }
    }

    public void ShootSmart()
    {
        if (_playerLocation != null && _playerLocation.position.y > transform.position.y)
        {
          GameObject enemyLaser = Instantiate(_enemyLaser[0],_firePointBack.position, Quaternion.identity);
          Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
        else
        {
            GameObject enemyLaser = Instantiate(_enemyLaser[0], _firePoint.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
                lasers[i].AssignEnemyLaserSimple();
            }
        }
    }

    public void MoveSmart()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _detectionRadius);

        foreach(Collider2D hit in hits)
        {
            if (hit != null)
            {

                if (hit.tag == "Laser" && _canDodge)
                {
                    StartCoroutine(Dodge(hit));
                    break;
                }
            }
        }
    }

    public void EnemyDeath()
    {
        _player.AddScore(_enemyPoints);
        _enemyExplosion.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _horizontalSpeed = 0;
        _ramSpeed = 0;
        _audioSource.Play();
        Destroy(gameObject.GetComponent<Collider2D>());
        _canShoot = false;
        Destroy(this.gameObject, 2.5f);
    }

    IEnumerator Dodge(Collider2D laser)
    {
        Vector2 direction;
        Transform laserPosition = laser.gameObject.transform;
        float randomDirection = Random.Range(1,3);
        if (laserPosition.position.x > transform.position.x)
        {
            direction = Vector2.left;
        }
        else if(laserPosition.position.x < transform.position.x)
        {
            direction = Vector2.right;
        }
        else
        {
            direction = new Vector2(randomDirection, 0);
        }

        float timer = Time.time + _dodgeTime;
        _canDodge = false;

        while (timer > Time.time)
        {
            transform.Translate(direction * _dodgeSpeed * Time.deltaTime);
            yield return null;
        }
        _canDodge = true;
    }


}


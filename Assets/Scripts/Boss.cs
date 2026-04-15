using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private float _speedHorizontal = 4f;
    private float _speedVertical = 5f;
    private float _direction = 1;
    [SerializeField]
    private float _xBorder = 10f;
    [SerializeField]
    private GameObject _bossLaser;
    private Spawn _spawn;
    private float _canFire = -1f;
    private float _fireRate = 3f;
    [SerializeField]
    private int _health = 10;
    private bool _canshoot = false;
    private bool _isInPosition = false;
    // Start is called before the first frame update
    void Awake()
    {
        transform.position = new Vector3(0, 7, 0);
        if(_bossLaser == null)
        {
            Debug.LogError("Boss laser is NULL");
        }
        _spawn = GameObject.Find("Spawn Manager").GetComponent<Spawn>();
        if(_spawn == null)
        {
            Debug.LogError("_spawn is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Shooting();

       
    }

    public void Movement()
    {
        if (!_isInPosition)
        {
            transform.Translate(Vector3.down * _speedVertical * Time.deltaTime);

            if (transform.position.y < 3.6f)
            {
                _isInPosition = true;
                _canshoot = true;
            }
        }
        else
        {
            transform.Translate(Vector3.right * _speedHorizontal * _direction * Time.deltaTime);

            if (transform.position.x > _xBorder)
            {
                transform.position = new Vector2(_xBorder, transform.position.y);
                _direction *= -1;
            }
            else if (transform.position.x < -_xBorder)
            {
                transform.position = new Vector2(-_xBorder, transform.position.y);
                _direction *= -1;
            }
        }
    }

    public void Shooting()
    {
        if (Time.time > _canFire && _canshoot == true)
        {
            _fireRate = Random.Range(3f, 5f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_bossLaser, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
                lasers[i].AssignEnemyLaserSimple();
            }
        }
    }

    public void Damage()
    {
        _health -= 1;
        if(_health <= 0)
        {
            _spawn.EnemyCount();
            Destroy(this.gameObject);
        }
    }
}

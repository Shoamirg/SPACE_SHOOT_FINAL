using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 7;
    [SerializeField]
    private Transform _laser;
    [SerializeField]
    private float _fireRate = 0.25f;
    [SerializeField]
    private float _xBoreders;
    [SerializeField]
    private float _yBorders;
    [SerializeField]
    private int _score;
    private float _cooldown = -1f;
    [SerializeField]
    private float _ammoMax;
    private float _ammoReal;
    [SerializeField]
    private float _magnetRadius = 5f;
    [SerializeField]
    private float _magnetSpeed = 5f;
    [SerializeField]
    private float _thrusterboost = 1.25f;
    [SerializeField]
    private float _shieldDurability = 3f;
    [SerializeField]
    private int _lives = 3;
    private Spawn _spawn;
    [SerializeField]
    private Transform _tripleshot;
    [SerializeField]
    private GameObject _multiShot;
    [SerializeField]
    private GameObject _homingMissile;
    [SerializeField]
    private GameObject _shield;
    [SerializeField]
    private GameObject _rightTurbine;
    [SerializeField]
    private GameObject _leftTurbine;
    [SerializeField]
    private AudioSource _lasersounds;
    private bool _IsShieldActive = false;
    private bool _cantripleshoot = false;
    private bool _canUseThruster = true;
    [SerializeField]
    private bool _canMultiShoot = false;
    [SerializeField]
    private bool _haveHomingMissile = false;
    UI_Manager _uiManager;
    [SerializeField]
    private GameObject _thruster;
    SpriteRenderer _thrusterRenderer;
    SpriteRenderer _shieldRenderer;
    [SerializeField]
    private Color _thrusterActivatedColor;
    [SerializeField]
    private Color _thrusterOriginalColor;
    [SerializeField]
    private Color _thrusterRechargeColor;
    [SerializeField]
    private Color _shieldDurabilityOneColor;
    [SerializeField]
    private Color _shieldDurabilityTwoColor;
    [SerializeField]
    private Color _shieldDurabilityThreeColor;

    // Start is called before the first frame update
    void Start()
    {
        _ammoReal = _ammoMax;
        transform.position = new Vector3(0, 0, 0);
        _spawn = GameObject.Find("Spawn Manager").GetComponent<Spawn>();
        if (_spawn == null)
        {
            Debug.LogError("_spawn is NULL");
        }
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        if (_uiManager == null)
        {
            Debug.LogError("_uiManager is null");
        }
        if (_lasersounds == null)
        {
            Debug.LogError("_lasersounds is NULL");
        }
        _thrusterRenderer = _thruster.GetComponent<SpriteRenderer>();
        if (_thrusterRenderer == null)
        {
            Debug.LogError("Thruster renderer is NULL");
        }
        _shieldRenderer = _shield.GetComponent<SpriteRenderer>();
        if (_shieldRenderer == null)
        {
            Debug.LogError("Shield renderer is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Shooting();


    }

    void Movement()
    {
        float VerticalInput = Input.GetAxis("Vertical");
        float HorizontalInput = Input.GetAxis("Horizontal");

        transform.Translate(new Vector3(_speed * HorizontalInput, _speed * VerticalInput, 0) * Time.deltaTime);

        if (transform.position.y > _yBorders)
        {
            transform.position = new Vector2(transform.position.x, _yBorders);
        }
        else if (transform.position.y < -_yBorders)
        {
            transform.position = new Vector2(transform.position.x, -_yBorders);
        }

        if (transform.position.x > _xBoreders)
        {
            transform.position = new Vector2(-_xBoreders, transform.position.y);
        }
        else if (transform.position.x < -_xBoreders)
        {
            transform.position = new Vector2(_xBoreders, transform.position.y);
        }
        Thrusters();
        Magnet();
    }

    void Thrusters()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canUseThruster == true)
        {
            StartCoroutine(PowerdownRoutineThrusters());

        }

    }
    
    public void Magnet()
    {
        if (Input.GetKey(KeyCode.C))
        {
            GameObject[] powerups = GameObject.FindGameObjectsWithTag("Powerup");
            foreach(GameObject go in powerups)
            {
                Powerup p = go.GetComponent<Powerup>();
                if (p != null)
                {
                    p.PowerupMagnet(_magnetRadius, transform, _magnetSpeed);
                }
            }
        }
    }

    void Shooting()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time > _cooldown)
        {
            _cooldown = Time.time + _fireRate;
            if (_ammoReal > 0)
            {
                _ammoReal -= 1;
                if (_cantripleshoot == true)
                {
                    _canMultiShoot = false;
                    Instantiate(_tripleshot, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                }
                else if (_canMultiShoot == true)
                {
                    _cantripleshoot = false;
                    Instantiate(_multiShot, new Vector3(transform.position.x, transform.position.y + 1.5f, 0), Quaternion.identity);
                }
                else if(_haveHomingMissile == true)
                {
                    _canMultiShoot = false;
                    _cantripleshoot = false;
                    Instantiate(_homingMissile, new Vector3(transform.position.x, transform.position.y + 1.05f, 0), Quaternion.identity);
                }

                else
                {
                    Instantiate(_laser, new Vector3(transform.position.x, transform.position.y + 1.05f, 0), Quaternion.identity);
                }
                _lasersounds.Play();
                _uiManager.DisplayAmmo(_ammoReal, _ammoMax);
            }
        }
    }

    void Health()
    {
        if (_lives <= 0)
        {
            _spawn.OnPlayerDeath();
            Destroy(this.gameObject);
         
        }
    }

    public void Damage()
    {
        if (_IsShieldActive == false)
        {
            _lives = _lives - 1;
            _uiManager.CameraShake();
            if(_lives > 2)
            {
                _rightTurbine.SetActive(false);
                _leftTurbine.SetActive(false);
            }
            else if (_lives == 2)
            {
                _rightTurbine.SetActive(true);
                _leftTurbine.SetActive(false);
            }
            else if (_lives == 1)
            {
                _leftTurbine.SetActive(true);
                _rightTurbine.SetActive(true);
            }
            _uiManager.DisplayLives(_lives);
        }
        else if (_IsShieldActive == true)
        {
            _shieldDurability -= 1;
            _uiManager.CameraShake();
            if (_shieldDurability == 2)
            {
                _shieldRenderer.color = _shieldDurabilityTwoColor;
            }
            else if (_shieldDurability == 1)
            {
                _shieldRenderer.color = _shieldDurabilityOneColor;
            }
            else if (_shieldDurability < 1)
            {
                _IsShieldActive = false;
                _shield.SetActive(false);
                _shieldRenderer.color = _shieldDurabilityThreeColor;
            }

        }
        Health();
    }

    public void TripleShotActive()
    {
        _cantripleshoot = true;
        StartCoroutine(PowerdownRoutineTripleshot());
    }

    public void MultiShotActive()
    {
        _canMultiShoot = true;
        StartCoroutine(PowerdownRoutineMultishot());
    }

    public void HomingMissileActive()
    {
        _haveHomingMissile = true;
        StartCoroutine(PowerdownRoutineHomingMissile());
    }

    public void SpeedBoostActive()
    {
        _speed *= 2;
        StartCoroutine(PowerdownRoutineSpeedBoost());
    }

    public void ShieldActive()
    {
        _IsShieldActive = true;
        _shield.SetActive(true);
        _shieldDurability = 3f;
        _shieldRenderer.color = _shieldDurabilityThreeColor;
    }

    public void AmmoActive()
    {
        _ammoReal = _ammoMax;
        _uiManager.DisplayAmmo(_ammoReal, _ammoMax);
    }

    public void SlowActive()
    {
        _speed /= 2;
        StartCoroutine(PowerdownRoutineSlowDebuff());
    }

    public void HealthActive()
    {
        if (_lives < 3)
        {
            _lives += 1;
            _uiManager.DisplayLives(_lives);
        }

        if (_lives > 2)
        {
            _rightTurbine.SetActive(false);
            _leftTurbine.SetActive(false);
        }
        else if (_lives == 2)
        {
            _rightTurbine.SetActive(true);
            _leftTurbine.SetActive(false);
        }
        else if (_lives == 1)
        {
            _leftTurbine.SetActive(true);
            _rightTurbine.SetActive(true);
        }
    }

    public void AddScore(int enemypoints)
    {
        _score += enemypoints;
        _uiManager.DisplayScore(_score);
    }

    IEnumerator PowerdownRoutineTripleshot()
    {
        yield return new WaitForSeconds(5);
        _cantripleshoot = false;
    }

    IEnumerator PowerdownRoutineMultishot()
    {
        yield return new WaitForSeconds(5);
        _canMultiShoot = false;
    }

    IEnumerator PowerdownRoutineHomingMissile()
    {
        yield return new WaitForSeconds(5);
        _haveHomingMissile = false;
    }

    IEnumerator PowerdownRoutineSpeedBoost()
    {
        yield return new WaitForSeconds(5);
        _speed /= 2;
    }

    IEnumerator PowerdownRoutineSlowDebuff()
    {
        yield return new WaitForSeconds(5);
        _speed *= 2;
    }

    IEnumerator PowerdownRoutineThrusters()
    {
        _speed *= _thrusterboost;
        _thrusterRenderer.color = _thrusterActivatedColor;
        _canUseThruster = false;
        yield return new WaitForSeconds(8f);
        _speed /= _thrusterboost;
        _thrusterRenderer.color = _thrusterRechargeColor;
        yield return new WaitForSeconds(5f);
        _canUseThruster = true;
        _thrusterRenderer.color = _thrusterOriginalColor;
    }

}

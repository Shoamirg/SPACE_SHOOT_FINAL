 
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    Player _player;
    [SerializeField] // 0 = tripleshot, 1 = speed, 2 = shield, 3 = ammo, 4= health, 5 = multishotб 6 = slow , 7 = homingMissile.
    private int _powerupID = 0;
    [SerializeField]
    private AudioClip _powerupNoise;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime);
        if (transform.position.y < -9)
        {
          
            Destroy(this.gameObject);
        }
    } 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            switch (_powerupID)
            {
                case 0:
                    _player.TripleShotActive();
                    break;
                case 1:
                    _player.SpeedBoostActive();
                    break;
                case 2:
                    _player.ShieldActive();
                    break;
                case 3:
                    _player.AmmoActive();
                    break;
                case 4:
                    _player.HealthActive();
                    break;
                case 5:
                    _player.MultiShotActive();
                    break;
                case 6:
                    _player.SlowActive();
                    break;
                case 7:
                    _player.HomingMissileActive();
                    break;
                default:
                    Debug.Log("Default_Powerup is activated");
                    break;
            }
            AudioSource.PlayClipAtPoint(_powerupNoise, transform.position);
            Destroy(this.gameObject);
        }
    }

    public void PowerupMagnet(float _radiusOfEffect, Transform _playerLocation,float _powerupSpeed)
    {
        
            float distToPlayer = Vector2.Distance(transform.position, _playerLocation.position);
            if(distToPlayer <= _radiusOfEffect)
            {
                Vector2 toPlayer = (_playerLocation.position - transform.position).normalized;
                transform.Translate(toPlayer * _powerupSpeed * Time.deltaTime, Space.World);
            }
        
    }

}

using System.Collections;
 
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _commonEnemy;
    [SerializeField]
    private GameObject[] _rareEnemy;
    [SerializeField]
    private GameObject _boss;
    [SerializeField]
    private GameObject _enemyContainer;
    private GameManager _gameManager;
    private UI_Manager _uiManager;
    [SerializeField]
    private bool _stopSpawn = false;
    [SerializeField]
    private GameObject[] _commonPowerups;
    [SerializeField]
    private GameObject[] _rarePowerups;
    private int _currentWave = 1;
    [SerializeField]
    private int[] _WaveEnemyCount;
    [SerializeField]
    private float[] _enemySpawnIntervalPerWave;
    [SerializeField]
    private int _enemyCountSpawned = 0;
    [SerializeField]
    private int _enemyDestroyed = 0;
    [SerializeField] private GameObject _homingPowerup;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("_gameManager is NULL");
        }
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        if (_uiManager == null)
        {
            Debug.LogError("_uiManager is NULL");
        }
    }


    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (!_stopSpawn)
        {
            switch (_currentWave)
            {
               
                case 1:
                    if (_enemyCountSpawned < _WaveEnemyCount[_currentWave -1])
                    {
                        int rarityChance = Random.Range(0, 100);
                        if (rarityChance < 10)
                        {
                            int randomEnemy = Random.Range(0, _rareEnemy.Length);
                            GameObject newEnemy = Instantiate(_rareEnemy[randomEnemy], new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
                            newEnemy.transform.parent = _enemyContainer.transform;
                        }
                        else
                        {
                            int randomEnemy = Random.Range(0, _commonEnemy.Length);
                            GameObject newEnemy = Instantiate(_commonEnemy[randomEnemy], new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
                            newEnemy.transform.parent = _enemyContainer.transform;
                        }
                        _enemyCountSpawned++;
                    }
                if(_enemyDestroyed >= _WaveEnemyCount[0])
                {
                    _currentWave +=1;
                        _enemyCountSpawned = 0;
                        _enemyDestroyed = 0;
                }
                yield return new WaitForSeconds(_enemySpawnIntervalPerWave[_currentWave-1]);
                    break;
                case 2:
                    if (_enemyCountSpawned < _WaveEnemyCount[_currentWave-1])
                    {
                        int rarityChancetwo = Random.Range(0, 100);
                        if (rarityChancetwo < 30)
                        {
                            int randomEnemy = Random.Range(0, _rareEnemy.Length);
                            GameObject newEnemy = Instantiate(_rareEnemy[randomEnemy], new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
                            newEnemy.transform.parent = _enemyContainer.transform;
                        }
                        else
                        {
                            int randomEnemy = Random.Range(0, _commonEnemy.Length);
                            GameObject newEnemy = Instantiate(_commonEnemy[randomEnemy], new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
                            newEnemy.transform.parent = _enemyContainer.transform;
                        }
                        _enemyCountSpawned++;
                    }
                    if (_enemyDestroyed >= _WaveEnemyCount[_currentWave-1])
                    {
                        _currentWave += 1;
                        _enemyCountSpawned = 0;
                        _enemyDestroyed = 0;
                    }
                        yield return new WaitForSeconds(_enemySpawnIntervalPerWave[1]);

                    break;
                case 3:
                    if (_enemyCountSpawned < 1)
                    {
                        Instantiate(_boss, new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
                        _enemyCountSpawned++;
                    }
                    while (_enemyDestroyed < 1)
                    {
                        yield return null; // Wait for next frame
                    }

                    // When boss is dead, trigger Game Over
                    _gameManager.GameOver();
                    _uiManager.GameOverSequence();

            
                 yield break;
            }
            
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (_stopSpawn == false)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-9f, 9f), 7, 0);

            int rarityChance = Random.Range(0, 100);

            if (rarityChance <10 )
            {
                // 🔥 RARE POWERUPS
                int roll = Random.Range(0, 100);
                GameObject selectedPowerup;

                if (roll < 50)
                {
                    // HOMING MISSILE (boosted chance)
                    selectedPowerup = _homingPowerup; // 👈 confirm index
                }
                else
                {
                    int randomIndex = Random.Range(0, _rarePowerups.Length);
                    selectedPowerup = _rarePowerups[randomIndex];
                }

                Instantiate(selectedPowerup, spawnPos, Quaternion.identity);
            }
            else
            {
                // ✅ COMMON POWERUPS (THIS WAS MISSING)
                int randomPowerup = Random.Range(0, _commonPowerups.Length);
                Instantiate(_commonPowerups[randomPowerup], spawnPos, Quaternion.identity);
            }

            // ✅ IMPORTANT DELAY
            yield return new WaitForSeconds(Random.Range(3f, 7f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawn = true;
    }

    public void EnemyCount()
    {
        _enemyDestroyed++;
    }
}

using System.Collections;
 
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _commonEnemy;
    [SerializeField]
    private GameObject[] _rareEnemy;
    [SerializeField]
    [Range(0f,1f)]
    private float _enemyShieldChance = 0.20f;
    [SerializeField]
    private GameObject _boss;
    [SerializeField]
    private GameObject _enemyContainer;
    private GameManager _gameManager;
    private UI_Manager _uiManager;
    [SerializeField]
    private bool _stopSpawn = false;
    private bool _isSpawning = false;
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
        // Debug: report how many shield powerup prefabs are present in each array
        int commonShieldCount = 0;
        int rareShieldCount = 0;
        if (_commonPowerups != null)
        {
            foreach (GameObject p in _commonPowerups)
            {
                if (p == null) continue;
                Powerup pu = p.GetComponent<Powerup>();
                if (pu != null && pu.GetPowerupID() == 2)
                {
                    commonShieldCount++;
                    Debug.Log("Spawn Start: commonPowerups contains shield prefab: " + p.name);
                }
            }
        }
        if (_rarePowerups != null)
        {
            foreach (GameObject p in _rarePowerups)
            {
                if (p == null) continue;
                Powerup pu = p.GetComponent<Powerup>();
                if (pu != null && pu.GetPowerupID() == 2)
                {
                    rareShieldCount++;
                    Debug.Log("Spawn Start: rarePowerups contains shield prefab: " + p.name);
                }
            }
        }
        Debug.Log($"Spawn Start: shield counts - common: {commonShieldCount}, rare: {rareShieldCount}");
    }


    public void StartSpawning()
    {
        if (_isSpawning) return;
        _isSpawning = true;
        if (_uiManager != null)
            _uiManager.DisplayWave(_currentWave);
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawn==false)
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
                                MaybeAttachShield(newEnemy);
                        }
                        else
                        {
                            int randomEnemy = Random.Range(0, _commonEnemy.Length);
                                GameObject newEnemy = Instantiate(_commonEnemy[randomEnemy], new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
                                newEnemy.transform.parent = _enemyContainer.transform;
                                MaybeAttachShield(newEnemy);
                        }
                        _enemyCountSpawned++;
                    }
                if(_enemyDestroyed >= _WaveEnemyCount[0])
                {
                    _currentWave +=1;
                        _enemyCountSpawned = 0;
                        _enemyDestroyed = 0;
                        if (_uiManager != null) _uiManager.DisplayWave(_currentWave);
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
                            MaybeAttachShield(newEnemy);
                        }
                        else
                        {
                            int randomEnemy = Random.Range(0, _commonEnemy.Length);
                            GameObject newEnemy = Instantiate(_commonEnemy[randomEnemy], new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
                            newEnemy.transform.parent = _enemyContainer.transform;
                            MaybeAttachShield(newEnemy);
                        }
                        _enemyCountSpawned++;
                    }
                    if (_enemyDestroyed >= _WaveEnemyCount[_currentWave-1])
                    {
                        _currentWave += 1;
                        _enemyCountSpawned = 0;
                        _enemyDestroyed = 0;
                        if (_uiManager != null) _uiManager.DisplayWave(_currentWave);
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
            // Small chance to explicitly spawn a shield powerup if available
            int explicitShieldChance = Random.Range(0, 100);
            if (explicitShieldChance < 5)
            {
                bool spawned = false;
                foreach (GameObject p in _commonPowerups)
                {
                    Powerup pu = p.GetComponent<Powerup>();
                    if (pu != null && pu.GetPowerupID() == 2)
                    {
                        Instantiate(p, new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
                        Debug.Log("Spawn: Explicit shield powerup instantiated from commonPowerups");
                        spawned = true;
                        break;
                    }
                }
                if (!spawned)
                {
                    foreach (GameObject p in _rarePowerups)
                    {
                        Powerup pu = p.GetComponent<Powerup>();
                        if (pu != null && pu.GetPowerupID() == 2)
                        {
                            Instantiate(p, new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
                            Debug.Log("Spawn: Explicit shield powerup instantiated from rarePowerups");
                            spawned = true;
                            break;
                        }
                    }
                }
                if (spawned)
                {
                    yield return new WaitForSeconds(Random.Range(3, 7));
                    continue;
                }
            }

            int rarityChance = Random.Range(0, 100);
            if (rarityChance < 10)
            {
                int randomPowerup = Random.Range(0, _rarePowerups.Length);
                Instantiate(_rarePowerups[randomPowerup], new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
            }
            else
            {
                int randomPowerup = Random.Range(0, _commonPowerups.Length);
                Instantiate(_commonPowerups[randomPowerup], new Vector3(Random.Range(-9f, 9f), 7, 0), Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(3, 7));
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

    private void MaybeAttachShield(GameObject enemy)
    {
        if (enemy == null) return;
        if (Random.value <= _enemyShieldChance)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e != null)
            {
                e.SetShieldActive(true);
            }
        }
    }
}

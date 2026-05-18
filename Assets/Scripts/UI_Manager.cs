using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _score;
    [SerializeField]
    private Text _ammo;
    [SerializeField]
    private TMP_Text _thrusterText;
    [SerializeField]
    private Slider _thrusterSlider;
    [SerializeField]
    private TMP_Text _waveText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Text _gameOver;
    [SerializeField]
    private Image _livesIMG;
    [SerializeField]
    private Sprite[] _liveSprites;
    private GameManager _gameManager;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    Vector3 _leftPosition = new Vector3(-0.5f, -0.5f, -10);
    [SerializeField]
    Vector3 _rightPosition = new Vector3(0.5f, 0.5f, -10);
    [SerializeField]
    private float _cameraShakeInterval = 0.2f;
    [SerializeField]
    private float _cameraShakeDuration = 4;

    // Start is called before the first frame update
    void Start()
    {
        _score.text = "Score: 0";
        _gameOver.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if(_gameManager == null)
        {
            Debug.LogError("_gameManager is NULL");
        }
      
    }

    // Update is called once per frame
 

    public void DisplayScore(int playerscore)
    {
        _score.text = "Score: " + playerscore;
    }

    public void DisplayLives(int currentlives)
    {  if (currentlives < 0 || currentlives >= _liveSprites.Length) return;
        _livesIMG.sprite = _liveSprites[currentlives];

        if(currentlives < 1)
        {
            GameOverSequence();
            _gameManager.GameOver();
        }
    }

    public void DisplayAmmo(float ammoReal,float ammoMax)
    {
        _ammo.text = "Ammo: " + ammoReal + "/" + ammoMax;
    }

    public void DisplayThruster(float current, float max)
    {
        if (_thrusterText != null)
            _thrusterText.text = "Thruster: " + Mathf.CeilToInt(current) + "/" + Mathf.CeilToInt(max);
        if (_thrusterSlider != null)
        {
            _thrusterSlider.minValue = 0f;
            _thrusterSlider.maxValue = max;
            _thrusterSlider.value = current;
            // Color fill: green when full, white when empty
            Image fillImage = _thrusterSlider.fillRect != null
                ? _thrusterSlider.fillRect.GetComponent<Image>()
                : null;
            if (fillImage != null)
                fillImage.color = Color.Lerp(Color.white, Color.green, current / max);
        }
    }

    public void DisplayWave(int wave)
    {
        if (_waveText == null) return;
        if (wave >= 3)
            _waveText.text = "[!!] BOSS INCOMING! [!!]";
        else
            _waveText.text = "Wave: " + wave;
    }
    public void GameOverSequence()
    {
        _gameOver.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(Flickering());
    }
    public void CameraShake()
    {
        StartCoroutine(CameraShakeRoutine());
    }
    IEnumerator Flickering()
    {
        while (true)
        {
            _gameOver.text = "Game Over";
            yield return new WaitForSeconds(0.5f);
            _gameOver.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CameraShakeRoutine()
    {
        for (int i = 0; i < _cameraShakeDuration; i++)
        {
            _camera.transform.position = _leftPosition;
            yield return new WaitForSeconds(_cameraShakeInterval);
            _camera.transform.position = _rightPosition;
            yield return new WaitForSeconds(_cameraShakeInterval);
        }
        _camera.transform.position = new Vector3(0, 0, -10);
    }
}

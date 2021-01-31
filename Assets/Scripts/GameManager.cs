using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    [SerializeField] private GameObject _playerGO;
    [SerializeField] private GameObject _spaceShipGO;
    [SerializeField] private GameObject _CUBES;
    private float _oxygenLevel;
    public float Range;
    private int _rangeCount;
    private float _ogSize;
    private float _gasMultiply;
    private bool _hasSaved;
    private bool _isOutside;
    private PlayerController _characterController;
    private Vector3 _startPos;
    [SerializeField] private Slider _gasSlider;
    private List<Camera> _cameraList;
    public LineRenderer LineRenderer;
    public int CollectedItems { get; private set; }
    
    [SerializeField] private GameObject _warningText;
    [SerializeField] private GameObject _gasGO;
    [SerializeField] private GameObject _unlockText;
    [SerializeField] private GameObject _startText;
    [SerializeField] private GameObject _gasText;
    [SerializeField] private GameObject _endText;
    [SerializeField] private GameObject _tooLongText;
    [SerializeField] private GameObject _menuGO;
    [SerializeField] private AudioClip _happy;
    [SerializeField] private AudioSource _effectSource;
    private float _gasLevel;
    public float GasLevel { 
        get { return _gasLevel; } 
        set {
            if (value < 15) _gasText.SetActive(true);
            else _gasText.SetActive(false);
            if (value >= 100) { _gasLevel = 100; 
                _gasSlider.value = 100f;
                _gasGO.SetActive(true);
                _gasGO.transform.localScale = new Vector3(_ogSize, _ogSize, _ogSize);
            }
            else if (value <= 0) { _gasLevel = 0; 
                _gasSlider.value = 0f;
                StartCoroutine(Die());
                _gasGO.SetActive(false);
            }
            else { _gasLevel = value; 
                _gasGO.SetActive(true);
                _gasGO.transform.localScale = new Vector3(_ogSize, _ogSize * (value/100), _ogSize);
                _gasSlider.value = value;
            }
        }
    }

    void Awake()
    {
        _instance = this;
        _gasLevel = 100;
        Range = 200;
        _hasSaved = false;
        _isOutside = false;
        _rangeCount = 0;
    }


    // Start is called before the first frame update
    void Start()
    {
        _startPos = _playerGO.transform.position;
        _characterController = _playerGO.GetComponent<PlayerController>();
        _unlockText.SetActive(false);
        var cameras = GameObject.FindGameObjectsWithTag("SideCamera");
        _cameraList = new List<Camera>();

        foreach (GameObject c in cameras) _cameraList.Add(c.GetComponent<Camera>());

        LineRenderer.endWidth = 10;
        LineRenderer.startWidth = 10;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        _startText.SetActive(true);
		StartCoroutine(NewUnlock(_startText));
        _ogSize = _gasGO.transform.localScale.y;
        _CUBES.SetActive(false);
        _gasText.SetActive(false);
        _gasMultiply = 1;
    }

	private void Update()
	{
        LineRenderer.SetPosition(0, _playerGO.transform.position);
        LineRenderer.SetPosition(1, _spaceShipGO.transform.position);
    }

	// Update is called once per frame
	void FixedUpdate()
    {
        float distance = Vector3.Distance(_spaceShipGO.transform.position, _playerGO.transform.position);
        if (distance < 50)
		{
            GasLevel += 0.1f * _gasMultiply;
            if (!_hasSaved)
            {
                SaveSystem.SavePlayer(this);
                _hasSaved = true;
                _isOutside = false;
            }
		}
		else if (distance < Range)
		{
            GasLevel -= 0.01f * _gasMultiply;
            _hasSaved = false;
            _isOutside = false;
        }
        else
		{
            _warningText.SetActive(true);
            GasLevel -= 0.1f * _gasMultiply;
            _isOutside = true;
            return;
		}
        _warningText.SetActive(false);
    }

    public void IncreaseRange(float amount)
	{
        Range += amount;
        //increase rope length
	}

    public void DecreaseGas(float amount)
	{
        Debug.Log("yep, works");
        GasLevel -= amount;
	}

    IEnumerator Die()
	{
        yield return new WaitForSeconds(5f);
        if(_isOutside || GasLevel <= 0)
		{
            SaveSystem.loadPlayer();
            _playerGO.transform.position = _startPos;
            _characterController.ResetPlayer();
            GasLevel = 100;
        }
	}

    public void CollectItems(int amount)
	{
        CollectedItems += amount;
        Debug.Log(CollectedItems);
        if (CollectedItems >= 4)
		{
            Range *= 1.5f;
            _gasMultiply /= 1.3f;
            CollectedItems = 0;
            _rangeCount++;
            
            int rnd = (int)Random.Range(0, _cameraList.Count);
            _cameraList.ElementAt(rnd).gameObject.SetActive(false);
            _cameraList.RemoveAt(rnd);

            if (_rangeCount >= 3)
			{
                _endText.SetActive(true);
                StartCoroutine(NewUnlock(_endText));
                _CUBES.SetActive(true);
                StartCoroutine(TooLong());
			}
			else
			{
                _unlockText.SetActive(true);
                StartCoroutine(NewUnlock(_unlockText));
            }

            _effectSource.clip = _happy;
            _effectSource.Play();
        }
	}
    IEnumerator TooLong()
	{
        yield return new WaitForSeconds(180f);
        _tooLongText.SetActive(true);
	}
    IEnumerator NewUnlock(GameObject text)
	{
        yield return new WaitForSeconds(10f);
        text.SetActive(false);
	}
    public void Exit()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void OpenMenu()
    {
        _menuGO.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CloseMenu()
    {
        _menuGO.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
    public void LoadEnd()
	{
        SceneManager.LoadSceneAsync(2);
	}
}

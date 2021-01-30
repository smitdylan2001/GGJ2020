using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private float _oxygenLevel;
    public float Range;
    private bool _hasSaved;
    private bool _isOutside;
    private PlayerController _characterController;
    private Vector3 _startPos;
    [SerializeField] private Slider _gasSlider;
    private List<Camera> _cameraList;
    public LineRenderer LineRenderer;
    public int CollectedItems { get; private set; }
    
    [SerializeField] private GameObject _warningText;
    [SerializeField] private GameObject _unlockText;
    private float _gasLevel;
    public float GasLevel { 
        get { return _gasLevel; } 
        set {
            if (value >= 100) { _gasLevel = 100; 
                _gasSlider.value = 100f; }
            else if (value <= 0) { _gasLevel = 0; 
                _gasSlider.value = 0f;
                StartCoroutine(Die());
            }
            else { _gasLevel = value; 
                _gasSlider.value = value; }
        }
    }

    void Awake()
    {
        _instance = this;
        _gasLevel = 100;
        Range = 200;
        _hasSaved = false;
        _isOutside = false;
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
            GasLevel += 0.1f;
            if (!_hasSaved)
            {
                SaveSystem.SavePlayer(this);
                _hasSaved = true;
                _isOutside = false;
            }
		}
		else if (distance < Range)
		{
            GasLevel -= 0.01f;
            _hasSaved = false;
            _isOutside = false;
        }
        else
		{
            _warningText.SetActive(true);
            GasLevel -= 0.1f;
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
            
        }
	}

    public void CollectItems(int amount)
	{
        CollectedItems += amount;
        Debug.Log(CollectedItems);
        if (CollectedItems >= 4)
		{
            Range *= 1.5f;
            CollectedItems = 0;
            
            _unlockText.SetActive(true);
            StartCoroutine(NewUnlock(_unlockText));
            int rnd = (int)Random.Range(0, _cameraList.Count);
            _cameraList[rnd].gameObject.SetActive(false);
            _cameraList.Remove(_cameraList[rnd]);
        }
	}

    IEnumerator NewUnlock(GameObject text)
	{
        yield return new WaitForSeconds(10f);
        text.SetActive(false);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float _range; 
    private float _gasLevel;
    public float GasLevel { 
        get { return _gasLevel; } 
        set {
            if (value >= 100) _gasLevel = 100;
            else _gasLevel = value;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _gasLevel = 100;
        _range = 50;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(_spaceShipGO.transform.position, _playerGO.transform.position) > 2)
		{
            GasLevel += 1;
		}
		else
		{
            _gasLevel -= 0.1f;
		}
    }

    public void IncreaseRange(float amount)
	{
        _range += amount;
        //increase rope length
	}
}

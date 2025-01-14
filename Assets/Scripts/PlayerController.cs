using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector2 _moveDirection; 
    private Vector2 _moveRotation;
    [SerializeField] private float _moveSensitivity = 200;
    [SerializeField] private float _rotateSensitivity = 800;
    private bool _stabalizeNow;
    private int _collectedItems;
    private int _collectedItemsDelivered;
    [SerializeField] private GameObject _collideWarning;
    [SerializeField] private GameObject _returnText;
    [SerializeField] private AudioSource _effectSource;
    [SerializeField] private AudioClip _happy;
    [SerializeField] private AudioClip _bonk;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _stabalizeNow = false;
        _collideWarning.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _rb.AddForce((gameObject.transform.forward * _moveDirection.y + gameObject.transform.right * _moveDirection.x) * Time.deltaTime * _moveSensitivity *100);
        _rb.AddRelativeTorque(new Vector3(-_moveRotation.y, _moveRotation.x, 0)* Time.deltaTime * (_rotateSensitivity /10));
    }

    void FixedUpdate()
	{
        if (_stabalizeNow)
        {
            _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, 0.04f);
            if (_rb.angularVelocity.magnitude > 0.02f)
			{
                _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, Vector3.zero, 0.0178f);
                GameManager.Instance.DecreaseGas(0.1f);
			}
			else
			{
				_rb.angularVelocity = Vector3.zero;
				
			}
		}
	}

    private IEnumerator ResetRotation()
	{
        yield return new WaitForSeconds(2f);
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0, 0, 0), 0.01f);
        GameManager.Instance.DecreaseGas(0.13f);
    }

    public void Move(InputAction.CallbackContext context)
	{
        _moveDirection = context.ReadValue<Vector2>();
	}

    public void Rotate(InputAction.CallbackContext context)
	{
        _moveRotation = context.ReadValue<Vector2>();
	}

    public void Stabalize(InputAction.CallbackContext context)
	{
        if (context.ReadValue<float>() > 0.5)
        {
            _stabalizeNow = true;
		}
		else
		{
            _stabalizeNow = false;
            //StopAllCoroutines();
		}
       
    }

	public void ResetPlayer()
	{
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Junk"))
		{
            _collideWarning.SetActive(true);
            StartCoroutine(Warning(_collideWarning));
            _effectSource.clip = _bonk;
            _effectSource.Play();
        }
        else if (collision.gameObject.CompareTag("Collectable"))
        {
            _collectedItems++;
            collision.gameObject.SetActive(false);
            ResetPlayer();
            if (_collectedItems >= 4)
			{
                _returnText.SetActive(true);
                StartCoroutine(Warning(_returnText));
			}
            _effectSource.clip = _happy;
            _effectSource.Play();
        }
        else if (collision.gameObject.CompareTag("SpaceShip"))
        {
            GameManager.Instance.CollectItems(_collectedItems);
            _collectedItemsDelivered += _collectedItems;
            if (_collectedItemsDelivered >= 4)
			{
                _collectedItems = 0;
                _collectedItemsDelivered = 0;
			}
            ResetPlayer();
        }
        else if (collision.gameObject.CompareTag("EndCube"))
        {
            GameManager.Instance.LoadEnd();
        }
    }

    IEnumerator Warning(GameObject text)
	{
        yield return new WaitForSeconds(4f);
        text.SetActive(false);
	}

    public void ChangeMove(float value)
    {
        _moveSensitivity = value;
    }
    public void ChangeRotate(float value)
    {
        _rotateSensitivity = value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector2 _moveDirection; 
    private Vector2 _moveRotation;
    [SerializeField] private float _moveSensitivity = 200;
    [SerializeField] private float _rotateSensitivity = 800;
    private bool _stabalizeNow;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _stabalizeNow = false;
    }

    // Update is called once per frame
    void Update()
    {
        _rb.AddForce((gameObject.transform.forward * _moveDirection.y + gameObject.transform.right * _moveDirection.x) * Time.deltaTime * _moveSensitivity *100);
        _rb.AddRelativeTorque(new Vector3(-_moveRotation.y, _moveRotation.x, 0)* Time.deltaTime * (_rotateSensitivity /10));
        Debug.Log(new Vector3(-_moveRotation.y, _moveRotation.x, 0) * Time.deltaTime * (_rotateSensitivity / 10));
    }

    void FixedUpdate()
	{
        if (_stabalizeNow)
        {
            _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, 0.04f);
            if (_rb.angularVelocity.magnitude > 0.07f)
			{
                _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, Vector3.zero, 0.0178f);
			}
            else
			{
                _rb.angularVelocity = Vector3.zero;
                StartCoroutine(ResetRotation());
            }
        }
	}

    private IEnumerator ResetRotation()
	{
        yield return new WaitForSeconds(2f);
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0, 0, 0), 0.01f);
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
            StopAllCoroutines();
		}
       
    }
}

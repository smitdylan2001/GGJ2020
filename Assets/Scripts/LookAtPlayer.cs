using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private GameObject _playerReference;

    // Start is called before the first frame update
    void Start()
    {
       _playerReference = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(-_playerReference.transform.position);
        gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x - 180, gameObject.transform.rotation.y -180, gameObject.transform.rotation.z);
    }
}

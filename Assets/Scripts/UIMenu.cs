using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour
{
	public GameObject SpaceShip;
	public GameObject CamGO;
	public void StartButton()
	{
		SceneManager.LoadScene(1);
	}

	private void Update()
	{
		CamGO.transform.RotateAround(SpaceShip.transform.position, Vector3.up, 10*Time.deltaTime);
	}
}

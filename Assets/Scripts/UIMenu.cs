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
	public void ExitButton()
	{
		Application.Quit();
	}
	private void Start()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void Update()
	{
		CamGO.transform.RotateAround(SpaceShip.transform.position, Vector3.up, 10*Time.deltaTime);
	}
}

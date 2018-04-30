using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingCube : MonoBehaviour {
	
	private bool moveLeft = false;
	private bool moveRight = false;
	private bool moveUp = false;
	private bool moveDown = false;

	// Use this for initialization
	public static TrainingCube main;

	void Awake() {
		main = this;
	}
		
	void Start(){

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			moveUp = true;
		} else if (Input.GetKeyUp(KeyCode.UpArrow)) {
			moveUp = false;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			moveDown = true;
		} else if (Input.GetKeyUp(KeyCode.DownArrow)) {
			moveDown = false;
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			moveLeft = true;
		} else if (Input.GetKeyUp(KeyCode.LeftArrow)) {
			moveLeft = false;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			moveRight = true;
		} else if (Input.GetKeyUp(KeyCode.RightArrow)) {
			moveRight = false;
		}


	}
	void FixedUpdate()
	{
		Vector3 newPosition = transform.position;
		if (moveUp)
		{
			newPosition.z = newPosition.z - (0.5f * Time.fixedDeltaTime);
		}
		if (moveDown)
		{
			newPosition.z = newPosition.z + (0.5f * Time.fixedDeltaTime);
		}
		if (moveLeft)
		{
			newPosition.x = newPosition.x + (0.5f * Time.fixedDeltaTime);
		}
		if (moveRight)
		{
			newPosition.x = newPosition.x - (0.5f * Time.fixedDeltaTime);
		}
		transform.position = newPosition;

	}
		
}

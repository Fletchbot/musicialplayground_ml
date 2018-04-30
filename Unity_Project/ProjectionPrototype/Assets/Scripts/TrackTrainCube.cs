using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTrainCube : MonoBehaviour {
	
	public GameObject personManager;
	private PersonManagerScript personManagerScript;
	private TrackedPerson trackedPerson;
	private bool on;
	// Use this for initialization

	void Awake(){
		on = true;
	}

	void Start(){

	}

	// Update is called once per frame
	void Update () {
		
	}
	void FixedUpdate()
	{
		if (on == true) {
			personManagerScript = personManager.GetComponent<PersonManagerScript> ();
			trackedPerson = new TrackedPerson ();
			updatePerson ();
			trackedPerson.id = 60;
			personManagerScript.addPerson (trackedPerson);
			on = false;
		}
		updatePerson ();
	}

	private void updatePerson()
	{
		/*trackedPerson.age = person.age;
		trackedPerson.centroidX = person.centroidX;
		trackedPerson.centroidY = person.centroidY;*/
		trackedPerson.velocityX = 0;
		trackedPerson.velocityY = 0;
		trackedPerson.positionX = transform.position.x;
		trackedPerson.positionY = transform.position.z;
	}
}

/**
 * OpenTSPS + Unity3d Extension
 * Created by James George on 11/24/2010
 * 
 * This example is distributed under The MIT License
 *
 * Copyright (c) 2010 James George
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TSPS;

public class InstrumentManagerScript : MonoBehaviour  {
	//game engine stuff for the example
	public GameObject helmManager;
	public GameObject bpmController;
	private Instrument[] instruments;

	public GameObject drumsEffect;
	public Gradient drumsColour;
	public GameObject leadEffect;
	public Gradient leadColour;
	public GameObject bassEffect;
	public Gradient bassColour;
	public GameObject arpEffect;
	public Gradient arpColour;
	public GameObject droneEffect;
	public Gradient droneColour;
	public GameObject airdroneEffect;
	public Gradient airdroneColour;

	private float currBPM = 120.0f;
	private float newBPM = 120.0f;
	public float bassX, bassY, droneX, droneY, airDroneX, airDroneY, arpX, arpY, drumsX, drumsY, leadX, leadY;
	public float drumsVelX, drumsVelY, bassVelX, bassVelY, droneVelX, droneVelY, airDroneVelX, airDroneVelY, arpVelX, arpVelY, leadVelX, leadVelY;

	private AudioHelm.AudioHelmClock clock;
		
	public static InstrumentManagerScript main;

	void Awake() {
		main = this;
	}
	void Start() {
		clock = bpmController.GetComponent<AudioHelm.AudioHelmClock>();
		// Create an array of instrument objects
		Instrument drums = new Instrument();
		drums.name = "drums";
		drums.color = drumsColour;
		drums.effect = drumsEffect;
		//drums.defaultCol = 0.0f; //collision 0 -1
		Instrument bass = new Instrument();
		bass.name = "bass";
		bass.color = bassColour;
		bass.effect = bassEffect;
		//bass.defaultCol = 0.0f; //collision 0 -1
		Instrument drone = new Instrument();
		drone.name = "drone";
		drone.color = droneColour;
		drone.effect = droneEffect;
		//drone.defaultCol = 1.0f; collision 1 - 0
		Instrument airDrone = new Instrument();
		airDrone.name = "airDrone";
		airDrone.defaultPosX = 1.0f;
		airDrone.defaultPosY = 0.0f;
		airDrone.defaultVelX = 1.0f;
		airDrone.defaultVelY = 0.0f;
		airDrone.color = airdroneColour;
		airDrone.effect = airdroneEffect;
		//airDrone.defaultCol = 0.5f; //collision 0.5 - 0.6?
		Instrument arp = new Instrument();
		arp.name = "arp";
		arp.defaultPosX = 1.0f;
		arp.defaultPosY = 0.0f;
		arp.defaultVelX = 0.5f;
		arp.defaultVelY = 0.5f;
		arp.color = arpColour;
		arp.effect = arpEffect;
		//arp.defaultCol = 0.0f; //collision 0 - 1
		Instrument lead = new Instrument();
		lead.name = "lead";
		lead.defaultPosX = 0.0f;
		lead.defaultPosY = 0.0f;
		lead.defaultVelX = 0.1f;
		lead.defaultVelY = 0.2f;
		lead.color = leadColour;
		lead.effect = leadEffect;
		//lead.defaultCol = 0.5f; //collision 0.5 - 1
		Instrument bpm = new Instrument();
		bpm.name = "bpm";
		bpm.defaultPosX = 1.4f;

		//instruments = new Instrument[6] { drums, bass, drone, airDrone, arp, lead };
		instruments = new Instrument[3] {drums, bass, drone};
		//instruments = new Instrument[1] { bass};
	}
			
	void Update () {
		updateInstruments();
	}

	public void updatePersons(Dictionary<int,TrackedPerson> persons)
	{
		for (int i = 0; i < instruments.Length; i++)
		{
			Instrument instrument = instruments[i];
			if (instrument.personAttached > -1) {
				if (persons.ContainsKey(instrument.personAttached)) {
					instrument.newPosX = persons [instrument.personAttached].centroidX;
					instrument.newPosY = persons [instrument.personAttached].centroidY;
				//	instrument.newPosX = persons [instrument.personAttached].positionX;
				//	instrument.newPosY = persons [instrument.personAttached].positionY;
					instrument.newVelX = persons [instrument.personAttached].velocityX;
					instrument.newVelY = persons [instrument.personAttached].velocityY;
				}
			}
		}
	}
	
	private void updateInstruments()
	{
		for (int i = 0; i < instruments.Length; i++)
		{
			Instrument instrument = instruments[i];

			if (instrument.personAttached == -1) {
				HelmManagerScript hms = helmManager.GetComponent<HelmManagerScript>();
					hms.SnareDelayMix = 0.0f; //posX
					hms.HihatDelayFeedback = 0.0f; //posX combined
					hms.HihatDelayMix = 0.0f; //posY
					hms.SnareDelayFeedback = 0.0f; //posY combined

			} else {
				instrument.currPosX += (instrument.newPosX - instrument.currPosX) / 6; 
				instrument.currPosY += (instrument.newPosY - instrument.currPosY) / 6;
				instrument.currVelX += (instrument.newVelX - instrument.currVelX) / 6; 
				instrument.currVelY += (instrument.newVelY - instrument.currVelY) / 6;
			}

			currBPM += (newBPM - currBPM) / 500;
			clock.bpm = currBPM; 

			switch (instrument.name)
			{
			case "drums":
				updateDrums (instrument.currPosX, instrument.currPosY, instrument.currVelX, instrument.currVelY);
				drumsX = instrument.currPosX;
				drumsY = instrument.currPosY;
				drumsVelX = instrument.currVelX;
				drumsVelY = instrument.currVelY;
					break;
			case "bass":
				updateBass (instrument.currPosX, instrument.currPosY, instrument.currVelX, instrument.currVelY);
				bassX = instrument.currPosX;
				bassY = instrument.currPosY;
				bassVelX = instrument.currVelX;
				bassVelY = instrument.currVelY;
					break;
			case "drone":
				updateDrone (instrument.currPosX, instrument.currPosY, instrument.currVelX, instrument.currVelY);
				droneX = instrument.currPosX;
				droneY = instrument.currPosY;
				droneVelX = instrument.currVelX;
				droneVelY = instrument.currVelY;
					break;
				case "airDrone" :
					updateAirDrone(instrument.currPosX, instrument.currPosY, instrument.currVelX, instrument.currVelY);
				airDroneX = instrument.currPosX;
				airDroneY = instrument.currPosY;
				airDroneVelX = instrument.currVelX;
				airDroneVelY = instrument.currVelY;
					break;
				case "arp" :
					updateArp(instrument.currPosX, instrument.currPosY, instrument.currVelX, instrument.currVelY);
				arpX = instrument.currPosX;
				arpY = instrument.currPosY;
				arpVelX = instrument.currVelX;
				arpVelY = instrument.currVelY;
					break;
				case "lead" :
					updateLead(instrument.currPosX, instrument.currPosY, instrument.currVelX, instrument.currVelY);
				leadX = instrument.currPosX;
				leadY = instrument.currPosY;
				leadVelX = instrument.currVelX;
				leadVelY = instrument.currVelY;
					break;

			}
		}
	}

	public void setInstrumentEnabled(Instrument instrument, bool value = true)
	{
		//Debug.Log (instrument.name + " " + value);
		HelmManagerScript hms = helmManager.GetComponent<HelmManagerScript>();
		switch(instrument.name)
		{
			case "drums" :
				if (value) {
					hms.DrumsEnable();
				} else {
					hms.DrumsDisable();
				}
				break;
			case "bass" :
				if (value) {
					hms.BassEnable();
				} else {
					hms.BassDisable();
				}
				break;
		case "drone":
			if (value) {
				hms.DroneEnable ();
			} else {
				hms.DroneDisable ();
			}
			break;
			case "airDrone" :
				if (value) {
					hms.AirDroneEnable();
				} else {
					hms.AirDroneDisable();
				}
				break;
			case "arp" :
				if (value) {
					hms.ArpEnable();
				} else {
					hms.ArpDisable();
				}
				break;
			case "lead" :
				if (value) {
					hms.LeadEnable();
				} else {
					hms.LeadDisable();
				}
				break;
		}
	}

	public Instrument assignInstrument(int personID)
	{
		List<Instrument> instrumentsNotAssigned = new List<Instrument>();
		for (int i = 0; i < instruments.Length; i++)
		{
			Instrument instrument = instruments[i];
			if (instrument.personAttached == -1)
			{
				instrumentsNotAssigned.Add(instrument);
			}
		}

		if (instrumentsNotAssigned.Count > 0) {
			System.Random rndwer = new System.Random();
			int r = rndwer.Next(instrumentsNotAssigned.Count);
			Instrument instrument = (Instrument)instrumentsNotAssigned[r];
			setInstrumentEnabled (instrument, true);
			instrument.personAttached = personID;
			return instrument;
		} else {
			return null;
		}
	}

	public void updateAssignInstrument()
	{
		for (int i = 0; i < instruments.Length; i++)
		{
			Instrument instrument = instruments[i];
			if (instrument.personAttached > -1) {
				setInstrumentEnabled (instrument, true);
			}

		/*for (int i = 0; i < instruments.Length; i++)
		{
			Instrument instrument = instruments[i];
			if (instrument.personAttached == oldId)
			{
				instrument.personAttached = newId;
				break;
			}
		}*/
	}
	}

	public void removeInstrument(int personID)
	{
	//	List<Instrument> instrumentsNotAssigned = new List<Instrument>();
		for (int i = 0; i < instruments.Length; i++)
		{
			Instrument instrument = instruments[i];
			if (instrument.personAttached == personID)
			{
				setInstrumentEnabled (instrument, false);
				instrument.personAttached = -1;
				break;
			}
		}
	}

	public void instrumentUpdate(Instrument instrument, float npx, float npy, float nvx, float nvy)
	{	
		if (instrument != null) {
			instrument.newPosX = npx;
			instrument.newPosY = npy;
			instrument.newVelX = nvx;
			instrument.newVelY = nvy;
		}
	}

	private void updateDrums(float npx, float npy, float nvx, float nvy)
	{
		
	
	//	hms.SnareDelaySync = mapValue(nvx, 0.4f, 0.45f); //velX
	//	hms.HihatDelaySync = mapValue(nvy, 0.4f, 0.45f); //velY
		//hms.CymbalHitEnable(); //collision
	}

	private void updateBass(float npx, float npy, float nvx, float nvy)
	{
		HelmManagerScript hms = helmManager.GetComponent<HelmManagerScript>();
		//hms.BassFeedbackTune = MachineLearningGridController.main.;
	//	hms.BassFeedbackAmount = mapValue(npy, 0.0f, 1.0f);
		//hms.BassSubShuffle = mapValue(nvx, 0.0f, 1.0f);
		//hms.BassOSC2tune = mapValue(nvy, 0.0f, 1.0f);
		//hms.BassReso = mapValue(?, 0.0f, 1.0f); //collision
	}

	private void updateDrone(float npx, float npy, float nvx, float nvy)
	{
		HelmManagerScript hms = helmManager.GetComponent<HelmManagerScript>();
		//hms.DroneX = mapValue(npx, 0.0f, 0.9f);
		//hms.DroneY = mapValue(npy, 0.9f, 0.0f);
		//hms.DroneFeedback = mapValue (nvx, 0.0f, -0.1f);
		//hms.DroneMod = mapValue (nvy, 0.0f, 0.2f);
		//hms.AirDronefilterBlend = mapValue (? , 1.0f, 0.0f); //collision
	}

	private void updateAirDrone(float npx, float npy, float nvx, float nvy)
	{
		HelmManagerScript hms = helmManager.GetComponent<HelmManagerScript>();
		hms.AirDroneX = mapValue(npx, 1.0f, 0.0f);
		hms.AirDroneY = mapValue(npy, 0.0f, 1.0f);
		//hms.AirDronefilterBlend = mapValue(nvx, 1.0f, 0.0f);
		//hms.AirDroneArpOn = mapValue(nvy, 0.0f, 1.0f);
		//hms.AirDroneDelayTempo = mapValue(nvx, 0.0f, 0.0f); //collision
	}

	private void updateArp(float npx, float npy, float nvx, float nvy)
	{
		HelmManagerScript hms = helmManager.GetComponent<HelmManagerScript>();
		hms.ArpStutter = mapValue(npx, 0.0f, 1.0f); //posX
		hms.ArpSub = mapValue(npy, 0.0f, 1.0f); //posY
	//	hms.ArpFeedback = mapValue(nvx, 0.5f, 0.6f); //velX
	//	hms.ArpDelayFeedback = mapValue(nvy, 0.5f, 0.6f); //velY
		//hms.ArpSustain = mapValue(?, 0.0f, 1.0f); //collision
	}

	private void updateLead(float npx, float npy, float nvx, float nvy)
	{
		HelmManagerScript hms = helmManager.GetComponent<HelmManagerScript>();
		hms.LeadMix = mapValue(npx, 0.0f, 1.0f); //posX
		hms.LeadSub = mapValue(npy, 1.0f, 0.0f); //posY
		//hms.LeadSustain = mapValue(npx, 0.1f, 0.5f); //velX
		//hms.LeadAttack = mapValue(npy,0.0f,0.2f); //velY
	//	hms.LeadDelaySync = mapValue(?, 0.5f, 1.0f); //collision ?
	//	hms.LeadDelayTempo = mapValue(?, 0.5f, 1.0f); //collision combined ?

	}
	/*private void updateBPM(float npx, float npy, float nvx, float nvy)
	{
		AudioHelm.AudioHelmClock clock = bpmController.GetComponent<AudioHelm.AudioHelmClock>();
		clock.bpm = mapValue (npx, -10.0f, 90.0f) + mapValue (npy,-10.0f, 90.0f);
	}*/

	public void updateBPM(float vel)
	{
	//	newBPM = mapValue (vel, 20.0f, 400.0f);
		//Debug.Log(newBPM);
	}

	private float mapValue(float value, float vmin, float vmax)
	{
		return vmin + ((vmax - vmin) * value);
	}
}

public class Instrument
{
	public string name = "";
	public int personAttached = -1;
	public float defaultPosX = 0.0f;
	public float defaultPosY = 0.0f;
	public float newPosX = 0.0f;
	public float newPosY = 0.0f;
	public float currPosX = 0.0f;
	public float currPosY = 0.0f;
	public float defaultVelX = 0.0f;
	public float defaultVelY = 0.0f;
	public float newVelX = 0.0f;
	public float newVelY = 0.0f;
	public float currVelX = 0.0f;
	public float currVelY = 0.0f;

	public GameObject effect;
	public Gradient color;
}
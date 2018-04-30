using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineLearningGridController : MonoBehaviour {
	public int[] input = new int[1];
	[SerializeField] float xSize = 1f;
	[SerializeField] float ySize = 1f;
	[SerializeField] float spacing = 0.1f;

	public static MachineLearningGridController main;
	public GameObject cubeCollectorPrefab;
    public List<CubeCollectorController> collectorList = new List<CubeCollectorController>();
    public OSC osc;
   	private float MsgTimer  = 0.0f;
    [SerializeField] float MessageInterval = 1.0f;
    [SerializeField] bool useWekinator = false;
//	HelmManagerScript hm = HelmManagerScript.main;
//	InstrumentManagerScript im = InstrumentManagerScript.main;
	private int m_BassNote, m_DroneNote, m_KickNote, m_SnareNote, m_HihatNote;
	public int bassNote, droneNote, kickNote, snareNote, hihatNote;

	public delegate void OnVariableChangeDelegate(int newVal);
	public event OnVariableChangeDelegate OnVariableChange;

	void Awake() {
		main = this;
	}
	void Start () {
		OnVariableChange += VariableChangeHandler;
		//Wekinator Outputs
		osc.SetAddressHandler( "/wek/outputs" , OnReceive );

        // instantiating collector cubes for machine learning.
		float xPos,yPos = 0;

		for (float i = 0; i < xSize ; i += spacing) {
			xPos = i;
			for (float ii = 0; ii < ySize ; ii += spacing) {
				yPos = ii;

				Vector3 position = new Vector3 (xPos, 0, yPos);

				GameObject inst = Instantiate (cubeCollectorPrefab, position, Quaternion.identity);
                collectorList.Add(inst.GetComponent<CubeCollectorController>());

			}
		}
	}
	private void VariableChangeHandler(int newVal)
	{
		//Debug.Log ("Note Change");
		InstrumentManagerScript.main.updateAssignInstrument();
	}

	void OnReceive(OscMessage message){
		//Bass Note Classifier
		HelmManagerScript.main.bassnote = (int)message.GetFloat(0);
 		//Bass Model
		HelmManagerScript.main.BassSubShuffle = message.GetFloat (1); 
		HelmManagerScript.main.BassFeedbackTune = message.GetFloat (1); 
		HelmManagerScript.main.BassFeedbackAmount = message.GetFloat (2);
		HelmManagerScript.main.BassOSC2tune = message.GetFloat (2);
		HelmManagerScript.main.BassReso = message.GetFloat (3);
		//Drone Note Classifier
		HelmManagerScript.main.dronenote = (int)message.GetFloat (4);
		//Drone Model
		HelmManagerScript.main.DroneX = message.GetFloat (5);
		HelmManagerScript.main.DroneY = message.GetFloat (5);
		HelmManagerScript.main.DroneFeedback = message.GetFloat (6);
		HelmManagerScript.main.DroneMod = message.GetFloat (6);
		HelmManagerScript.main.DronefilterBlend = message.GetFloat (7);
		//Drum Note Classifier
		HelmManagerScript.main.kicknote = (int)message.GetFloat (8);
		HelmManagerScript.main.snarenote = (int)message.GetFloat (9);
		HelmManagerScript.main.hihatnote = (int)message.GetFloat (10);
		//Drum Model
		HelmManagerScript.main.SnareDelayMix = message.GetFloat (11);
		HelmManagerScript.main.SnareDelayFeedback = message.GetFloat (11);
		HelmManagerScript.main.SnareDelaySync = message.GetFloat (12);
		HelmManagerScript.main.HihatDelayMix = message.GetFloat (13);
		HelmManagerScript.main.HihatDelayFeedback = message.GetFloat (13);
		HelmManagerScript.main.HihatDelaySync = message.GetFloat (14);
		HelmManagerScript.main.DrumPitch = message.GetFloat (15);


		bassNote = HelmManagerScript.main.bassnote;
		droneNote = HelmManagerScript.main.dronenote;
		kickNote = HelmManagerScript.main.kicknote;
		snareNote = HelmManagerScript.main.snarenote;
		hihatNote = HelmManagerScript.main.hihatnote;

		//Debug.Log ("BassNote = " + bassNote );
		//Debug.Log ("DroneNote = " + droneNote );
		//Debug.Log ("KickNote = " + kickNote );
		//Debug.Log ("SnareNote = " + snareNote );
		//Debug.Log ("HihatNote = " + hihatNote );
		//Debug.Log ("DrumsVel = " + HelmManagerScript.main.HihatDelaySync);

	
	}

	private void Update()
	{
		if (bassNote != m_BassNote && OnVariableChange != null) {
			m_BassNote = bassNote;
			OnVariableChange (bassNote);
		}
		if (droneNote != m_DroneNote && OnVariableChange != null) {
			m_DroneNote = droneNote;
			OnVariableChange (droneNote);
		}
		if (kickNote != m_KickNote && OnVariableChange != null) {
			m_KickNote = kickNote;
			OnVariableChange (kickNote);
		}
		if (snareNote != m_SnareNote && OnVariableChange != null) {
			m_SnareNote = snareNote;
			OnVariableChange (snareNote);
		}
		if (hihatNote != m_HihatNote && OnVariableChange != null) {
			m_HihatNote = hihatNote;
			OnVariableChange (hihatNote);
		}


//		SendMessage ();
		if (MsgTimer < Time.time) {
			SendMessage();
            
            MsgTimer = Time.time + MessageInterval;
		}
	}

	void SendMessage() {
        
        if (useWekinator)
        {
            OscMessage message = new OscMessage();
            message.address = "/wek/inputs";
			//Bass
			message.values.Add (InstrumentManagerScript.main.bassX); //1
			message.values.Add (InstrumentManagerScript.main.bassY); //2
			message.values.Add (InstrumentManagerScript.main.bassVelX); //3
			message.values.Add (InstrumentManagerScript.main.bassVelY); //4
			//Drone
			message.values.Add (InstrumentManagerScript.main.droneX); //5
			message.values.Add (InstrumentManagerScript.main.droneY); //6
			message.values.Add (InstrumentManagerScript.main.droneVelX); //7
			message.values.Add (InstrumentManagerScript.main.droneVelY); //8
			//Drums
			message.values.Add (InstrumentManagerScript.main.drumsX); //9
			message.values.Add (InstrumentManagerScript.main.drumsY); //10
			message.values.Add (InstrumentManagerScript.main.drumsVelX); //11
			message.values.Add (InstrumentManagerScript.main.drumsVelY); //12

			// 197 is the limit for sending messages
//			for (int i = 0; i < collectorList.Count; i++) 
  //          {
    //            message.values.Add(collectorList[i].weightValue);
      //      }

            osc.Send(message);
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.34209
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InstrumentMode {
	AIControlled,
	PlayerControlled
}

public class InstrumentController : UserManagementObjectController
{
	public KeyCode gestureKey;
	const float recordingUpdateFrameLengthMS = 100; // in MS
	private int currentRecordingUpdateFrame = 0; // the current frame within the recording period
	private int lastRecordingFrame = 0;
	public const float NoPitchChange = -13f;
	public const float NoVolumeChange = -1f;
	private class PitchAndVolume {
		public float pitch = NoPitchChange;
		public float volume = NoVolumeChange;
	}
	private Dictionary<int, PitchAndVolume> recordingData = null; // = new ArrayList<PitchAndVolume>(); // allow recording up to 20 seconds
	private int recordingDataLength = 0;
	public InstrumentMode _mode = InstrumentMode.AIControlled;
	[RPC]
	public void setMode(int mode) {
		this.mode = (InstrumentMode)mode;
	}
	public InstrumentMode mode {
		get {
			return _mode;
		}
		set {
			_mode = value;

			if (value == InstrumentMode.PlayerControlled) {
				//this.gameObject.renderer.material.color = Color.red;
				Debug.Log (this.instrumentName + " is now PLAYER CONTROLLED");
				// start a new recording buffer
				recordingData = new Dictionary<int, PitchAndVolume>();
				currentRecordingUpdateFrame = 0;
				lastRecordingFrame = 0;
				recordingDataLength = 0;
			}
			else {
				//this.gameObject.renderer.material.color = Color.white;
				Debug.Log (this.instrumentName + " is now back to being AI CONTROLLED");
				currentRecordingUpdateFrame = 0;
				recordingDataLength = lastRecordingFrame;
			}
		}
	}
	public string midiPlayerName = "MidiPlayer";
	public string instrumentName = "";

	private GameObject avatarParent = null;
	private AvatarObjectController avatarObjectController = null;

	private OrchestraMidiPlayer _midiPlayer = null;
	private OrchestraMidiPlayer midiPlayer {
		get {
			if (_midiPlayer == null) _midiPlayer =  (OrchestraMidiPlayer)(GameObject.Find(midiPlayerName).GetComponent<OrchestraMidiPlayer>()); 
			return _midiPlayer;
		}
	}
	private InstrumentType _instrumentType = InstrumentType.NoInstrument;
	public InstrumentType instrumentType {
		get {
			if (_instrumentType == InstrumentType.NoInstrument) {
				InstrumentType iT = InstrumentTypeHelpers.stringToInstrumentType(instrumentName);
				if (iT != null) {
					_instrumentType = iT;
				}
			}
			return _instrumentType;

		}

	}
	private AudioSource audioSourceComponent;
	public void Start() {
		//audioSourceComponent = this.GetComponent<AudioSource>();

		inputListenerScript = GameObject.Find ("InputListenerObject").GetComponent<GestureInputListener>();

		InstrumentManager.instance.allInstruments.Add (this.gameObject);
		Debug.Log ("Instrument count: " + InstrumentManager.instance.allInstruments.Count);
		
		avatarParent = GameObject.Find ("avatarParent");
		avatarObjectController = avatarParent.GetComponent<AvatarObjectController> ();
	}
	public InstrumentController ()
	{
	}
	private bool _playing;
	public bool playing {
		get {
			return _playing;
		}
		set {
			_playing = value;
			if (value) {
				//startPlaying();
				networkView.RPC ("startPlayingRPC", RPCMode.All);

			}
			else {
				//stopPlaying ();
				networkView.RPC ("stopPlayingRPC", RPCMode.All);
			}
		}
	}
	
	private float pitchThreshold = 0.25f;
	private float pitchSpeed = 0.5f;
	private GestureInputListener inputListenerScript = null;
	//public float rot;

	[RPC]
	public void startPlayingRPC() {
		//midiPlayer.muteInstrument(
		Debug.Log (this.instrumentType);
		midiPlayer.unmuteInstrument (this.instrumentType);
		if (!(midiPlayer.isPlaying) && midiPlayer.readyToPlay) {
			// currently not playing. start playing.
			midiPlayer.Play();
		}

		this.setAccessGrantedName ("player1");
	} 
	[RPC]
	public void stopPlayingRPC() {
		midiPlayer.muteInstrument (this.instrumentType);
		this.setAccessGrantedName ("player2");
	} 
	private int counter = 0;
	// don't save values for every iteration, just every ten 
	public void FixedUpdate() {
		counter++;
		if (counter < 10)
			return;
		else
			counter = 0;


		if (this.mode == InstrumentMode.PlayerControlled) { 

			currentRecordingUpdateFrame++;
		} else if (this.mode == InstrumentMode.AIControlled && recordingData != null) {
			currentRecordingUpdateFrame++;
			if (currentRecordingUpdateFrame > lastRecordingFrame) currentRecordingUpdateFrame = 0;

			// we have recording data, so we need to make sure to set pitch & volume accordingly
			PitchAndVolume pav;
			if (recordingData.TryGetValue(currentRecordingUpdateFrame, out pav)) {
				if (pav.pitch != NoPitchChange) this.networkView.RPC ("SetPitchRPC", RPCMode.All, pav.pitch);
				if (pav.volume != NoVolumeChange) this.networkView.RPC ("setVolumeRPC", RPCMode.All, pav.volume);

			}


		}

	}

	private int numberOfKeypressesSinceLastCheck = 0;
	private float nextCheckAtTime = 0;
	public float keypressCheckInterval = 2f;
	public int maxVolumeRate = 5; // number of triggers per interval to achive max. volume

	public void Update()
	{

		if (Network.isClient && (inputListenerScript.selectedViewID.Equals (networkView.viewID) || gameObject == avatarObjectController.selectedInstrument)) {
			//pitch range should go from 1/3 to 3 (three times slower to three times faster)

			//change the pitch according to the SpaceMouse y Axis rotation
			float maxPitchBend = 12f;
			float minPitchBend = -12f;
			float currentPitch = midiPlayer.getPitchBendForInstrument (this.instrumentType);

			GameObject sp = GameObject.Find ("Spacemouse");
			if (sp == null)
				return;
			//rot = sp.transform.rotation.y;

			if (sp.transform.rotation.y > pitchThreshold) {
				//higher pitch!

				//change pitch in 1/3 of the speed
				float actualPitchSpeed = (currentPitch >= 0) ? pitchSpeed : pitchSpeed / 3f;
				float newPitch = Math.Min (currentPitch + (actualPitchSpeed * Time.deltaTime), maxPitchBend);
				networkView.RPC ("SetPitchRPC", RPCMode.All, newPitch);

			
			}
			if (sp.transform.rotation.y < -pitchThreshold) {
				//lower pitch!
				float actualPitchSpeed = (currentPitch >= 0) ? pitchSpeed : pitchSpeed / 3f;


				float newPitch = Math.Max (currentPitch - (actualPitchSpeed * Time.deltaTime), minPitchBend);
				networkView.RPC ("SetPitchRPC", RPCMode.All, newPitch);
			}

			//now make an RPC call to the server to send the new pitch value
			//networkView.RPC ("SetPitchRPC", RPCMode.Server, audioSourceComponent.pitch);
		} else if (Network.isServer) {
			if (this.mode == InstrumentMode.PlayerControlled) {
				if (Time.time >= nextCheckAtTime) {
					// time to check how many keypresses we've had
					if (numberOfKeypressesSinceLastCheck > 0) {
						float newVolume = Mathf.Clamp((1/maxVolumeRate) * numberOfKeypressesSinceLastCheck, 0, 1);
						this.networkView.RPC ("setVolumeRPC", RPCMode.All, newVolume);
					}
					else {
						// no keypresses at all! zero volume
						this.networkView.RPC ("setVolumeRPC", RPCMode.All, 0f);
					}
					numberOfKeypressesSinceLastCheck = 0;
					nextCheckAtTime = Time.time + keypressCheckInterval;


				}
				if (Input.GetKeyDown(this.gestureKey)) {
					numberOfKeypressesSinceLastCheck++;
				} 
			}
		}
	}

	[RPC]
	public void SetPitchRPC(float pitchValue)
	{
		// record the action
		if (this.mode == InstrumentMode.PlayerControlled && Network.isServer) {
			PitchAndVolume pav = new PitchAndVolume();
			pav.pitch = pitchValue;
			pav.volume = NoVolumeChange;

			recordingData[currentRecordingUpdateFrame] = pav;
			lastRecordingFrame = currentRecordingUpdateFrame;
		}
		midiPlayer.setPitchBendForInstrument (this.instrumentType, pitchValue);
	}
	[RPC]
	public void setVolumeForAllInstrumentsRPC(float volume) {
		InstrumentManager.instance.setVolumeForAllInstruments(volume);

	}
	[RPC]
	public void setVolumeRPC(float volume) {
		// record the action
		if (this.mode == InstrumentMode.PlayerControlled && Network.isServer) {
			PitchAndVolume pav = new PitchAndVolume();
			pav.pitch = NoPitchChange;
			pav.volume = volume;
			lastRecordingFrame = currentRecordingUpdateFrame;
			recordingData[currentRecordingUpdateFrame] = pav;
			
		}
		//audioSourceComponent.volume = volume;
		midiPlayer.setVolumeForInstrument (this.instrumentType, volume);
	}
}



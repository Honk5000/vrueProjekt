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

public class InstrumentController : UserManagementObjectController
{
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

	public void Update()
	{
		if(Network.isClient && (inputListenerScript.selectedViewID.Equals(networkView.viewID) || gameObject == avatarObjectController.selectedInstrument) )
		{
			//pitch range should go from 1/3 to 3 (three times slower to three times faster)

			//change the pitch according to the SpaceMouse y Axis rotation
			float maxPitchBend = 12f;
			float minPitchBend = -12f;
			float currentPitch = midiPlayer.getPitchBendForInstrument(this.instrumentType);

			GameObject sp = GameObject.Find("Spacemouse");
			if (sp == null) return;
			//rot = sp.transform.rotation.y;

			if (sp.transform.rotation.y > pitchThreshold)
			{
				//higher pitch!

				//change pitch in 1/3 of the speed
				float actualPitchSpeed = (currentPitch >= 0) ? pitchSpeed : pitchSpeed/3f;
				float newPitch = Math.Min( currentPitch + (actualPitchSpeed * Time.deltaTime), maxPitchBend);
				networkView.RPC ("SetPitchRPC", RPCMode.All, newPitch);

			
			}
			if(sp.transform.rotation.y < -pitchThreshold)
			{
				//lower pitch!
				float actualPitchSpeed = (currentPitch >= 0) ? pitchSpeed : pitchSpeed/3f;


				float newPitch = Math.Max( currentPitch - (actualPitchSpeed * Time.deltaTime), minPitchBend);
				networkView.RPC ("SetPitchRPC", RPCMode.All, newPitch);
			}

			//now make an RPC call to the server to send the new pitch value
			//networkView.RPC ("SetPitchRPC", RPCMode.Server, audioSourceComponent.pitch);
		}
	}

	[RPC]
	public void SetPitchRPC(float pitchValue)
	{
		midiPlayer.setPitchBendForInstrument (this.instrumentType, pitchValue);
	}
	[RPC]
	public void setVolumeForAllInstrumentsRPC(float volume) {
		InstrumentManager.instance.setVolumeForAllInstruments(volume);

	}
	[RPC]
	public void setVolumeRPC(float volume) {
		//audioSourceComponent.volume = volume;
		midiPlayer.setVolumeForInstrument (this.instrumentType, volume);
	}
}



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
using System.Collections.Generic;

public class InstrumentManager : ScriptableObject
{
	static readonly object padlock = new object();
	private static InstrumentManager manager;

	protected InstrumentManager () // don't allow instantiation of this object *from the outside*
	{
	}

	public static InstrumentManager instance
	{
		get
		{
			lock (padlock)
			{
				return (manager ? manager : manager = new InstrumentManager());
			}
		}
	}

	private List<GameObject> _allInstruments ;
	public List<GameObject> allInstruments {
		get {
			if (_allInstruments == null) {
				_allInstruments = new List<GameObject>();

			}
			return _allInstruments;
		}

	}

	// set a common volume for all instruments
	public void setVolumeForAllInstruments(float volume) {
		foreach (GameObject instrument in this.allInstruments) {
			instrument.GetComponent<AudioSource>().volume = volume;
		}
	}
}


/* =====================================================================================
 * ARTiFICe - Augmented Reality Framework for Distributed Collaboration
 * ====================================================================================
 * Copyright (c) 2010-2012 
 * 
 * Annette Mossel, Christian Schönauer, Georg Gerstweiler, Hannes Kaufmann
 * mossel | schoenauer | gerstweiler | kaufmann @ims.tuwien.ac.at
 * Interactive Media Systems Group, Vienna University of Technology, Austria
 * www.ims.tuwien.ac.at
 * 
 * ====================================================================================
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * =====================================================================================
 */

using UnityEngine;
using System.Collections;
using System;
	
/// <summary>
/// Links data from a TrackBone to the Avatar Skript 
/// </summary>
[System.Serializable]
public class Limb
{
  	public string name="";//deviceName of the limb as in Opentracker.xml
	public Transform transf=null; //GameObject in the hierarchy of the character
	private string spawnParentObjectName="TrackingCamera";
	private Quaternion initRot=Quaternion.identity;//initial rotation, stored because it might be non-identity
	private GameObject gO=null;//GameObject the TrackBone Skript is attached to
	private string avatarName="";//Name of the Character, not used yet, init to "user"
	private bool initialized=false;
	private bool useRotation=true;
	private bool useTranslation=false;
	/// <summary>
	/// Ctor of the Limb 
	/// </summary>
	/// <param name="pavatarName">
	/// avatar name <see cref="System.String"/>
	/// </param>
	/// <param name="pname">
	/// limb name <see cref="System.String"/>
	/// </param>
	/// <param name="ptransf">
	/// GameObject from the Character hierarchy <see cref="Transform"/>
	/// </param>
	public Limb(string pavatarName,string pname,Transform ptransf)
	{
		name=pname;
		transf=ptransf;
		avatarName=pavatarName;
	}
	public bool isInitialized()
	{
		return initialized;
	}
	/// <summary>
	/// Initializes the Tracking for a single Limb 
	/// </summary>
	public void StartTrackingLimb()
	{
		string gOName=avatarName+"/"+name;
		gO=new GameObject(gOName);
		GameObject avatarObj=GameObject.Find(avatarName);
		//Creates an avatarObject we can append the TrackBone Objects to
		if(!avatarObj)
		{
			avatarObj=new GameObject(avatarName);
			GameObject spawnParentObject=GameObject.Find(spawnParentObjectName);
			if(spawnParentObject)
			{
				avatarObj.transform.parent=spawnParentObject.transform;
			}
		}
		//Attach the TrackBone object below the Avatar object
		if(avatarObj)
		{
			gO.transform.parent=avatarObj.transform;
		}
	
		//Initialize and add the TrackBone Component
		if(gO)
		{
			if(transf)
			{
				initRot=transf.rotation;
				TrackBone tB=gO.AddComponent<TrackBone>();
				tB.deviceName=name;
			}
		}
		else
		{
			Debug.Log("Error: Limb::StartTrackingLimb: gO null");
		}
		initialized=true;
	}
	/// <summary>
	/// Called every frame. Update with new tracking data 
	/// </summary>
	public void Update()
	{
		if(gO&&initialized)
		{
			TrackBone tB=gO.GetComponent<TrackBone>();
			if(tB)
			{
				tB.UpdateTracker();
				if(tB.isTracked())
				{
					if(useRotation)
					{
						transf.rotation=gO.transform.rotation*initRot;
					}
					if(useTranslation)
					{
						transf.localPosition=gO.transform.position;
					}
				}
			}
		}
	}	
	public Transform getTrackedTransform()
	{
		if(gO)
		{
			return gO.transform;
		}
		return null;
	}
}


/// <summary>
/// Provides tracking data received from Kinect to Unity3D game objects.  
/// </summary>
public class Avatar : MonoBehaviour {
	//Array of Limbs that is used to hold the GameObjects of the Character, which are to be transformed
	public Transform waistTranslationObject=null;
	public Limb[] limbArray= new Limb[] { 
								new Limb("user","Head",null),
								new Limb("user","Neck",null),
								new Limb("user","Torso",null),
								new Limb("user","Waist",null),
								new Limb("user","LeftCollar",null),
								new Limb("user","LeftShoulder",null),
								new Limb("user","LeftElbow",null),
								new Limb("user","LeftWrist",null),
								new Limb("user","LeftHand",null),
								new Limb("user","LeftFingertip",null),
								new Limb("user","RightCollar",null),
								new Limb("user","RightShoulder",null),
								new Limb("user","RightElbow",null),
								new Limb("user","RightWrist",null),
								new Limb("user","RightHand",null),
								new Limb("user","RightFingertip",null),
								new Limb("user","LeftHip",null),
								new Limb("user","LeftKnee",null),
								new Limb("user","LeftAnkle",null),
								new Limb("user","LeftFoot",null),
								new Limb("user","RightHip",null),
								new Limb("user","RightKnee",null),
								new Limb("user","RightAnkle",null),
								new Limb("user","RightFoot",null)
								};
	
	/*	
	 * Mapping of indices and VRPN station numbers to limbs
	0	Head	 	12	Right Elbow
	1	Neck	 	13	Right Wrist
	2	Torso	 	14	Right Hand
	3	Waist	 	15	Right Fingertip
	4	Left Collar	 	16	Left Hip
	5	Left Shoulder	 	17	Left Knee
	6	Left Elbow	 	18	Left Ankle
	7	Left Wrist	 	19	Left Foot
	8	Left Hand	 	20	Right Hip
	9	Left Fingertip	 	21	Right Knee
	10	Right Collar	 	22	Right Ankle
	11	Right Shoulder	 	23	Right Foot
		*/
	private bool initialized=false;//True if StartTrackingLimb has been called for all limbs

	/// <summary>
    /// Use Start() for initialization
    /// </summary>
	void Start () 
	{
		//reset root structure rotation so we dont store it in the initial rotation
        Quaternion rotOriginal = transform.rotation;
        transform.rotation = Quaternion.identity;

		//iterate over all limbs and initialize them
		foreach(Limb l in limbArray)
    	{
			l.StartTrackingLimb();//create a TrackBone script for each entry that has a transform attached to it
		}
		initialized=true;

		//set rotation of the root back to what it was
        transform.rotation = rotOriginal;
		
	}
	
	/// <summary>
    /// Update is called once per frame
    /// </summary>
	void Update () 
	{
		NetworkView nV=this.GetComponent<NetworkView>();
		if((!nV)||(nV.isMine))
		{
			//reset Avatar object rotation, so we deont compensate for rotation twice
            Quaternion rotOriginal = transform.rotation;
            transform.rotation = Quaternion.identity;
			
			if(initialized)
			{
				//Update all Limbs
				foreach(Limb l in limbArray)
			    {			
					if(l.transf)
					{
						l.Update();
					}
			    }
			}
			
			//repeat the parent rotation, which we have compensated for
			transform.rotation = rotOriginal;
			//set tracked translation
			if(waistTranslationObject)
			{
				waistTranslationObject.localPosition=limbArray[3].getTrackedTransform().localPosition;
			}
		}			
	}
	void OnEnable()
	{
		Debug.Log("Avatar Test");
	}
}

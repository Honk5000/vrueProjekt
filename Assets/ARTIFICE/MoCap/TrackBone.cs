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
using System;

/// <summary>
/// Provides tracking data received from Kinect/VRPN-Source toUnity3D game objects. 
/// Inherits from abstract class TrackProvider. 
/// </summary>
public class TrackBone : TrackProvider 
{

    /// <summary>
    /// Time which must pass without receiving new tracking data for this device until attached game
    /// object will be invisible.
    /// [ms]
    /// 
    /// Set this value in the derived class or directly in the script's property view in Unity3D
    /// </summary>
    public double deviceTimeout = 200.0;
	private bool isStarted=false;
	private bool independent=true;
	
	//true if used independently from Avatar.cs
	public bool isIndependent
    {
        get
        {
            return independent;
        }
		set
        {
            independent=isIndependent;
        }
    }
	
    public bool deviceTracked
    {
        get
        {
            return isTracked();
        }
    }

    /// <summary>
    /// Use Start() for initialization
    /// </summary>
	public void StartTracker() 
    {
        // generate tracker object for this bone
        generateTracker(deviceName);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public void UpdateTracker() 
    {
		if(!isStarted)
		{
			StartTracker();
			isStarted=true;
		}

		try 
        {
            // get current tracking event
            TrackingEvent ev = updateTracking();
			if(this.isTracked())
			{
				Vector3 position;
				Quaternion orientation;
				transformOpenTrackerToUnity3D(ev,out position,out orientation);
				//Kinect specific transformations : put that in Opentracker at some point
					NormalizeQuaternion(ref orientation);
					// Z coordinate in OpenNI is opposite from Unity. We will create a quat
			        // to rotate from OpenNI to Unity (relative to initial rotation)
					//Vector3 scaleUnity = new Vector3(1,1,1);
			
					/*Matrix4x4 matRot = Matrix4x4.TRS(Vector3.zero, orientation, scaleUnity);
					Vector3 worldZVec = new Vector3(-matRot[2], -matRot[6], matRot[10]);
			    	Vector3 worldYVec = new Vector3(matRot[1], matRot[5], -matRot[9]);
			    	orientation = Quaternion.LookRotation(worldZVec, worldYVec);*/
				
				Vector3 axis=new Vector3();
				float angle=0.0f;
				orientation.ToAngleAxis(out angle,out axis);
				
				axis.x=-axis.x;
				axis.y=-axis.y;
				axis.z=axis.z;
				
				orientation=Quaternion.AngleAxis(angle,axis);
				////////////////////
				
				transform.localPosition = position;
				transform.localRotation = orientation;
			}
        } catch (System.Exception ex) 
        {
			Console.WriteLine("{0} Detailed Exception:", ex);
        }
		
    }
	
	void Update()
	{
		if(isIndependent)
		{
			UpdateTracker();
		}
	}

    /// <summary>
    /// Check if new Bone tracking data is received until device timout is reached.
    /// Tracking event data is only upated as long as bone is visible in video image.
    /// </summary>
	public override bool isTracked() 
    {
		return (Tracking.getTime() - trackingTimestamp) < deviceTimeout;
	}

	
	public static void NormalizeQuaternion (ref Quaternion q)
	{
		float sum = 0;
		for (int i = 0; i < 4; ++i)
			sum += q[i] * q[i];
		float magnitudeInverse = 1 / Mathf.Sqrt(sum);
		for (int i = 0; i < 4; ++i)
			q[i] *= magnitudeInverse;	
	}
}
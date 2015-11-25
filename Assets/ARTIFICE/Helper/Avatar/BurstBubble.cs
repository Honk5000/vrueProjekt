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

/// <summary>
/// Destroys the distributed object it is attached to, if it is hit by another rigidbody
/// </summary>
public class BurstBubble : MonoBehaviour {
	private float initTime=0.0f;
	void Start()
	{
		initTime=Time.realtimeSinceStartup;
	}
	// Callback called if some object enters the trigger body
	void OnTriggerEnter(Collider other) 
	{
		if((Time.realtimeSinceStartup-initTime)>1.0f)
		{
			Debug.Log("OnTriggerEnter Bubble"+(Time.realtimeSinceStartup-initTime).ToString());
			
			if(Network.isServer)
			{
				Network.Destroy(this.gameObject.networkView.viewID);
			}

		}

	}
}

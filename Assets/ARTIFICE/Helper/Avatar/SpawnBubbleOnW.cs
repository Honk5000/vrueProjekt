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
/// Instanitates a prefab if Key "W" is hit
/// </summary>
public class SpawnBubbleOnW : MonoBehaviour {
	public Transform DropPrefab;

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.W))
		{
			if(!GameObject.Find("Bubble(Clone)"))
			{
				GameObject trackerObj=GameObject.Find("TrackerObject");
				Vector3 trackerObjPos=trackerObj.transform.position;
				Network.Instantiate(DropPrefab,trackerObjPos,Quaternion.identity,0);
			}
		}
	}
}

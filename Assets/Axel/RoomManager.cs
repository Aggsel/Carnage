﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [HideInInspector] [SerializeField] public Bounds roomBounds; //Could be used as the room size. Is handled by level manager atm. 

    //Uses the rooms colliders to generate a bounding box, encapsulating the entire room.
    //This is slow, only for use in editor. Bounding box is then stored in the prefab.
    [ContextMenu("Get Bounding Box")]
    private void GetBoundingBox(){
        Collider[] colliders = GetComponentsInChildren<Collider>();
        Bounds newBounds = new Bounds(new Vector3(0,0,0), new Vector3(0,0,0));
        for(int i = 0; i < colliders.Length; i++){
            newBounds.Encapsulate(colliders[i].bounds);
        }
        roomBounds = newBounds;
    }
}
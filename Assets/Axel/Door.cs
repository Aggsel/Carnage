using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Tooltip("Reference to a door prefab.")]
    [SerializeField] private GameObject doorObject = null;
    [Tooltip("Reference to a wall prefab.")]
    [SerializeField] private GameObject wallObject = null;
    private GameObject setObject = null;

    public void SetDoor(bool isDoor){
        if(setObject != null)
            DestroyImmediate(setObject);

        this.setObject = Instantiate(isDoor ? doorObject : wallObject, transform.position, transform.rotation, transform.parent);
        this.gameObject.SetActive(false);
    }
}

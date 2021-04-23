using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private List<Passive> passiveList = new List<Passive>();

    public void Initialize(Passive addedPassive, GameObject player)
    {
        passiveList.Add(addedPassive);
        addedPassive.Initialize(player);   
    }

    void Update()
    {
        for (int i = 0; i < passiveList.Count; i++)
        {
            if (passiveList[i].CheckValidity()) //if a passive is valid, then it means it's effect can be triggered this frame
            {
                passiveList[i].TriggerPassive();
            }
        }
    }
}
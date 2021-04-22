using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private List<Passive> passiveList = new List<Passive>();

    void Start()
    {
        //create a way to call initialize on passive item's that get picked up, do the same in CooldownController
    }

    public void Initialize(Passive addedPassive, GameObject player)
    {
        passiveList.Add(addedPassive);
        addedPassive.Initialize(player);   
    }

    void Update()
    {
        for (int i = 0; i < passiveList.Count; i++)
        {
            if (passiveList[i].CheckValidity())
            {
                passiveList[i].TriggerPassive();
            }
        }
    }
}

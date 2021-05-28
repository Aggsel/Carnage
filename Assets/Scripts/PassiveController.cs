using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PassiveController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject passivePrefab = null;
    [SerializeField] private Transform passiveParent = null;
    [SerializeField] private List<GameObject> passivePrefabList = new List<GameObject>();
    [SerializeField] private List<Passive> passiveList = new List<Passive>();

    public void Initialize(Passive addedPassive, GameObject player)
    {
        //passive ui
        if (passiveList.Contains(addedPassive))
        {
            int currentAmount = int.Parse(passivePrefabList[passiveList.IndexOf(addedPassive)].GetComponentInChildren<TextMeshProUGUI>().text);
            currentAmount++;
            passivePrefabList[passiveList.IndexOf(addedPassive)].GetComponentInChildren<TextMeshProUGUI>().text = currentAmount.ToString();
        }
        else
        {
            GameObject newPassive = Instantiate(passivePrefab) as GameObject;
            newPassive.transform.SetParent(passiveParent, false);
            passivePrefabList.Add(newPassive);
            newPassive.GetComponentInChildren<TextMeshProUGUI>().text = "1";
            newPassive.GetComponent<Image>().sprite = addedPassive.sprite;
        }

        passiveList.Add(addedPassive);
        addedPassive.Initialize(player);
    }

    //maybe lateupdate?
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
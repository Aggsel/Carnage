using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemTrail : MonoBehaviour
{
    [SerializeField] private float speed = 0.0f;
    [SerializeField] private GameObject spawnParticle = null;

    private Vector3 startPos = Vector3.zero;
    private ParticleSystem par = null;
    private GameObject itemObj = null;
    private NavMeshAgent agent = null;

    public void SetItem (GameObject item)
    {
        itemObj = item;
    }

    private void Start ()
    {
        par = GetComponentInChildren<ParticleSystem>();
        agent = GetComponent<NavMeshAgent>();
        startPos = par.transform.localPosition;

        agent.speed = speed;
    }

    private void Update()
    {
        Bobbing();
        MoveToItem();
    }

    private void MoveToItem()
    {
        if(itemObj != null)
        {
            agent.SetDestination(itemObj.transform.position);

            float dist = Vector3.Distance(transform.position, itemObj.transform.position);

            if(dist < 0.5f)
            {
                GameObject spawn = Instantiate(spawnParticle) as GameObject;
                spawn.transform.SetPositionAndRotation(itemObj.transform.position, Quaternion.identity);
                itemObj.SetActive(true);

                Destroy(spawn, 1.0f);
                Destroy(gameObject);
            }
        }
    }

    private void Bobbing()
    {
        float step = Mathf.Cos(Time.time) * 0.20f;
        par.transform.localPosition = startPos + new Vector3(0, step, 0);
    }
}

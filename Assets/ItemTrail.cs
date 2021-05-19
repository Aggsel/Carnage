using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemTrail : MonoBehaviour
{
    [SerializeField] private float speed = 0.0f;
    [SerializeField] private GameObject spawnParticle = null;

    [SerializeField] private float dist = 0.0f;

    private Vector3 startPos = Vector3.zero;
    private ParticleSystem par = null;
    private GameObject itemObj = null;
    private NavMeshAgent agent = null;
    private NavMeshPath path = null;
    private float destroyTimer = 0.0f;

    public void SetStuff (GameObject item)
    {
        itemObj = item;

        if (par == null)
        {
            par = GetComponentInChildren<ParticleSystem>();
        }

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        path = new NavMeshPath();
        agent.CalculatePath(itemObj.transform.position, path);

        startPos = par.transform.localPosition;
        agent.speed = speed;
        destroyTimer = 0.0f;
    }

    private void Update()
    {
        destroyTimer += Time.deltaTime;
        Debug.Log(destroyTimer);

        if(destroyTimer > 20.0f)
        {
            SpawnItem();
        }

        Bobbing();
        MoveToItem();
    }

    private void MoveToItem()
    {
        if(itemObj != null && agent != null)
        {
            if(path.status != NavMeshPathStatus.PathInvalid || path.status != NavMeshPathStatus.PathPartial)
            {
                agent.SetDestination(itemObj.transform.position);
            }
            else
            {
                Debug.LogWarning("ItemTrail cant find a way to the item or is on a invalid navmesh!");
            }

            dist = Vector3.Distance(transform.position, itemObj.transform.position);

            if(dist < 1.0f)
            {
                SpawnItem();
            }
        }
        else
        {
            Debug.LogWarning("No item (destination) found for itemTrail to move towards!");
        }
    }

    private void SpawnItem()
    {
        GameObject spawn = Instantiate(spawnParticle) as GameObject;
        spawn.transform.SetPositionAndRotation(itemObj.transform.position, Quaternion.identity);
        itemObj.SetActive(true);

        Destroy(spawn, 1.0f);
        Destroy(gameObject);
    }

    private void Bobbing()
    {
        float step = Mathf.Cos(Time.time) * 0.20f;
        par.transform.localPosition = startPos + new Vector3(0, step, 0);
    }
}

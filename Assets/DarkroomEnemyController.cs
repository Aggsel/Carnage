using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkroomEnemyController : MonoBehaviour
{
    [SerializeField]
    private GameObject player = null;
    private ConstraintMeleeBehavior cmb = null;
    private bool enemiesLookAt = true;

    void Awake()
    {
        cmb = this.GetComponent<ConstraintMeleeBehavior>();
        cmb.enabled = false;
    }

    public void ActivateEnemy()
    {
        cmb.enabled = true;
    }

    public void EnemyLookAt(bool value)
    {
        enemiesLookAt = value;
    }

    void Update()
    {
        if (enemiesLookAt)
        {
            transform.LookAt(player.transform);
        }
    }
}

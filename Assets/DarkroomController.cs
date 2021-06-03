using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkroomController : MonoBehaviour
{

    [SerializeField]
    private DarkroomEnemyController[] darkRoomEnemyList = null;
    private UIController uic = null;
    private AudioManager am = null;

    void Start()
    {
        am = AudioManager.Instance;
        am.SetParameterByName(ref am.ambManager,"Battle", 0f);
        am.SetParameterByName(ref am.ambManager, "State", 0f);
        am.PlaySound(ref am.ambManager);
        uic = GameObject.Find("Game Controller Controller/Canvas").GetComponent<UIController>();
        StartCoroutine("DarkroomStart");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator DarkroomStart()
    {
        yield return new WaitForSeconds(10.0f);
        foreach (DarkroomEnemyController item in darkRoomEnemyList)
        {
            item.EnemyLookAt(false);
            item.ActivateEnemy();
            yield return new WaitForSeconds(Random.Range(0f, 0.30f));
        }
    }
}

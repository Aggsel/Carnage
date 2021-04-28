using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Carl
/// 2021-04-08
/// </summary>

public class Viewbob : MonoBehaviour
{
    [Header("Viewbobbing: ")]
    [Tooltip("The strength of Bob. No but just the amount each view'bob' moves")]
    [SerializeField] private float bobStrength = 0.5f;
    [Tooltip("how fast the viewbobbing moves, should be in sync with movement sounds")]
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 origin = Vector3.zero;
    private Vector3 dest = Vector3.zero;
    private float time = -1.0f;
    private bool down = false;

    private AudioManager am = null;
    private CharacterController cc;

    private void Start ()
    {
        am = AudioManager.Instance;
        cc = GetComponentInParent<CharacterController>();
        origin = transform.localPosition;
    }

    private void LateUpdate ()
    {
        //not jumping nor dashing
        if(cc.isGrounded && cc.enabled)
        {
            Bobbing();
        }
    }

    private void Bobbing ()
    {
        float vel = cc.velocity.magnitude; //Mathf.Sqrt(cc.velocity.x * cc.velocity.x + cc.velocity.z * cc.velocity.z);
        //Debug.Log(vel);

        time = Mathf.PingPong(Time.time * bobSpeed, 2.0f) -1.0f;
        dest = (vel * 2.0f) * new Vector3(0, -Mathf.Sin(time * time * (bobStrength * 0.001f)), Mathf.Sin(time * (bobStrength * 0.001f)));

        //change to sync with footsteps
        if(transform.localPosition.y <= -0.05f && !down)
        {
            down = true;
            //am.SetParameterByName(am.playerFootsteps, "surface", 0.5f);
            am.PlaySound(am.playerFootsteps);
        }
        else if (transform.localPosition.y > -0.05f)
        {
            down = false;
        }

        transform.localPosition = origin + dest;
    }
}

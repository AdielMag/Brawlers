using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFighting : MonoBehaviour
{
    int kicks;
    int heavyAttack;

    PlayerMovement pMov;
    Animator anim;
    Rigidbody rb;

    private void Start()
    {
        pMov = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        if (Input.GetButtonDown("LightAtt"))
        {
            kicks = Random.Range(1, 3);
        }
        else if (Input.GetButtonUp("LightAtt"))
        {
            kicks = 0;
        }

        if (Input.GetButtonDown("HeavyAtt")) 
        {
            heavyAttack = Random.Range(1, 4);
        }else if (Input.GetButtonUp("HeavyAtt"))
        {
            heavyAttack = 0;
        }

        if (rb.velocity.x == 0)
            kicks = heavyAttack = 0;

        anim.SetInteger("Kicks", kicks);
        anim.SetInteger("Heavy Attack", heavyAttack);

    }

    public void StartAttack(float time) 
    {
        StartCoroutine(pMov.ChangeMovementSpeed(time,1.7f));
        StartCoroutine(pMov.CantGrabWallTimer(time));
        StartCoroutine(pMov.CantChangeDircetion(time));
        StartCoroutine(pMov.DashTimer(time));

    }
}

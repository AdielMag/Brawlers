using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFighting : MonoBehaviour
{
    public float health, maxHealth =100;

    public float kickMaxDistance = 2.3f, attackMaxDistance;

    int kicks;
    int heavyAttack;

    PlayerMovement pMov;
    PlayerInput pInput;
    Animator anim;
    Rigidbody rb;

    public GameObject rightFootKickParticles, leftFootKickParticles;

    private void Start()
    {
        pMov = GetComponent<PlayerMovement>();
        pInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        health = maxHealth;
    }

    private void Update()
    {

        if (Input.GetButtonDown(pInput.ligthAttack))
        {
            kicks = Random.Range(1, 3);
        }
        else if (Input.GetButtonUp(pInput.ligthAttack))
        {
            kicks = 0;
        }

        if (Input.GetButtonDown(pInput.heavyAttack)) 
        {
            heavyAttack = Random.Range(1, 4);
        }else if (Input.GetButtonUp(pInput.heavyAttack))
        {
            heavyAttack = 0;
        }

        if (rb.velocity.x == 0)
            kicks = heavyAttack = 0;

        anim.SetInteger("Kicks", kicks);
        anim.SetInteger("Heavy Attack", heavyAttack);

    }

    public void TakeHit(float amount, Vector3 direction,float knockbackMultiplayer)
    {
        health -= amount;

        rb.velocity = direction * knockbackMultiplayer;
        //rb.drag = 6;
    }

    public void Kick(int direction)
    {
        StartCoroutine(KickParticleTimer(direction == 1 ? true : false));


        Vector3 kickDir = rb.velocity.x > 0 ? transform.right : -transform.right;

        Ray ray = new Ray(transform.position + Vector3.up / 2, kickDir);

        if (Physics.Raycast(ray, out RaycastHit hit, kickMaxDistance))
        {
            if (hit.transform.GetComponent<PlayerFighting>())
            {
                StartCoroutine(DamageAndKnockBackOffsetTimer(hit.transform.GetComponent<PlayerFighting>(), 0, -hit.normal, 50));
            }
        }
    }

    public void StartAttack(float time) 
    {
        StartCoroutine(pMov.ChangeMovementSpeed(time,1.7f));
        StartCoroutine(pMov.CantGrabWallTimer(time));
        StartCoroutine(pMov.CantChangeDircetion(time));
        StartCoroutine(pMov.DashTimer(time));
    }
     
    // Enumatrators
    public IEnumerator KickParticleTimer(bool direction)
    {
        if (direction)
            rightFootKickParticles.SetActive(true);
        else
            leftFootKickParticles.SetActive(true);

        yield return new WaitForSeconds(.5f);

        if (direction)
            rightFootKickParticles.SetActive(false);
        else
            leftFootKickParticles.SetActive(false);
    }

    public IEnumerator DamageAndKnockBackOffsetTimer(PlayerFighting pFight, float amount, Vector3 direction, float knockbackMultiplayer)
    {
        yield return new WaitForSeconds(.085f);
        pFight.TakeHit(amount, direction, knockbackMultiplayer);
    }

}

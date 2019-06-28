using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    public GameObject selectorUI;
    public Transform charecterSelectableParent;
    public bool playerEntered;

    bool canMoveUI = true;

    Transform selectedCharacter;
    int selectableNum;

    Animator anim;
    PlayerInput pInput;

    private void Start()
    { 
        selectorUI.SetActive(false);
        anim = GetComponent<Animator>();
        pInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (playerEntered)
        {
            selectorUI.transform.position = Vector3.Lerp(selectorUI.transform.position, selectedCharacter.position, Time.deltaTime * 8);
            HandleSelectorMovement();
        }

        // if any button is pressed
        else if (Input.GetButton(pInput.ligthAttack) || Input.GetButton(pInput.dash) || Input.GetButton(pInput.heavyAttack)
            || Input.GetButton(pInput.jump) || Input.GetButton(pInput.grabWall))
        {
            EnableSelector();
        }
    }

    void HandleSelectorMovement()
    {
        Vector2 inputDir = new Vector2(Input.GetAxisRaw(pInput.horizontal), Input.GetAxisRaw(pInput.vertical));

        if (!canMoveUI)
            return;

        if (inputDir.x > 0)
        {
            for (int i = selectableNum + 1; i < charecterSelectableParent.childCount; i++)
            {
                if (CheckIfSelectableIsFree(i))
                {
                    charecterSelectableParent.GetChild(selectableNum).GetComponent<CharacterSelectable>().selected = false;
                    charecterSelectableParent.GetChild(i).GetComponent<CharacterSelectable>().selected = true;
                    selectedCharacter = charecterSelectableParent.GetChild(i);
                    selectableNum = i;

                    canMoveUI = false;
                    StartCoroutine(WaitUntilNextMovement());

                    break;
                }
            }
        }

        else if (inputDir.x < 0)
        {
            for (int i = selectableNum - 1; i > -1; i--)
            {
                if (CheckIfSelectableIsFree(i))
                {
                    charecterSelectableParent.GetChild(selectableNum).GetComponent<CharacterSelectable>().selected = false;
                    charecterSelectableParent.GetChild(i).GetComponent<CharacterSelectable>().selected = true;
                    selectedCharacter = charecterSelectableParent.GetChild(i);
                    selectableNum = i;

                    canMoveUI = false;
                    StartCoroutine(WaitUntilNextMovement());

                    break;
                }
            }
        }

        else if (inputDir.y > 0)
        {
            int targetNum = selectableNum - 6;
            if (CheckIfSelectableIsFree(targetNum))
            {
                charecterSelectableParent.GetChild(selectableNum).GetComponent<CharacterSelectable>().selected = false;
                charecterSelectableParent.GetChild(targetNum).GetComponent<CharacterSelectable>().selected = true;
                selectedCharacter = charecterSelectableParent.GetChild(targetNum);
                selectableNum = targetNum;

                canMoveUI = false;
                StartCoroutine(WaitUntilNextMovement());
            }
        }
        else if (inputDir.y < 0)
        {
            int targetNum = selectableNum + 6;
            if (CheckIfSelectableIsFree(targetNum))
            {
                charecterSelectableParent.GetChild(selectableNum).GetComponent<CharacterSelectable>().selected = false;
                charecterSelectableParent.GetChild(targetNum).GetComponent<CharacterSelectable>().selected = true;
                selectedCharacter = charecterSelectableParent.GetChild(targetNum);
                selectableNum = targetNum;

                canMoveUI = false;
                StartCoroutine(WaitUntilNextMovement());
            }
        }
    }

    void EnableSelector()
    {
        anim.SetBool("On", true);
        for(int i =0;i < charecterSelectableParent.childCount; i++)
        {
            if(CheckIfSelectableIsFree(i))
            {
                charecterSelectableParent.GetChild(i).GetComponent<CharacterSelectable>().selected = true;
                selectedCharacter = charecterSelectableParent.GetChild(i);
                selectableNum = i;
                break;
            }
        }

        selectorUI.transform.position = selectedCharacter.position;
        selectorUI.SetActive(true);
        playerEntered = true;
    }

    bool CheckIfSelectableIsFree(int selectableNum)
    {
        if (selectableNum < 0 || selectableNum > charecterSelectableParent.childCount -1)
            return false;

        return !charecterSelectableParent.GetChild(selectableNum).GetComponent<CharacterSelectable>().selected;
    }

    IEnumerator WaitUntilNextMovement()
    {
        yield return new WaitForSeconds(.2f);
        canMoveUI = true;
    }
}

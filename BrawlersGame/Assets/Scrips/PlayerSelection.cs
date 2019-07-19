using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    public GameObject selectorUI;
    public Transform charecterSelectableParent;
    public bool playerEntered;

    [Header("Editor Variables")]
    public Image image;
    public Text name;

    bool canMoveUI = true;

    CharacterSelectable gameSelectedCharacter;
    Transform currentSelectedCharacter;
    int currentSelectableNum;

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
            selectorUI.transform.position = Vector3.Lerp(selectorUI.transform.position, currentSelectedCharacter.position, Time.deltaTime * 8);
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

        if (Input.GetButtonDown(pInput.ligthAttack))
        {
            DeSelectCharacter();
        }

        if (!canMoveUI)
            return;

        if (inputDir.x > 0)
        {
            for (int i = currentSelectableNum + 1; i < charecterSelectableParent.childCount; i++)
            {
                if (CheckIfSelectableIsFree(i))
                {
                    if (charecterSelectableParent.GetChild(currentSelectableNum).GetComponent<CharacterSelectable>() != gameSelectedCharacter)
                        charecterSelectableParent.GetChild(currentSelectableNum).GetComponent<CharacterSelectable>().selected = false;

                    charecterSelectableParent.GetChild(i).GetComponent<CharacterSelectable>().selected = true;
                    currentSelectedCharacter = charecterSelectableParent.GetChild(i);
                    currentSelectableNum = i;

                    anim.SetTrigger("ChangeCharacter");

                    canMoveUI = false;
                    StartCoroutine(WaitUntilNextMovement());

                    break;
                }
            }
        }
        else if (inputDir.x < 0)
        {
            for (int i = currentSelectableNum - 1; i > -1; i--)
            {
                if (CheckIfSelectableIsFree(i))
                {
                    charecterSelectableParent.GetChild(currentSelectableNum).GetComponent<CharacterSelectable>().selected = false;
                    charecterSelectableParent.GetChild(i).GetComponent<CharacterSelectable>().selected = true;
                    currentSelectedCharacter = charecterSelectableParent.GetChild(i);
                    currentSelectableNum = i;

                    anim.SetTrigger("ChangeCharacter"); canMoveUI = false;
                    StartCoroutine(WaitUntilNextMovement());

                    break;
                }
            }
        }
        else if (inputDir.y > 0)
        {
            int targetNum = currentSelectableNum - 6;
            if (CheckIfSelectableIsFree(targetNum))
            {
                charecterSelectableParent.GetChild(currentSelectableNum).GetComponent<CharacterSelectable>().selected = false;
                charecterSelectableParent.GetChild(targetNum).GetComponent<CharacterSelectable>().selected = true;
                currentSelectedCharacter = charecterSelectableParent.GetChild(targetNum);
                currentSelectableNum = targetNum;

                anim.SetTrigger("ChangeCharacter"); canMoveUI = false;
                StartCoroutine(WaitUntilNextMovement());
            }
        }
        else if (inputDir.y < 0)
        {
            int targetNum = currentSelectableNum + 6;
            if (CheckIfSelectableIsFree(targetNum))
            {
                charecterSelectableParent.GetChild(currentSelectableNum).GetComponent<CharacterSelectable>().selected = false;
                charecterSelectableParent.GetChild(targetNum).GetComponent<CharacterSelectable>().selected = true;
                currentSelectedCharacter = charecterSelectableParent.GetChild(targetNum);
                currentSelectableNum = targetNum;

                anim.SetTrigger("ChangeCharacter"); canMoveUI = false;
                StartCoroutine(WaitUntilNextMovement());
            }
        }

        if (Input.GetButtonDown(pInput.jump))
        {
            SelectCharacter();
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
                currentSelectedCharacter = charecterSelectableParent.GetChild(i);
                currentSelectableNum = i;
                break;
            }
        }

        selectorUI.transform.position = currentSelectedCharacter.position;
        ChangeCharacter();
        selectorUI.SetActive(true);
        playerEntered = true;
    }

    void SelectCharacter()
    {
        gameSelectedCharacter = currentSelectedCharacter.GetComponent<CharacterSelectable>();
        canMoveUI = false;

        anim.SetBool("Selected",true); canMoveUI = false;
    }

    void DeSelectCharacter()
    {
        gameSelectedCharacter = null;
        canMoveUI = true;

        anim.SetBool("Selected", false);
    }

    bool CheckIfSelectableIsFree(int selectableNum)
    {
        if (selectableNum < 0 || selectableNum > charecterSelectableParent.childCount -1)
            return false;

        return !charecterSelectableParent.GetChild(selectableNum).GetComponent<CharacterSelectable>().selected;
    }

    public void ChangeCharacter()
    {
        CharacterSelectable charSelected = charecterSelectableParent.GetChild(currentSelectableNum).GetComponent<CharacterSelectable>();

        image.sprite = charSelected.icon.sprite;
        name.text = charSelected.name;
    }

    IEnumerator WaitUntilNextMovement()
    {
        yield return new WaitForSeconds(.2f);
        canMoveUI = true;
    }
}

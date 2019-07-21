using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class PlayerSelection : MonoBehaviour
{
    // Rewired Stuff
    public int playerId;
    private Player player; // The Rewired Player
    private CharacterController cc;


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

    private void Start()
    { 
        selectorUI.SetActive(false);
        anim = GetComponent<Animator>();

        player = ReInput.players.GetPlayer(playerId);
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (playerEntered)
        {
            selectorUI.transform.position = Vector3.Lerp(selectorUI.transform.position, currentSelectedCharacter.position, Time.deltaTime * 8);
            HandleSelectorMovement();


            if (player.GetButtonDown("Jump") && canMoveUI)
            {
                SelectCharacter();
            }
            else if (player.GetButtonDown("Light Attack"))
            {
                DeSelectCharacter();
            }
        }

        // if any button is pressed
        else if (player.GetButton("Light Attack") || player.GetButton("Dash") || player.GetButton("Heavy Attack")
            || player.GetButton("Jump") || player.GetButton("Grab Wall"))
        {
            EnableSelector();
        }
    }

    void HandleSelectorMovement()
    {
        Vector2 inputDir = new Vector2(player.GetAxis("Move Horizontal"), player.GetAxis("Move Vertical"));

        

        if (!canMoveUI)
            return;

        if (inputDir.x > .2f)
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
        else if (inputDir.x < -.2f)
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
        else if (inputDir.y > .2f)
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
        else if (inputDir.y < -.2f)
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

        StartCoroutine(WaitUntilNextMovement());

    }

    void SelectCharacter()
    {
        gameSelectedCharacter = currentSelectedCharacter.GetComponent<CharacterSelectable>();

        GameManager gMan = GameManager.instance;

        switch (playerId)
        {
            case 0:
                gMan.p1Character = gameSelectedCharacter.name;
                break;
            case 1:
                gMan.p2Character = gameSelectedCharacter.name;
                break;
            case 2:
                gMan.p3Character = gameSelectedCharacter.name;
                break;
            case 3:
                gMan.p4Character = gameSelectedCharacter.name;
                break;
        }

        gMan.CheckIfCanStartGame();

        canMoveUI = false;

        anim.SetBool("Selected",true); canMoveUI = false;
    }

    void DeSelectCharacter()
    {
        gameSelectedCharacter = null;

        GameManager gMan = GameManager.instance;

        switch (playerId)
        {
            case 0:
                gMan.p1Character = null;
                break;
            case 1:
                gMan.p2Character = null;
                break;
            case 2:
                gMan.p3Character = null;
                break;
            case 3:
                gMan.p4Character = null;
                break;
        }

        gMan.CheckIfCanStartGame();

        canMoveUI = true;

        anim.SetBool("Selected", false);
    }

    bool CheckIfSelectableIsFree(int selectableNum)
    {
        if (selectableNum < 0 || selectableNum > charecterSelectableParent.childCount -1)
            return false;

        return !charecterSelectableParent.GetChild(selectableNum).GetComponent<CharacterSelectable>().selected;
    }

    public void ChangeCharacter() // Used by an animation event
    {
        CharacterSelectable charSelected = charecterSelectableParent.GetChild(currentSelectableNum).GetComponent<CharacterSelectable>();

        GameManager gMan = GameManager.instance;

        image.sprite = charSelected.icon.sprite;
        name.text = charSelected.name;
    }

    IEnumerator WaitUntilNextMovement()
    {
        yield return new WaitForSeconds(.2f);
        canMoveUI = true;
    }
}

using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SelectElement : MonoBehaviour
{
    public Transform[] objectsToSelect;
    public int currentObjectNum;

    Transform targetObject;

    bool canMoveUI = true;
    void HandleSelectorMovement()
    {
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("P1Horizontal"), Input.GetAxisRaw("P1Vertical"));

        if (!canMoveUI)
            return;

        else if (inputDir.y < 0)
        {
            if (!canMoveUI)
                return;

            if (currentObjectNum + 1 >= objectsToSelect.Length)
                return;

            currentObjectNum++;
            targetObject = objectsToSelect[currentObjectNum];
            StartCoroutine(WaitUntilNextMovement());

        }
        else if (inputDir.y > 0)
        {
            if (!canMoveUI)
                return;

            if (currentObjectNum - 1 < 0)
                return;

            currentObjectNum--;
            targetObject = objectsToSelect[currentObjectNum];
            StartCoroutine(WaitUntilNextMovement());

        }

    }

    void DoAction()
    {
        if (!canMoveUI)
            return;

        SelectAction actionToDo = targetObject.GetComponent<SelectAction>();

        switch (actionToDo.action)
        {
            case SelectAction.Action.LoadScene:
                GameManager.instance.LoadScene(actionToDo.sceneName);
                break;
            case SelectAction.Action.PickCharacter:
                break;
            case SelectAction.Action.Exit:
                GameManager.instance.ExitGame();
                break;
        }

        StartCoroutine(WaitUntilNextMovement());
    }

    IEnumerator WaitUntilNextMovement()
    {
        canMoveUI = false;
        yield return new WaitForSeconds(.2f);
        canMoveUI = true;
    }

    private void Start()
    {
        targetObject = objectsToSelect[0];
    }

    private void Update()
    {
        HandleSelectorMovement();

        if (Input.GetButton("P1Jump"))
        {
            DoAction();
        }

        transform.position = Vector3.Lerp(transform.position, targetObject.position, Time.deltaTime * 6);
        (transform.GetChild(0) as RectTransform).sizeDelta = Vector2.Lerp((transform.GetChild(0) as RectTransform).sizeDelta, (targetObject as RectTransform).sizeDelta, Time.deltaTime * 6);
    }
}

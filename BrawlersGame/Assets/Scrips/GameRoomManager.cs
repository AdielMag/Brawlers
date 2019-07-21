using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomManager : MonoBehaviour
{
    public Slider[] playersHpBars;

    GameManager gMan;
    CameraController cCon;

    public static GameRoomManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gMan = GameManager.instance;
        cCon = CameraController.instance;

        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        GameObject playerObj;

        int playersSum = 2;
        if (!string.IsNullOrEmpty(gMan.p3Character))
            playersSum++;
        if (!string.IsNullOrEmpty(gMan.p4Character))
            playersSum++;

        cCon.playersTransforms = new Transform[playersSum];

        // First player
        playerObj = Instantiate(Resources.Load("Prefabs/Characters/" + gMan.p1Character)) as GameObject;
        cCon.playersTransforms[0] = playerObj.transform;
        playerObj.GetComponent<PlayerFighting>().hpBar = playersHpBars[0];

        // Second Player
        playerObj = Instantiate(Resources.Load("Prefabs/Characters/" + gMan.p2Character)) as GameObject;
        cCon.playersTransforms[1] = playerObj.transform;
        playerObj.GetComponent<PlayerMovement>().playerId = 1;
        playerObj.GetComponent<PlayerFighting>().hpBar = playersHpBars[1];
        playersHpBars[1].gameObject.SetActive(true);

        //Third Player
        if (!string.IsNullOrEmpty(gMan.p3Character))
        {
            playerObj = Instantiate(Resources.Load("Prefabs/Characters/" + gMan.p3Character)) as GameObject;
            cCon.playersTransforms[2] = playerObj.transform;
            playerObj.GetComponent<PlayerMovement>().playerId = 2;
            playerObj.GetComponent<PlayerFighting>().hpBar = playersHpBars[2];
            playersHpBars[2].gameObject.SetActive(true);
        }

        //Fourth Player
        if (!string.IsNullOrEmpty(gMan.p4Character))
        {
            playerObj = Instantiate(Resources.Load("Prefabs/Characters/" + gMan.p4Character)) as GameObject;
            cCon.playersTransforms[3] = playerObj.transform;
            playerObj.GetComponent<PlayerMovement>().playerId = 3;
            playerObj.GetComponent<PlayerFighting>().hpBar = playersHpBars[3];
            playersHpBars[3].gameObject.SetActive(true);
        }
    }
}

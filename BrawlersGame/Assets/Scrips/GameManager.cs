using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class GameManager : MonoBehaviour
{
    // Rewired Stuff
    private Player player; // The Rewired Player
    private CharacterController cc;

    public string p1Character, p2Character, p3Character, p4Character;

    bool canStartGame;
    public GameObject startGameIndicator;

    public static GameManager instance;
    private void Awake()
    {
        if (instance != this && instance != null)
            Destroy(gameObject);

        instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        player = ReInput.players.GetPlayer(0);
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (player.GetButton("Start") && canStartGame)
        {
            startGameIndicator.SetActive(false);
            LoadScene("GameRoom");
        }
    }

    public void CheckIfCanStartGame()
    {
        int playersSelectedSum = new int();

        if (!string.IsNullOrEmpty(p1Character))
            playersSelectedSum++;
        if (!string.IsNullOrEmpty(p2Character))
            playersSelectedSum++;
        if (!string.IsNullOrEmpty(p3Character))
            playersSelectedSum++;
        if (!string.IsNullOrEmpty(p4Character))
            playersSelectedSum++;

        if (playersSelectedSum > 1)
        {
            canStartGame = true;
            startGameIndicator.SetActive(true);
        }
        else 
        {
            canStartGame = false;
            startGameIndicator.SetActive(false);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneName, float timeToWait)
    {
        StartCoroutine(WaitThanLoadScene(sceneName, timeToWait));
    }

    IEnumerator WaitThanLoadScene(string sceneName, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

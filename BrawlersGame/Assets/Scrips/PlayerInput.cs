using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public int playerNum;

    public string horizontal;
    public string vertical;

    public string jump;
    public string grabWall;
    public string dash;
    public string ligthAttack;
    public string heavyAttack;

    private void Awake()
    {
        AssignInput();
    }

    public void AssignInput()
    {
        horizontal  = "P" + playerNum + "Horizontal";
        vertical    = "P" + playerNum + "Vertical";

        jump            = "P" + playerNum + "Jump";
        grabWall        = "P" + playerNum + "GrabWall";
        dash            = "P" + playerNum + "Dash";
        ligthAttack     = "P" + playerNum + "LightAttack";
        heavyAttack     = "P" + playerNum + "HeavyAttack";
    }
}

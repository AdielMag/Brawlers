using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAction : MonoBehaviour
{
    public enum Action {LoadScene,PickCharacter,Exit}
    public Action action;

    public string sceneName;
}

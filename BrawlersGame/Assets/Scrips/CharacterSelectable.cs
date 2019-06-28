using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectable : MonoBehaviour
{
    public float name;
    public Image image;
    public bool selected;

    [Header("UI Variables")]
    public Image icon;

    private void Awake()
    {
        if(image)
            icon = image;
    }
}

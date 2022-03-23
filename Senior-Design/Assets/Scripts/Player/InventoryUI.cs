using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI singleton;
    public GameObject[] spriteObjects;
    private int selectedIndex = -1;

    private void Awake()
    {
        singleton = this;
    }

    public void SetDisplayItem(ItemType item)
    {
        var index = (int)item;
        if (index >= 0 && index < spriteObjects.Length)
        {
            if (selectedIndex >= 0)
            {
                spriteObjects[selectedIndex].SetActive(false);
                spriteObjects[index].SetActive(true);
                selectedIndex = index;
            }
            else
            {
                spriteObjects[index].SetActive(true);
                selectedIndex = index;
            }
        }
        else
        {
            spriteObjects[selectedIndex].SetActive(false);
            selectedIndex = index;
        }

    }
}

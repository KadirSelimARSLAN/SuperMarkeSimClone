using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfParent : MonoBehaviour
{

    public bool isStorage;
    public int shelfParentIndx;
    public GameObject ShelfChild;

    public GameObject BoxChild;

    public bool boxMode=true;

    private void Start()
    {
        setActiveChild(boxMode);
    }
    public void setActiveChild(bool box_mode)
    {
        if (box_mode)
        {
            ShelfChild.SetActive(false);
            BoxChild.SetActive(true);
        }
        else
        {
            
            BoxChild.SetActive(false);
            ShelfChild.SetActive(true);
        }


    }



}

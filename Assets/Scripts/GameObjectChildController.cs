using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectChildController : MonoBehaviour
{
    public int myCurretVisibleItemIndx;
    public Transform bagPoint;
    public void VisibleItem(int indx)
    {
        myCurretVisibleItemIndx = indx;

        transform.GetChild(myCurretVisibleItemIndx).transform.gameObject.SetActive(true);
    }
    public void InVisibleItem()
    {
    
        transform.GetChild(myCurretVisibleItemIndx).transform.gameObject.SetActive(false);
    }


}

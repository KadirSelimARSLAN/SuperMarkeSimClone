using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateManager : Singleton<InstantiateManager>
{
    public GameObject[] itemBoxes;
    public GameObject[] shoesItemBox;
    public GameObject[] furnitureItemBox;
    public GameObject[] items_all;


    public Transform itemBoxParent;
    public Transform furnitureParent;
    public Transform storageParent;

    public void CloneItemBox(OrderData order)
    {
        if (order.product_Type == "Shoes")
        {

            GameObject itemBox = Instantiate(shoesItemBox[order.product_Indx], transform.position, transform.rotation);
            itemBox.transform.parent = itemBoxParent;
        }
        else if(order.product_Type == "Furniture")
        {
            GameObject furniture = Instantiate(furnitureItemBox[order.product_Indx], transform.position, transform.rotation);
            //DEBUG
            if(order.product_Indx ==0)
            furniture.transform.parent = furnitureParent;
            if (order.product_Indx == 1)
                furniture.transform.parent = storageParent;
        }
      
    }
    public void CloneItemBox(int itemBoxIndx)
    {
      

          GameObject itemBox =  Instantiate(itemBoxes[itemBoxIndx], transform.position, transform.rotation);
     
       

    }
    public void CloneShelfParent(int CloneShelfParent)
    {


        Instantiate(furnitureItemBox[CloneShelfParent], transform.position, transform.rotation);


    }
}

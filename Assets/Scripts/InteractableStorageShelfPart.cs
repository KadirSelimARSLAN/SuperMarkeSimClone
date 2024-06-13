using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableStorageShelfPart : IInteractable
{
    public List<GameObject> myShelfPoints = new List<GameObject>();
    
    public int shelfFreeItemCount = 0;
    public int maxShelfFreeItemCount;
    
    public string myItemName;
    public int myItemGeneralIndx;

    public string myItemType;
    public bool haveItem = false;
    public PriceTagData myPriceTag;


    private void Start()
    {
        gameObject.transform.parent.GetComponent<InteractableShelf>()?.UpdateEpoChild(GetComponent<Renderer>());


    }
    public override void OnFocus()
    {
        if (myShelfPoints.Count > 0)
        {
            if (shelfFreeItemCount != maxShelfFreeItemCount)
            {
                for (int i = 0; i < myShelfPoints.Count; i++)
                {

                    if (myShelfPoints[i].transform.childCount > 0)
                        myShelfPoints[i].transform.GetChild(0).GetComponent<InteractableItemBox>().getInteracted(true);
                }

            }
        }
      
   
    }

    public override void OnInteract(Transform playerHandPos)
    {
        if (FirstPersonController.Instance.currentHandTool != null)
        {

            if (FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>() != null)
             {

                if (!haveItem)
                {

                    GetShelfPartItemPoints(FirstPersonController.Instance.currentHandTool);

                }
                if (myItemName == FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>().ItemName && shelfFreeItemCount > 0 )
                {
                    
                    if (FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>().isReadyForInteract)
                    {
                        AddItem(FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>());
                    }
                  
                    
                }

            }
        }


    }
    public void OnReInteract(Transform playerHandPos)
    {
        
      if (FirstPersonController.Instance.currentHandTool ==null && myShelfPoints[shelfFreeItemCount].transform.childCount > 0)
           {

            if (myShelfPoints[shelfFreeItemCount].transform.GetChild(0).GetComponent<InteractableItemBox>().isReadyForInteract )
            {
                SendItemBack(myShelfPoints[shelfFreeItemCount].transform.GetChild(0)?.GetComponent<InteractableItemBox>());
            }
            
          //  FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>().ItemCount++;
           }


    }
    public override void OnLoseFocus()
    {
        if (myShelfPoints.Count > 0)
        {
            if (shelfFreeItemCount != maxShelfFreeItemCount)
            {
                for (int i = 0; i < myShelfPoints.Count; i++)
                {

                    if (myShelfPoints[i].transform.childCount > 0)
                        myShelfPoints[i].transform.GetChild(0).GetComponent<InteractableItemBox>().getInteracted(false);
                }

            }
        }
    }

    public override bool readyForInteract()
    {
        throw new System.NotImplementedException();
    }


    public void AddItem(InteractableItemBox box)
    {
       
        box.MoveStorageShelfPart(myShelfPoints[--shelfFreeItemCount].transform);
        myPriceTag.item_Count = maxShelfFreeItemCount - shelfFreeItemCount;
        myPriceTag.SetStorageTagValues(myItemType, myItemName);
        if (maxShelfFreeItemCount -1  == shelfFreeItemCount)
        {
            gameObject.layer = 11;
        }
        if (myShelfPoints.Count > 0)
        {
            if (shelfFreeItemCount != maxShelfFreeItemCount)
            {
                for (int i = 0; i < myShelfPoints.Count; i++)
                {

                    if (myShelfPoints[i].transform.childCount > 0)
                        myShelfPoints[i].transform.GetChild(0).GetComponent<InteractableItemBox>().getInteracted(true);
                }

            }
        }

    }

    public void SendItemBack(InteractableItemBox box)
    {
     
        shelfFreeItemCount++;
        box.MovePlayerHand(FirstPersonController.Instance.playerHandPos);
        FirstPersonController.Instance.currentHandTool = box.gameObject;
      
        if (shelfFreeItemCount >= maxShelfFreeItemCount)
        {
            gameObject.layer = 9;
            shelfFreeItemCount = 0;
            myItemName = "";
            myItemType = "";
            haveItem = false;

            myShelfPoints.Clear();
            myPriceTag.MakeVisiblePriceTag(false);
            maxShelfFreeItemCount = shelfFreeItemCount;
        }
    }

    private void GetShelfPartItemPoints(GameObject box)
    {
        Transform shelfTransform = null;
        if(box.gameObject.transform.GetComponent<InteractableItemBox>() != null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (box.gameObject.GetComponent<InteractableItemBox>().ItemName == transform.GetChild(i).transform.gameObject.name)
                {
                    shelfTransform = transform.GetChild(i);

                }

            }

        }
       

        foreach (Transform child in shelfTransform)
        {
            myShelfPoints.Add(child.gameObject);
            shelfFreeItemCount++;
            myItemName = box.gameObject.GetComponent<InteractableItemBox>().ItemName;
            myItemGeneralIndx = box.gameObject.GetComponent<InteractableItemBox>().generalItemIndex;
            myItemType = box.gameObject.GetComponent<InteractableItemBox>().itemType;
            haveItem = true;
          
        }
        myPriceTag.MakeVisiblePriceTag(true);
      
        myPriceTag.item_Count = maxShelfFreeItemCount - shelfFreeItemCount;
        myPriceTag.SetStorageTagValues(myItemType, myItemName);
        maxShelfFreeItemCount = shelfFreeItemCount;



    }

    public void LoadStorageShelfPartsItems(List<ItemBoxData> myItemBoxes)
    {
        Transform shelfTransform = null;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (myItemName == transform.GetChild(i).transform.gameObject.name)
            {
                shelfTransform = transform.GetChild(i);

            }

        }
        if (shelfTransform != null)
        {
            foreach (Transform child in shelfTransform)
            {
                myShelfPoints.Add(child.gameObject);
                myPriceTag.MakeVisiblePriceTag(true);
                myPriceTag.SetPriceTagValues(myItemGeneralIndx);
            }
        }
        for (int i = 0 ; i <myItemBoxes.Count; i++)
        {
            // Orijinal objenin scale değerlerini saklayın
            Vector3 originalScale = InstantiateManager.Instance.itemBoxes[myItemGeneralIndx].transform.localScale;

            // Clone'u oluşturun
            GameObject itemBoxClone = Instantiate(InstantiateManager.Instance.itemBoxes[myItemGeneralIndx], myShelfPoints[shelfFreeItemCount + i].transform.position, myShelfPoints[shelfFreeItemCount + i].transform.rotation);

            itemBoxClone.GetComponent<Rigidbody>().isKinematic = true;
            itemBoxClone.GetComponent<Collider>().isTrigger = true;
            itemBoxClone.GetComponent<Collider>().enabled = false;

            // Clone'un scale değerini orijinal scale değerleriyle ayarlayın
            itemBoxClone.transform.localScale = originalScale;

            // Clone'u parent objeye bağlayın
            itemBoxClone.transform.parent = myShelfPoints[shelfFreeItemCount + i].transform;


            if (itemBoxClone != null)  // Check if the child is actually an InteractableItemBox
            {


                itemBoxClone.GetComponent<InteractableItemBox>().generalItemIndex = myItemBoxes[i].generalitem_indx;
                itemBoxClone.transform.position = myItemBoxes[i].itemBox_Position;
                itemBoxClone.transform.rotation = myItemBoxes[i].itemBox_Rotation;
                itemBoxClone.GetComponent<InteractableItemBox>().itemBoxIndx = myItemBoxes[i].itemBox_indx;
                itemBoxClone.GetComponent<InteractableItemBox>().ItemCount = myItemBoxes[i].item_Count;
                itemBoxClone.GetComponent<InteractableItemBox>().isOpened = myItemBoxes[i].isOpen;

            }
        }
        /*for (int i = myItemBoxes.Count -1; i >=0 ; i--)
        {
            // Orijinal objenin scale değerlerini saklayın
            Vector3 originalScale = InstantiateManager.Instance.itemBoxes[myItemGeneralIndx].transform.localScale;

            // Clone'u oluşturun
            GameObject itemBoxClone = Instantiate(InstantiateManager.Instance.itemBoxes[myItemGeneralIndx], myShelfPoints[i].transform.position, myShelfPoints[i].transform.rotation);

            itemBoxClone.GetComponent<Rigidbody>().isKinematic = true;
            itemBoxClone.GetComponent<Collider>().isTrigger = true;
            itemBoxClone.GetComponent<Collider>().enabled = false;

            // Clone'un scale değerini orijinal scale değerleriyle ayarlayın
            itemBoxClone.transform.localScale = originalScale;

            // Clone'u parent objeye bağlayın
            itemBoxClone.transform.parent = myShelfPoints[i].transform;


          if (itemBoxClone != null)  // Check if the child is actually an InteractableItemBox
          {


              itemBoxClone.GetComponent<InteractableItemBox>().generalItemIndex = myItemBoxes[i].generalitem_indx;
              itemBoxClone.transform.position = myItemBoxes[i].itemBox_Position;
              itemBoxClone.transform.rotation = myItemBoxes[i].itemBox_Rotation;
              itemBoxClone.GetComponent<InteractableItemBox>().itemBoxIndx = myItemBoxes[i].itemBox_indx;
              itemBoxClone.GetComponent<InteractableItemBox>().ItemCount = myItemBoxes[i].item_Count;
              itemBoxClone.GetComponent<InteractableItemBox>().isOpened = myItemBoxes[i].isOpen;

          }
      }*/


        /*  for (int i = maxShelfFreeItemCount; i > shelfFreeItemCount; i--)
          {
              // Orijinal objenin scale değerlerini saklayın
              Vector3 originalScale = InstantiateManager.Instance.itemBoxes[myItemGeneralIndx].transform.localScale;

              // Clone'u oluşturun
              GameObject itemBoxClone = Instantiate(InstantiateManager.Instance.itemBoxes[myItemGeneralIndx], myShelfPoints[i - 1].transform.position, myShelfPoints[i - 1].transform.rotation);
              itemBoxClone.GetComponent<Rigidbody>().isKinematic = true;
              itemBoxClone.GetComponent<Collider>().isTrigger = true;
              itemBoxClone.GetComponent<Collider>().enabled = false;

              // Clone'un scale değerini orijinal scale değerleriyle ayarlayın
              itemBoxClone.transform.localScale = originalScale;

              // Clone'u parent objeye bağlayın
              itemBoxClone.transform.parent = myShelfPoints[i - 1].transform;
          }*/
        if (shelfFreeItemCount !=maxShelfFreeItemCount)
        {
            gameObject.layer = 11;
        }
        if (myShelfPoints.Count > 0)
        {
            if (shelfFreeItemCount != maxShelfFreeItemCount)
            {
                for (int i = 0; i < myShelfPoints.Count; i++)
                {

                    if (myShelfPoints[i].transform.childCount > 0)
                        myShelfPoints[i].transform.GetChild(0).GetComponent<InteractableItemBox>().getInteracted(true);
                }

            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class InteractableShelfPart : IInteractable
{
    
    public List<GameObject> myShelfPoints = new List<GameObject>();
    
    public int shelfFreeItemCount = 0;
    public int maxShelfFreeItemCount;
    public string myItemType;
    public string myItemName;
    public int myItemGeneralIndx;
    public bool haveItem=false;
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

                    if (myShelfPoints[i].transform.childCount>0)
                        myShelfPoints[i].transform.GetChild(0).GetComponent<InteractableItem>().getInteracted(true);
                }

            }
        }
     
    }
   
    public override void OnInteract(Transform playerHandPos)
    {
        
            if (FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>().ItemCount > 0)
            {

                if (FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>() != null)
                {

                    if (FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>().isOpened == true)
                    {
                    if (!haveItem)
                    {
                        GetShelfPartItemPoints(FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>().myItems[FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>().ItemCount - 1]);

                    }
                    if (myItemName== FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>().ItemName && shelfFreeItemCount > 0)
                    {
                        AddItem(FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>().myItems[FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>().ItemCount - 1]);
                        FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>().moveItemtoShelfPart();               
                    }
                }
            }
        }
       
      
    }
    public  void OnReInteract(Transform playerHandPos)
    {
       
            if (maxShelfFreeItemCount > shelfFreeItemCount)
            {
                if (FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>().ItemCount < FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>().MaxItemCount)
                {


                    if (FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>() != null)
                    {

                      
                        if (FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>().isOpened == true)
                        {
                            if (FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>().ItemName == myItemName && myShelfPoints[shelfFreeItemCount].transform.childCount >0 )
                            {

                            FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>().AddItems(myShelfPoints[shelfFreeItemCount].transform.GetChild(0)?.GetComponent<InteractableItem>());

                           SendItemBack(myShelfPoints[shelfFreeItemCount].transform.GetChild(0)?.GetComponent<InteractableItem>(), FirstPersonController.Instance.currentHandTool?.GetComponent<InteractableItemBox>().transform.GetChild(FirstPersonController.Instance.currentHandTool.GetComponent<InteractableItemBox>().ItemCount-1));


                          
                            }


                        }


                    }
                }
            
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
                    
                    if (myShelfPoints[i].transform.childCount>0)
                        myShelfPoints[i].transform.GetChild(0).GetComponent<InteractableItem>().getInteracted(false);
                 
                }

            }

        }
       
    }

    public override bool readyForInteract()
    {
        throw new System.NotImplementedException();
    }


    public void AddItem(InteractableItem Item)
    {
        
            Item.MoveToShelfPartPoint(myShelfPoints[--shelfFreeItemCount].transform);
     


    }

    public void SendItemBack(InteractableItem Item , Transform boxParent)
    {
        
        shelfFreeItemCount++;
        Item.MoveToShelfPartPoint(boxParent);
        if (shelfFreeItemCount >= maxShelfFreeItemCount)
        {
            shelfFreeItemCount = 0;
            myItemName = "";
            haveItem = false;
            myItemType = "";
            myShelfPoints.Clear();
            myPriceTag.MakeVisiblePriceTag(false);
            maxShelfFreeItemCount = shelfFreeItemCount;
            FurnituresParent.Instance.itemsShelfParts.Remove(this.gameObject.GetComponent<InteractableShelfPart>());
       
        }

      
       
    }
    public void SendItemBacktoCustomer(int itemIndexInShelfPoint, Transform customerParent)
    {


        InteractableItem ıtem= myShelfPoints[itemIndexInShelfPoint].transform.GetChild(0)?.GetComponent<InteractableItem>();
         shelfFreeItemCount++;
        ıtem.MoveToShelfPartPoint(customerParent);
        if (shelfFreeItemCount >= maxShelfFreeItemCount)
        {
            shelfFreeItemCount = 0;
            myItemName = "";
            haveItem = false;
            myItemType = "";
            myShelfPoints.Clear();
            myPriceTag.MakeVisiblePriceTag(false);
            maxShelfFreeItemCount = shelfFreeItemCount;
            FurnituresParent.Instance.itemsShelfParts.Remove(this.gameObject.GetComponent<InteractableShelfPart>());
        }

    }
    private void GetShelfPartItemPoints(InteractableItem item)
    {
      Transform shelfTransform =null;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (item.itemName == transform.GetChild(i).transform.gameObject.name)
            {
               shelfTransform = transform.GetChild(i); 
                  
            }

        }
        if (shelfTransform != null)
        {
            foreach (Transform child in shelfTransform)
            {
                myShelfPoints.Add(child.gameObject);
            
                shelfFreeItemCount++;
                myItemName = item.itemName;
                myItemType = item.itemType;
                myItemGeneralIndx = item.generalItemIndx;
                haveItem = true;
                myPriceTag.MakeVisiblePriceTag(true);
                myPriceTag.SetPriceTagValues(item.generalItemIndx);
            }
            FurnituresParent.Instance.itemsShelfParts.Add(this.gameObject.GetComponent<InteractableShelfPart>());
            maxShelfFreeItemCount = shelfFreeItemCount;
        }
    }

    public void LoadShelfPartsItems()
    {
        Transform shelfTransform = null;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (myItemName == transform.GetChild(i).transform.gameObject.name)
            {
                shelfTransform = transform.GetChild(i);
                
            }

        }
        if(shelfTransform != null)
        {
            foreach (Transform child in shelfTransform)
            {
                myShelfPoints.Add(child.gameObject);
              
                myPriceTag.MakeVisiblePriceTag(true);
                myPriceTag.SetPriceTagValues(myItemGeneralIndx);
            }
            FurnituresParent.Instance.itemsShelfParts.Add(this.gameObject.GetComponent<InteractableShelfPart>());
        }
      


        for (int i = maxShelfFreeItemCount; i > shelfFreeItemCount; i--)
        {
          
            Vector3 originalScale = InstantiateManager.Instance.items_all[myItemGeneralIndx].transform.localScale;
 
            GameObject itemClone = Instantiate(InstantiateManager.Instance.items_all[myItemGeneralIndx], myShelfPoints[i - 1].transform.position, myShelfPoints[i - 1].transform.rotation);

            itemClone.transform.localScale = originalScale;

            itemClone.transform.parent = myShelfPoints[i - 1].transform;
        }


    }

    public void UpdatePreviewMode(bool isPreview)
    {
        if (isPreview)
        {
            FurnituresParent.Instance.itemsShelfParts.Remove(this.gameObject.GetComponent<InteractableShelfPart>());
        }
        else 
        {
            if(maxShelfFreeItemCount >shelfFreeItemCount)
            FurnituresParent.Instance.itemsShelfParts.Add(this.gameObject.GetComponent<InteractableShelfPart>());

        }
     
    }
}

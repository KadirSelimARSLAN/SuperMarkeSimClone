using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManagerInGame : Singleton<SaveManagerInGame>
{
    public GameData myData = new GameData();
    public InteractablePCandDesk pcandDesk;


    public Transform itemBoxParent;
    public Transform furnitureParent;
    public Transform storageParent;
    public   void Start()
    {
       
        LoadGame();
       

    }
    //For Debugging 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(FirstPersonController.Instance.previewMode !=true)
            {
                Debug.Log("Kayıt dosyası oluşturuldu");
                SaveGame();
             /* TODO : CONTROL CURRENTHANDTOOL AFTER   if (FirstPersonController.Instance.currentHandTool != FirstPersonController.Instance.currentHandTool.GetComponent<InteractablePCandDesk>() || FirstPersonController.Instance.currentHandTool != FirstPersonController.Instance.currentHandTool.GetComponent<InteractableShelf>())
                {
                  
                }*/
            }
           
            else
            {
                Debug.Log("preview olduğundan kaydedilemedi");

            }
            
        }
    }

    public void SaveGame()
    {
        SaveGameManager.CurrentSaveData.player_Money += 20;
        SaveGameManager.CurrentSaveData.player_Position =  FirstPersonController.Instance.gameObject.transform.position;
        SaveGameManager.CurrentSaveData.player_Rotation = FirstPersonController.Instance.gameObject.transform.rotation;

        SaveGameManager.CurrentSaveData.SavedItemBoxes.Clear();
        SaveGameManager.CurrentSaveData.SavedShelfs.Clear();
        SaveGameManager.CurrentSaveData.SavedStorageShelfs.Clear();
        //NAVMESH DEBUG
        AINavManager.Instance.UpdateNavMesh();
        foreach (Transform childTransform in itemBoxParent.transform)
        {
            if (childTransform.gameObject.activeInHierarchy)
            {
                InteractableItemBox itemBox = childTransform.gameObject.GetComponent<InteractableItemBox>();

                if (itemBox != null)  
                {
                    ItemBoxData currentItemBoxD = new ItemBoxData();
                    currentItemBoxD.generalitem_indx = itemBox.generalItemIndex;
                    currentItemBoxD.itemBox_Position = itemBox.transform.position;
                    currentItemBoxD.itemBox_Rotation = itemBox.transform.rotation;
                    currentItemBoxD.itemBox_indx = itemBox.itemBoxIndx;
                    currentItemBoxD.item_Count = itemBox.ItemCount;
                    currentItemBoxD.isOpen = itemBox.IsOpen();
                    SaveGameManager.CurrentSaveData.SavedItemBoxes.Add(currentItemBoxD);
                }
            }
        }
        foreach (Transform furnitureChildTransform in furnitureParent.transform)
        {

            ShelfParent interactableShelfParent = furnitureChildTransform.gameObject.GetComponent<ShelfParent>();

            ShelfData shelfData = new ShelfData();
            shelfData.shelf_Parts = new List<ShelfPartData>();

            shelfData.shelfParent_Indx = interactableShelfParent.shelfParentIndx;
            shelfData.is_BoxMode = interactableShelfParent.boxMode;  
            if (interactableShelfParent.boxMode)
            {
                shelfData.shelfBox_Position = interactableShelfParent.BoxChild.transform.position;
                shelfData.shelfBox_Rotation = interactableShelfParent.BoxChild.transform.rotation;
            }
            else
            {
                shelfData.shelf_Position = interactableShelfParent.ShelfChild.transform.position;
                shelfData.shelf_Rotation = interactableShelfParent.ShelfChild.transform.rotation;

            }

            if(interactableShelfParent.isStorage == false)
            {
                for (int i = 0; i < interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts.Length; i++)
                {

                    ShelfPartData shelfPartData = new ShelfPartData();
                    shelfPartData.generalitem_indx = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myItemGeneralIndx;
                    shelfPartData.have_Item = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().haveItem;
                    shelfPartData.my_ItemType = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myItemType;
                    shelfPartData.my_ItemName = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myItemName;
                    shelfPartData.shelf_Part_ItemCount = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().maxShelfFreeItemCount - interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().shelfFreeItemCount;
                    shelfPartData.max_Shelf_Part_ItemCount = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().maxShelfFreeItemCount;
                 

                    shelfData.shelf_Parts.Add(shelfPartData);
                }

                SaveGameManager.CurrentSaveData.SavedShelfs.Add(shelfData);
            }
            else
            {


            }
           
        }
        foreach (Transform storageChildTransform in storageParent.transform)
        {
          
            ShelfParent interactableShelfParent = storageChildTransform.gameObject.GetComponent<ShelfParent>();

            ShelfStorageData shelfData = new ShelfStorageData();
            shelfData.shelf_Parts = new List<StorageShelfPartData>();
           
            shelfData.shelfStorageParent_Indx = interactableShelfParent.shelfParentIndx;
            shelfData.is_BoxMode = interactableShelfParent.boxMode;  

            if (interactableShelfParent.boxMode)
            {
                shelfData.shelfStorageBox_Position = interactableShelfParent.BoxChild.transform.position;
                shelfData.shelfStorageBox_Rotation = interactableShelfParent.BoxChild.transform.rotation;
            }
            else
            {
             
                shelfData.shelfStorage_Position = interactableShelfParent.ShelfChild.transform.position;
                shelfData.shelfStorage_Rotation = interactableShelfParent.ShelfChild.transform.rotation;

            }

              if (interactableShelfParent.isStorage == true)
              {
                  for (int i = 0; i < interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts.Length; i++)
                  {
                  
                     StorageShelfPartData storageShelfPart = new StorageShelfPartData();
                       storageShelfPart.Item_BoxData = new List<ItemBoxData>();
                      storageShelfPart.generalitem_indx = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().myItemGeneralIndx;
                      storageShelfPart.have_Item = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().haveItem;
                      storageShelfPart.my_ItemType = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().myItemType;
                      storageShelfPart.my_ItemName = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().myItemName;
                      storageShelfPart.shelf_Part_ItemCount = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().maxShelfFreeItemCount - interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().shelfFreeItemCount;
                      storageShelfPart.max_Shelf_Part_ItemCount = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().maxShelfFreeItemCount;

                       for(int j=0;j < interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().myShelfPoints?.Count; j++)
                    { 
                       
                        if(interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().myShelfPoints[j].gameObject.transform.childCount >0)
                        {
                            InteractableItemBox itemBox = interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().myShelfPoints[j].gameObject.transform.GetChild(0).GetComponent<InteractableItemBox>();

                            if (itemBox != null)  
                            {

                                ItemBoxData currentItemBoxD = new ItemBoxData();
                                currentItemBoxD.generalitem_indx = itemBox.generalItemIndex;
                                currentItemBoxD.itemBox_Position = itemBox.transform.position;
                                currentItemBoxD.itemBox_Rotation = itemBox.transform.rotation;
                                currentItemBoxD.itemBox_indx = itemBox.itemBoxIndx;
                                currentItemBoxD.item_Count = itemBox.ItemCount;
                                currentItemBoxD.isOpen = itemBox.IsOpen();
                                storageShelfPart.Item_BoxData.Add(currentItemBoxD);
                            }

                        }
                       
                        
                    }
                      shelfData.shelf_Parts.Add(storageShelfPart);
                  }
                SaveGameManager.CurrentSaveData.SavedStorageShelfs.Add(shelfData);

            }
             
         
        }
        

        SaveGameManager.SaveGame();
    }

    public void LoadGame()
    {
        SaveGameManager.LoadGame();
        myData = SaveGameManager.CurrentSaveData;
        FirstPersonController.Instance.gameObject.transform.parent.transform.position = myData.player_Position;
        FirstPersonController.Instance.gameObject.transform.parent.transform.rotation = myData.player_Rotation;
        pcandDesk.LoadPcandDesk();
        UIManager.Instance.UpdateMoney();

      


        foreach (var itemBoxData in SaveGameManager.CurrentSaveData.SavedItemBoxes)
        {
          
            GameObject itemBoxObject = Instantiate(InstantiateManager.Instance.itemBoxes[itemBoxData.itemBox_indx], itemBoxData.itemBox_Position, itemBoxData.itemBox_Rotation);
            itemBoxObject.transform.parent = itemBoxParent;
            InteractableItemBox itemBox = itemBoxObject.GetComponent<InteractableItemBox>();

          
            if (itemBox != null)
            {
                itemBox.generalItemIndex = itemBoxData.generalitem_indx;
                itemBox.transform.position = itemBoxData.itemBox_Position;
                itemBox.transform.rotation = itemBoxData.itemBox_Rotation;
                itemBox.itemBoxIndx = itemBoxData.itemBox_indx;
                itemBox.ItemCount = itemBoxData.item_Count;
                itemBox.isOpened = itemBoxData.isOpen;
              
            }
        }
        foreach (var interactableShelfParentData in SaveGameManager.CurrentSaveData.SavedShelfs)
        {
         

          
            GameObject shelfParentObject = Instantiate(InstantiateManager.Instance.furnitureItemBox[interactableShelfParentData.shelfParent_Indx], Vector3.zero, Quaternion.identity);
            shelfParentObject.transform.parent = furnitureParent;

            ShelfParent shelf_Parent = shelfParentObject.GetComponent<ShelfParent>();

            // Set properties based on saved data
            if (shelf_Parent != null)
            {
                shelf_Parent.shelfParentIndx = interactableShelfParentData.shelfParent_Indx;
                shelf_Parent.boxMode = interactableShelfParentData.is_BoxMode;
                if (shelf_Parent.boxMode)
                {
                    shelf_Parent.BoxChild.transform.position = interactableShelfParentData.shelfBox_Position;
                    shelf_Parent.BoxChild.transform.rotation = interactableShelfParentData.shelfBox_Rotation;
                }
               else
                {

                    shelf_Parent.ShelfChild.transform.position = interactableShelfParentData.shelf_Position;
                    shelf_Parent.ShelfChild.transform.rotation = interactableShelfParentData.shelf_Rotation;
                }

                for (int i = 0; i < interactableShelfParentData.shelf_Parts.Count; i++)
                {
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myItemGeneralIndx = interactableShelfParentData.shelf_Parts[i].generalitem_indx;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().haveItem = interactableShelfParentData.shelf_Parts[i].have_Item;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myItemType = interactableShelfParentData.shelf_Parts[i].my_ItemType;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myItemName = interactableShelfParentData.shelf_Parts[i].my_ItemName;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().maxShelfFreeItemCount = interactableShelfParentData.shelf_Parts[i].max_Shelf_Part_ItemCount;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().shelfFreeItemCount = interactableShelfParentData.shelf_Parts[i].max_Shelf_Part_ItemCount -  interactableShelfParentData.shelf_Parts[i].shelf_Part_ItemCount;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().LoadShelfPartsItems();
                   

                }

            }
        }
        foreach (var interactablStorageeShelfParentData in SaveGameManager.CurrentSaveData.SavedStorageShelfs)
        {
           


            GameObject shelfParentObject = Instantiate(InstantiateManager.Instance.furnitureItemBox[interactablStorageeShelfParentData.shelfStorageParent_Indx], Vector3.zero, Quaternion.identity);
            shelfParentObject.transform.parent = storageParent;

            ShelfParent shelf_Parent = shelfParentObject.GetComponent<ShelfParent>();
         
            
            if (shelf_Parent != null)
            {
                shelf_Parent.shelfParentIndx = interactablStorageeShelfParentData.shelfStorageParent_Indx;
                shelf_Parent.boxMode = interactablStorageeShelfParentData.is_BoxMode;
                if (shelf_Parent.boxMode)
                {
                    shelf_Parent.BoxChild.transform.position = interactablStorageeShelfParentData.shelfStorageBox_Position;
                    shelf_Parent.BoxChild.transform.rotation = interactablStorageeShelfParentData.shelfStorageBox_Rotation;
                }
                else
                {

                    shelf_Parent.ShelfChild.transform.position = interactablStorageeShelfParentData.shelfStorage_Position;
                    shelf_Parent.ShelfChild.transform.rotation = interactablStorageeShelfParentData.shelfStorage_Rotation;
                }

                for (int i = 0; i < interactablStorageeShelfParentData.shelf_Parts.Count; i++)
                {
                  
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().myItemGeneralIndx = interactablStorageeShelfParentData.shelf_Parts[i].generalitem_indx;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().haveItem = interactablStorageeShelfParentData.shelf_Parts[i].have_Item;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().myItemType = interactablStorageeShelfParentData.shelf_Parts[i].my_ItemType;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().myItemName = interactablStorageeShelfParentData.shelf_Parts[i].my_ItemName;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().maxShelfFreeItemCount = interactablStorageeShelfParentData.shelf_Parts[i].max_Shelf_Part_ItemCount;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().shelfFreeItemCount = interactablStorageeShelfParentData.shelf_Parts[i].max_Shelf_Part_ItemCount - interactablStorageeShelfParentData.shelf_Parts[i].shelf_Part_ItemCount;
                    shelf_Parent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableStorageShelfPart>().LoadStorageShelfPartsItems(interactablStorageeShelfParentData.shelf_Parts[i].Item_BoxData);
                  
                }

                }
            AINavManager.Instance.UpdateNavMesh();

        }
    }


    }



[System.Serializable]
public struct ItemData
{
    
    public string product_Type;
    public string product_Name;
    public int product_Indx;
    public float product_Price;
    public float recommended_Price;
    public float price_Given;

    public ItemData(string type, string name, int index, float price, float recommendedPrice, float givenPrice)
    {
        product_Type = type;
        product_Name = name;
        product_Indx = index;
        product_Price = price;
        recommended_Price = recommendedPrice;
        price_Given = givenPrice;
    }
}
[System.Serializable]
public struct ItemBoxData
{
    public int itemBox_indx;
    public int generalitem_indx;
    public bool isOpen;
    public int item_Count;
    public Vector3 itemBox_Position;
    public Quaternion itemBox_Rotation;

    public ItemBoxData( int generalİtemindx,int index,bool open,int itemCount,Vector3 pos,Quaternion rot)
    {

        generalitem_indx = generalİtemindx;
        itemBox_indx = index;
        isOpen = open;
        item_Count = itemCount;
        itemBox_Position = pos;
        itemBox_Rotation = rot;
    }
}

[System.Serializable]
public struct ShelfData
{
    public bool is_Storage;
    public int shelfParent_Indx;
    public bool is_BoxMode;   
    public Vector3 shelf_Position;
    public Quaternion shelf_Rotation;
    public Vector3 shelfBox_Position;
    public Quaternion shelfBox_Rotation;
    public List<ShelfPartData> shelf_Parts;

    public ShelfData(bool isStorage ,int index,bool boxMode, Vector3 Shelfpos, Quaternion Shelfrot, Vector3 ShelfBoxpos, Quaternion ShelfBoxrot)
    {

        is_Storage = isStorage;
        shelfParent_Indx = index;
        is_BoxMode = boxMode;
        shelf_Position = Shelfpos;
        shelf_Rotation = Shelfrot;
        shelfBox_Position = ShelfBoxpos;
        shelfBox_Rotation = ShelfBoxrot;
        shelf_Parts = new List<ShelfPartData>(); 

    }
}

[System.Serializable]
public struct ShelfPartData
{
    public int generalitem_indx;
    public int shelf_Part_ItemCount;
    public int max_Shelf_Part_ItemCount;
    public string my_ItemType;
    public string my_ItemName;
    public bool have_Item;


    public ShelfPartData(int generalİtemindx, int itemCount,int maxItemCount, string itemType, string itemName,bool haveItem)
    {
        generalitem_indx = generalİtemindx;
        shelf_Part_ItemCount = itemCount;
        max_Shelf_Part_ItemCount = maxItemCount;
        my_ItemType = itemType;
        my_ItemName = itemName;
        have_Item = haveItem;
    }
}
[System.Serializable]
public struct StorageShelfPartData
{
    public int generalitem_indx;
    public int shelf_Part_ItemCount;
    public int max_Shelf_Part_ItemCount;
    public string my_ItemType;
    public string my_ItemName;
    public bool have_Item;
    public List<ItemBoxData> Item_BoxData;

    public StorageShelfPartData(int generalİtemindx, int itemCount, int maxItemCount, string itemType, string itemName, bool haveItem)
    {
        generalitem_indx = generalİtemindx;
        shelf_Part_ItemCount = itemCount;
        max_Shelf_Part_ItemCount = maxItemCount;
        my_ItemType = itemType;
        my_ItemName = itemName;
        have_Item = haveItem;
        Item_BoxData = new List<ItemBoxData>();
    }
}
[System.Serializable]
public struct ShelfStorageData
{
    public bool is_Storage;
    public int shelfStorageParent_Indx;
    public bool is_BoxMode;
    public Vector3 shelfStorage_Position;
    public Quaternion shelfStorage_Rotation;
    public Vector3 shelfStorageBox_Position;
    public Quaternion shelfStorageBox_Rotation;
    public List<StorageShelfPartData> shelf_Parts;
   


    public ShelfStorageData(bool isStorage, int index, bool boxMode, Vector3 Shelfpos, Quaternion Shelfrot, Vector3 ShelfBoxpos, Quaternion ShelfBoxrot)
    {

        is_Storage = isStorage;
        shelfStorageParent_Indx = index;
        is_BoxMode = boxMode;
        shelfStorage_Position = Shelfpos;
        shelfStorage_Rotation = Shelfrot;
        shelfStorageBox_Position = ShelfBoxpos;
        shelfStorageBox_Rotation = ShelfBoxrot;
        shelf_Parts = new List<StorageShelfPartData>();

    }
}


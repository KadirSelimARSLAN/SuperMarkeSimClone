using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {

    #region Player
    public float player_Money;
    public Vector3 player_Position;
    public Quaternion player_Rotation;
    #endregion

    #region ProductsData
    public List<ItemData> general_Items = new List<ItemData>();
    public List<ItemData> furniture_Items = new List<ItemData>();
    #endregion




    #region PCDESK
    public Vector3 pcAndDeskPosition;
    public Quaternion pcAndDeskRotation;
    public Vector3 pcAndDeskParentPosition;
    #endregion


    #region InteractableItemBox
    public List<ItemBoxData>SavedItemBoxes = new List<ItemBoxData>();
    #endregion

    #region Furnitures

    public List<ShelfData> SavedShelfs = new List<ShelfData>();

    public List<ShelfStorageData> SavedStorageShelfs = new List<ShelfStorageData>();


    #endregion



    public GameData()
    {
        general_Items = new List<ItemData>();
        furniture_Items = new List<ItemData>();

        general_Items.Add(new ItemData("Shoes", "Cylinder", 0, 7.95f, 10.30f, 7.95f));
        general_Items.Add(new ItemData("Shoes", "Circle", 1, 11.99f, 15.01f, 11.99f));

        furniture_Items.Add(new ItemData("Furniture", "Shelf_Mid", 0, 71.65f, 10.30f, 71.65f));
        furniture_Items.Add(new ItemData("Furniture", "Storage_Shelf", 1, 210.45f, 15.01f, 210.45f));

        pcAndDeskPosition = new Vector3(0, -3.48f, -1.3f);
        pcAndDeskRotation = Quaternion.identity;

        SavedItemBoxes = new List<ItemBoxData>();
        SavedShelfs = new List<ShelfData>();
        SavedStorageShelfs = new List<ShelfStorageData>();
    }

}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using EPOOutline;
public class PriceTagData : IInteractable
{
   
    public float item_Count;

    public int pricetag_itemIndx;
    public ItemData priceTag_Data = new ItemData();

    public TextMeshProUGUI Item_Name;
    public TextMeshProUGUI Item_Price;
    public TextMeshProUGUI ItemCountText;


    public Outlinable epo;
    private bool isInteracted = false;
    private void Start()
    {
        epo = GetComponent<Outlinable>();
        epo.AddAllChildRenderersToRenderingList();
        epo.RenderStyle = RenderStyle.FrontBack;
    }

    public void SetTexts()
    {
       
        Item_Name.text = priceTag_Data.product_Name.ToString();
        Item_Price.text = priceTag_Data.price_Given.ToString();
        ItemCountText.text = item_Count.ToString();
       
    }


    /*public PriceTagData(string type, string name, int index, float price, float recommendedPrice, float givenPrice)
    {
        product_Type = type;
        product_Name = name;
        product_Indx = index;
        product_Price = price;
        recommended_Price = recommendedPrice;
        price_Given = givenPrice;
    }*/


    public void MakeVisiblePriceTag(bool b)
    {
       
            this.gameObject.SetActive(b);
        item_Count = 0;
       

    }
    public void SetPriceTagValues(int itemIndx)
    {
        
        pricetag_itemIndx = itemIndx;
       

       
            for (int i = 0; i < SaveManagerInGame.Instance.myData.general_Items.Count; i++)
            {
                
                if (pricetag_itemIndx == SaveManagerInGame.Instance.myData.general_Items[i].product_Indx)
                {
                priceTag_Data.product_Type = SaveManagerInGame.Instance.myData.general_Items[i].product_Type;
                priceTag_Data.product_Name = SaveManagerInGame.Instance.myData.general_Items[i].product_Name;
                priceTag_Data.product_Indx = SaveManagerInGame.Instance.myData.general_Items[i].product_Indx;
                    priceTag_Data.product_Price = SaveManagerInGame.Instance.myData.general_Items[i].product_Price;
                    priceTag_Data.recommended_Price = SaveManagerInGame.Instance.myData.general_Items[i].recommended_Price;
                    priceTag_Data.price_Given = SaveManagerInGame.Instance.myData.general_Items[i].price_Given;
                    item_Count++;

                    if (priceTag_Data.price_Given <= 0)
                    {
                 
                        priceTag_Data.price_Given = priceTag_Data.product_Price;
                    }
                  
                  
                
            }

        }
       
        SetTexts();

    }
    public void SetStorageTagValues(string itemType, string itemName)
    {
        priceTag_Data.product_Type = itemType;

        priceTag_Data.product_Name = itemName;

       

        SetTexts();

    }
    /*   public void  SetPriceTagValues(InteractableItem item)
       {
           priceTag_Data.product_Name =

           product_Type = item.itemType;
           product_Name = item.itemName;

           if(product_Type == "Shoes")
           {
               for (int i = 0;i< SaveManagerInGame.Instance.myData.shoes_Items.Count;i++)
               {

                   if (product_Name == SaveManagerInGame.Instance.myData.shoes_Items[i].product_Name)
                   {
                       product_Indx = SaveManagerInGame.Instance.myData.shoes_Items[i].product_Indx;
                       product_Price = SaveManagerInGame.Instance.myData.shoes_Items[i].product_Price;
                       recommended_Price =SaveManagerInGame.Instance.myData.shoes_Items[i].recommended_Price;
                       price_Given = SaveManagerInGame.Instance.myData.shoes_Items[i].price_Given;
                       if (price_Given <= 0)
                       {

                           price_Given = product_Price;
                       }
                   }
               }

           }
         //Other item type ifs here



           SetTexts();




       }*/

    public void InteractWithPriceTag()
    {

        UIManager.Instance.SetPricePanelInteractUI(this.gameObject.GetComponent<PriceTagData>());

    }

    public override void OnInteract(Transform playerHandPos)
    {
        InteractWithPriceTag();
    }

    public override void OnFocus()
    {
        epo.enabled = true;

        epo.FrontParameters.Color = Color.green;
        epo.BackParameters.Color = new Color(1f, 0f, 0f, 0f);
    }

    public override void OnLoseFocus()
    {
        epo.enabled = false;
    }

    public override bool readyForInteract()
    {
        throw new System.NotImplementedException();
    }
}

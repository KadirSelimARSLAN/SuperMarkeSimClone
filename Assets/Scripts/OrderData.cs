using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class OrderData : MonoBehaviour
{
    public string product_Type;
    public string product_Name;
    public int product_Indx;
    public float product_Price;
    public float recommended_Price;
    public float price_Given;
    public int order_Count;
      

    public TextMeshProUGUI Item_Name;
    public TextMeshProUGUI Item_Price;
    public TextMeshProUGUI Order_Count_Text;

    public void SetTexts()
    {
        Item_Name.text = product_Name.ToString();
        Item_Price.text = product_Price.ToString();
        Order_Count_Text.text = order_Count.ToString();

    }

    public OrderData(string type, string name, int index, float price, float recommendedPrice, float givenPrice)
    {
        product_Type = type;
        product_Name = name;
        product_Indx = index;
        product_Price = price;
        recommended_Price = recommendedPrice;
        price_Given = givenPrice;
    }

    public void DeleteOrder()
    {
        PCGUIManager.Instance.DeleteOrderFromOrderList(this.gameObject);
     
    }

    public void IncrementantTextValue()
    {

        order_Count++;
        SetTexts();
        PCGUIManager.Instance.CalculateTotalOrderPrice();

    }
    public void DecrementantTextValue()
    {
        order_Count--;
        SetTexts();
        PCGUIManager.Instance.CalculateTotalOrderPrice();
        if (order_Count < 1)
        {
            PCGUIManager.Instance.DeleteOrderFromOrderList(this.gameObject);
        }

    }
}

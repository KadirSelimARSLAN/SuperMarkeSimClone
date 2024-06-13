using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class PCGUIManager : Singleton<PCGUIManager>
{
    private bool isPcMode = false;
    public bool IsPcMode { get => isPcMode; set => isPcMode = value; }

    // World Canvas sınırlarını almak için RectTransform
    public RectTransform canvasRect;

    // Fare imleci görüntüsünü temsil eden GameObject
    public GameObject cursorImage;
    public Vector2 cursorOffset;


    public OrderData currentSelectedItem;
    public List<GameObject> current_Order_Data = new List<GameObject>();

    public Transform orderParent;
    public GameObject orderPrefab;
    public TextMeshProUGUI totalOrderPriceText;
    public TextMeshProUGUI orderCountText;
    public List<TextMeshProUGUI> changedOrderCountTexts = new List<TextMeshProUGUI>();
   public int orderCountTextValue = 0;
    public float totalOrderPrice;
    [Header("ScrollsShop")]
    public GameObject products_Scroll;
    public GameObject furniture_Scroll;
    [Header("ProductsPanels")]
    public GameObject product_Shoes_Scroll;
    public GameObject x_Scroll;
    [Header("Panels")]
    public GameObject desktopPanel;
    public GameObject shopPanel;
    public GameObject basketPanel;
    

    void Update()
    {
        if (IsPcMode)
        {
            // Fare pozisyonunu ekran koordinatlarına dönüştür
            Vector3 screenMousePos = Input.mousePosition;

            // Fare pozisyonunu dünya koordinatlarına dönüştür
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(screenMousePos);

            // Fare pozisyonunu Canvas'in yerel koordinatlarına dönüştür
            Vector2 canvasMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenMousePos, Camera.main, out canvasMousePos);

            // Canvas sınırları içinde sınırla
            canvasMousePos.x = Mathf.Clamp(canvasMousePos.x, canvasRect.rect.xMin, canvasRect.rect.xMax);
            canvasMousePos.y = Mathf.Clamp(canvasMousePos.y, canvasRect.rect.yMin, canvasRect.rect.yMax);

            // Fare imleci görüntüsünün pozisyonunu güncelle
            cursorImage.transform.localPosition = canvasMousePos + cursorOffset;
        }
       
    }

   
    public void DesktopButtons(int indx)
    {
        if (indx == 0)
        {
           shopPanel.SetActive(true);   
        }
    }
   
    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
       
    }
    public void CloseBasketPanel()
    {
        basketPanel.SetActive(false);
    }
    public void OpenBasketPanel()
    {
        if (current_Order_Data.Count > 0)
        {
            for (int i = 0; i < changedOrderCountTexts.Count; i++)
            {

                changedOrderCountTexts[i].text = "1";

            }
            changedOrderCountTexts.Clear();
            basketPanel.SetActive(true);
        }
       
    }

    public void ShopMainPageScroll(int Scrollindex)
    {
       if(Scrollindex == 0)
        {
            products_Scroll.SetActive(true);
            furniture_Scroll.SetActive(false);
        }
       else if(Scrollindex==1)
        {
            products_Scroll.SetActive(false);
            furniture_Scroll.SetActive(true);
        }
          
    }
    public void ShopProductsPageScroll(int Scrollindex)
    {
        if (Scrollindex == 0)
        {
            product_Shoes_Scroll.SetActive(true);
            x_Scroll.SetActive(false);
        }
        else if (Scrollindex == 1)
        {
            product_Shoes_Scroll.SetActive(false);
            x_Scroll.SetActive(true);
        }

    }
    #region ORDER
    public void ClearAllChangedOrderTexts()
    {

        for (int i = 0; i < changedOrderCountTexts.Count; i++)
        {

            changedOrderCountTexts[i].text = "1";

        }
        changedOrderCountTexts.Clear();
    }
    public void OrderShoesBox(int indx)
    {
        if(indx <=SaveManagerInGame.Instance.myData.general_Items.Count )
        {
           
            bool sameOrder = false;
            for (int i = current_Order_Data.Count - 1; i >= 0; i--)
            {

                if (current_Order_Data[i].GetComponent<OrderData>().product_Name == SaveManagerInGame.Instance.myData.general_Items[indx].product_Name)
                {
                    current_Order_Data[i].GetComponent<OrderData>().order_Count =  current_Order_Data[i].GetComponent<OrderData>().order_Count + orderCountTextValue;
                    current_Order_Data[i].GetComponent<OrderData>().SetTexts();
                    sameOrder = true;
                }
            }

                if(!sameOrder)
                {
                    var currentOrder = Instantiate(orderPrefab, orderParent.position, orderParent.transform.rotation);
                    currentOrder.transform.parent = orderParent.transform;
                    currentOrder.transform.localScale = new Vector3(1, 1, 1);
                    currentOrder.GetComponent<OrderData>().product_Type = SaveManagerInGame.Instance.myData.general_Items[indx].product_Type;
                    currentOrder.GetComponent<OrderData>().product_Indx = SaveManagerInGame.Instance.myData.general_Items[indx].product_Indx;
                    currentOrder.GetComponent<OrderData>().product_Name = SaveManagerInGame.Instance.myData.general_Items[indx].product_Name;
                    currentOrder.GetComponent<OrderData>().product_Price = SaveManagerInGame.Instance.myData.general_Items[indx].product_Price;
                currentOrder.GetComponent<OrderData>().order_Count =  currentOrder.GetComponent<OrderData>().order_Count + orderCountTextValue;
                currentOrder.GetComponent<OrderData>().SetTexts();
                    current_Order_Data.Add(currentOrder);
                }
            
          
            orderCountText.gameObject.SetActive(true);
            totalOrderPriceText.gameObject.SetActive(true);
            orderCountText.text = current_Order_Data.Count.ToString();
            CalculateTotalOrderPrice();

        }

        
      
      
    }
    public void OrderFurnitureBox(int indx)
    {
        if (indx <= SaveManagerInGame.Instance.myData.furniture_Items.Count)
        {
            bool sameOrder = false;
            for (int i = current_Order_Data.Count - 1; i >= 0; i--)
            {

                if (current_Order_Data[i].GetComponent<OrderData>().product_Name == SaveManagerInGame.Instance.myData.furniture_Items[indx].product_Name)
                {
                    current_Order_Data[i].GetComponent<OrderData>().order_Count = current_Order_Data[i].GetComponent<OrderData>().order_Count + orderCountTextValue;
                    current_Order_Data[i].GetComponent<OrderData>().SetTexts();
                    sameOrder = true;
                }
            }

            if (!sameOrder)
            {
                var currentOrder = Instantiate(orderPrefab, orderParent.position, orderParent.transform.rotation);
                currentOrder.transform.parent = orderParent.transform;
                currentOrder.transform.localScale = new Vector3(1, 1, 1);
                currentOrder.GetComponent<OrderData>().product_Type = SaveManagerInGame.Instance.myData.furniture_Items[indx].product_Type;
                currentOrder.GetComponent<OrderData>().product_Indx = SaveManagerInGame.Instance.myData.furniture_Items[indx].product_Indx;
                currentOrder.GetComponent<OrderData>().product_Name = SaveManagerInGame.Instance.myData.furniture_Items[indx].product_Name;
                currentOrder.GetComponent<OrderData>().product_Price = SaveManagerInGame.Instance.myData.furniture_Items[indx].product_Price;
                currentOrder.GetComponent<OrderData>().order_Count = currentOrder.GetComponent<OrderData>().order_Count + orderCountTextValue;
                currentOrder.GetComponent<OrderData>().SetTexts();
                current_Order_Data.Add(currentOrder);
            }


            orderCountText.gameObject.SetActive(true);
            totalOrderPriceText.gameObject.SetActive(true);
            orderCountText.text = current_Order_Data.Count.ToString();
            CalculateTotalOrderPrice();

        }

    }
    public void DeleteOrderFromOrderList(GameObject order)
    {

        current_Order_Data.Remove(order.gameObject);
        Destroy(order);
       
        if (current_Order_Data.Count <= 0)
        {
            orderCountText.gameObject.SetActive(false);
            totalOrderPriceText.gameObject.SetActive(false);
            basketPanel.SetActive(false);
        }
        else
        {
            orderCountText.text = current_Order_Data.Count.ToString();
            CalculateTotalOrderPrice();
           
        }
      
        
    }

   public void CalculateTotalOrderPrice()
    {
         totalOrderPrice = 0 ;
        for(int i = 0; i < current_Order_Data.Count; i++)
        {
            
         totalOrderPrice += (current_Order_Data[i].GetComponent<OrderData>().product_Price * current_Order_Data[i].GetComponent<OrderData>().order_Count);
            
            
        }
        totalOrderPriceText.text = "Total Order Price :" + totalOrderPrice.ToString("00.00");
        
    }


    public void GetBuyOrders()
    {
        if(SaveManagerInGame.Instance.myData.player_Money >= totalOrderPrice)
        { 
            for (int i = current_Order_Data.Count - 1; i >= 0; i--)
            {
                for(int j = 0; j < current_Order_Data[i].GetComponent<OrderData>().order_Count; j++)
                {
                    InstantiateManager.Instance.CloneItemBox(current_Order_Data[i].GetComponent<OrderData>());
                   
                }
                DeleteOrderFromOrderList(current_Order_Data[i]);
            }
        }

    }
    public void IncrementantTextValue(TextMeshProUGUI Order_CountText)
    {

        int value = int.Parse(Order_CountText.text);

        value++;
        changedOrderCountTexts.Add(Order_CountText);
        Order_CountText.text = value.ToString();
    }
    public void DecrementantTextValue(TextMeshProUGUI Order_CountText)
    {
        int value = int.Parse(Order_CountText.text);
        if(value-1 >=1)
        {
            value--;
            changedOrderCountTexts.Add(Order_CountText);
            Order_CountText.text = value.ToString();
        }
      
    }
    public void OrderCountTextValue(TextMeshProUGUI Order_CountText)
    {

         orderCountTextValue = int.Parse(Order_CountText.text);

    }
    #endregion
}

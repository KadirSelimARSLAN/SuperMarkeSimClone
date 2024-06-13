using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI staminaText=default;
    [SerializeField] private Image defaultCursor;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI dayTimeText = default;

    //SETPRICE 
    [SerializeField] private GameObject setPricePanel;
   
    [SerializeField] private TextMeshProUGUI product_Price_Text;
    [SerializeField] private TextMeshProUGUI product_Recommend_Price_Text;
    [SerializeField] private TextMeshProUGUI net_Profit_Text;
    [SerializeField] private TextMeshProUGUI Product_Name;
    
    [SerializeField] private TMP_InputField priceInputField;
    public int productItemIndex;
    public ItemData UIman_priceTag_Data = new ItemData();
    public PriceTagData currentInteractPriceTag;
    private string productTypee;
    private float Current_Price_Val = 0;
    private bool isSetPriceMode = false;
    private void OnEnable()
    {
        FirstPersonController.OnStaminaChange += UpdateStamina;
      

    }
    private void OnDisable()
    {
        FirstPersonController.OnStaminaChange -= UpdateStamina;
     
    }
    private void Start()
    {
        
        UpdateStamina(100);
    }

    private void Update()
    {
        UpdateDayTime();
        if (isSetPriceMode == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseSetPricePanel();
               
            }
        }
    }
    public void UpdateDayTime()
    {
    
    }
    private void UpdateStamina(float currentStamina)
    {
        staminaText.text = currentStamina.ToString("00");
    }
    public void UpdateMoney()
    {
        moneyText.text = SaveManagerInGame.Instance.myData.player_Money.ToString();
    }
  
    public void VisibleDefaultCursor(bool visible)
    {
       
        if (visible)
        {
            defaultCursor.enabled = true;
        }
        else
        {
            defaultCursor.enabled = false;

        }
     
    }

    public void SetPricePanelInteractUI(PriceTagData priceTag)
    {
        isSetPriceMode = true;
        UIman_priceTag_Data = priceTag.priceTag_Data;
        currentInteractPriceTag = priceTag;
        productTypee = priceTag.priceTag_Data.product_Type;
        Product_Name.text = priceTag.priceTag_Data.product_Name.ToString(); ;
        product_Price_Text.text = priceTag.priceTag_Data.product_Price.ToString();
        product_Recommend_Price_Text.text = "Recommended Price : " + priceTag.priceTag_Data.recommended_Price.ToString() + "$"; ;
        Current_Price_Val = priceTag.priceTag_Data.price_Given;
        net_Profit_Text.text = "Profit : " + (priceTag.priceTag_Data.price_Given - priceTag.priceTag_Data.product_Price).ToString("F2") + "$";
        priceInputField.text = priceTag.priceTag_Data.price_Given.ToString();



        FirstPersonController.Instance.canMove = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        VisibleDefaultCursor(false);
        setPricePanel.SetActive(true);
    }
  
    public void InputFieldUpdate()
    {   
        if(priceInputField.text !="" && priceInputField.text != null && priceInputField.text != "." && priceInputField.text != "-")
        {   
            Current_Price_Val = (float.Parse(priceInputField.text));          
            net_Profit_Text.text = "Profit : " + (Current_Price_Val - float.Parse(product_Price_Text.text)).ToString("F2") + "$";
           

        }
        else
        {    
            net_Profit_Text.text = "Profit : " + (Current_Price_Val - float.Parse(product_Price_Text.text)).ToString("F2") + "$";
            priceInputField.placeholder.GetComponent<TextMeshProUGUI>().text = Current_Price_Val.ToString();
        }
         

    }
    public void SetInputFieldEnd()
    {
        GetDecimalPart();
    }
    public void GetDecimalPart()
    {
        string inputText = priceInputField.text;

        // . karakterine göre bölme
        string[] parts = inputText.Split('.');

        // Eğer . karakteri yoksa veya ondalık kısmın sağında iki basamak yoksa çıkış yap
        if (parts.Length != 2)
        {
            Debug.Log("Ondalık kısım yok veya iki basamak yok.");

            string integerPart2 = parts[0].TrimStart('0');
            if (string.IsNullOrEmpty(integerPart2))
            {
                integerPart2 = "0";
            }
            float result2;
            if (float.TryParse(inputText, out result2))
            {

                if (float.Parse(inputText) < 0)
                {
                    integerPart2 = "0";
                }
            }else
            { integerPart2 = "0"; }

            // Negatif kontrolü yapın
          

            priceInputField.text = integerPart2;

           
        }
     else
        {
            // Ondalık kısmın ilk iki basamağını al
            string decimalPart = parts[1].Substring(0, Mathf.Min(2, parts[1].Length));

            // Başındaki sıfırları kaldırmak için TrimStart('0') kullanın
            string integerPart = parts[0].TrimStart('0');

            // Eğer integerPart boş ise, bir sıfır ekle
            if (string.IsNullOrEmpty(integerPart))
            {
                integerPart = "0";
            }

            // Negatif kontrolü yapın
            float result;
            if (float.TryParse(inputText, out result))
            {

                if (float.Parse(inputText) < 0)
                {
                    integerPart = "0";
                }
            }
            priceInputField.text = integerPart + "." + decimalPart;
        }
       

       
      

        UIman_priceTag_Data.price_Given = float.Parse(priceInputField.text);

        if (productTypee == "Shoes")
        {
            for (int i = 0; i < SaveManagerInGame.Instance.myData.general_Items.Count; i++)
            {

                if (Product_Name.text == SaveManagerInGame.Instance.myData.general_Items[i].product_Name)
                {
                    
                    SaveManagerInGame.Instance.myData.general_Items[i] = UIman_priceTag_Data;
                    currentInteractPriceTag.priceTag_Data = UIman_priceTag_Data;

                }
            }

        }

        currentInteractPriceTag.SetTexts();
      
    }



    public void CloseSetPricePanel()
    {
        isSetPriceMode = false;
      GameManager.Instance.SetAllPriceTag(currentInteractPriceTag.pricetag_itemIndx);
        FirstPersonController.Instance.canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        VisibleDefaultCursor(true);
        setPricePanel.SetActive(false);


      
    }
    public void SetAllPriceTag(int indx)
    {
        foreach (Transform furnitureChildTransform in InstantiateManager.Instance.furnitureParent.transform)
        {

            ShelfParent interactableShelfParent = furnitureChildTransform.gameObject.GetComponent<ShelfParent>();


            
                for (int i = 0; i < interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts.Length; i++)
                {


                    if (indx == interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myItemGeneralIndx)
                    {
                       
                        interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myPriceTag.SetPriceTagValues(currentInteractPriceTag.pricetag_itemIndx);

                    }

                }


            



        }
    }
}

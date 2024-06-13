using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BagPointPOS : MonoBehaviour
{
    public GameObject POSItemBarPrefab;
    public Transform POSItemBarParent;
    public List<int> currentlyObtainedItems = new List<int>();
    public float currentTotalPOSItemsPrice;
    public float currentChangeValue;
    public float currentGivenMoneyValue;
    public TextMeshProUGUI currentTotalPOSItemsPrice_Text;
    public TextMeshProUGUI currentPOSFeeTotalReceived_Text;
    public TextMeshProUGUI currentPOSFeeTotal_Text;
    public TextMeshProUGUI currentPOSFeeChange_Text;
    public TextMeshProUGUI currentPOSFeeGivenMoney;

    //POS ITEM LOCS
    public GameObject posItemLocParent;
    public Vector3 firstPositionPosItemLocParent;
    private void Start()
    {
        firstPositionPosItemLocParent = posItemLocParent.transform.position;
    }
    public void AddItemToList(int indx)
    {

        bool isSame = false;
        currentlyObtainedItems.Add(indx);
    
        for (int j = 0; j < POSItemBarParent.childCount; j++)
        {
            if (POSItemBarParent.GetChild(j).GetComponent<POSItemData>().product_Indx == indx)
            {
                POSItemBarParent.GetChild(j).GetComponent<POSItemData>().order_Count++;
                POSItemBarParent.GetChild(j).GetComponent<POSItemData>().SetTexts();
                isSame = true;
            }
        }
        if (isSame == false)
        {
          
            var POSItemBarPrefabClone = Instantiate(POSItemBarPrefab, POSItemBarParent.transform.position, POSItemBarParent.transform.rotation);
            POSItemBarPrefabClone.transform.parent = POSItemBarParent.transform;
            POSItemBarPrefabClone.transform.localScale = new Vector3(1, 1, 1);

            POSItemBarPrefabClone.GetComponent<POSItemData>().product_Type = SaveManagerInGame.Instance.myData.general_Items[indx].product_Type;
            POSItemBarPrefabClone.GetComponent<POSItemData>().product_Indx = SaveManagerInGame.Instance.myData.general_Items[indx].product_Indx;
            POSItemBarPrefabClone.GetComponent<POSItemData>().product_Name = SaveManagerInGame.Instance.myData.general_Items[indx].product_Name;
            POSItemBarPrefabClone.GetComponent<POSItemData>().price_Given = SaveManagerInGame.Instance.myData.general_Items[indx].price_Given;
            POSItemBarPrefabClone.GetComponent<POSItemData>().order_Count = 1;
            POSItemBarPrefabClone.GetComponent<POSItemData>().SetTexts();
        }
   
        //ADD ALLWAYS
        currentTotalPOSItemsPrice += SaveManagerInGame.Instance.myData.general_Items[indx].price_Given;
        currentTotalPOSItemsPrice_Text.text = currentTotalPOSItemsPrice.ToString("C");
        

        if (currentlyObtainedItems.Count == transform.parent.GetComponent<InteractablePOS>().CurrentTotalPosItemCount)
        {
            transform.parent.GetComponent<InteractablePOS>().myCustomers[0].CalculateMyMoney(currentTotalPOSItemsPrice);

           

        }

        //POITEMLOCATIONS
        
    }

    public void ShowPOSFeeScreen(float CustomerPaid)
    {

        currentPOSFeeTotal_Text.text = currentTotalPOSItemsPrice.ToString("C");
        currentPOSFeeTotalReceived_Text.text = CustomerPaid.ToString("C");

        currentChangeValue = CustomerPaid - currentTotalPOSItemsPrice;

        if (currentChangeValue != 0.0f)
        {
          //  int firstDecimalDigit = (int)(currentChangeValue * 10) % 10;
          /*  if (firstDecimalDigit != 0)
            {
                // Ondalık kısmın ilk rakamı sıfır değilse, 0.1'e yuvarla
                currentChangeValue = Mathf.Round(currentChangeValue * 10.0f) * 0.1f;
            }
            else
            {*/
                // Ondalık kısmın ilk rakamı sıfırsa, 0.01'e yuvarla
                currentChangeValue = Mathf.Round(currentChangeValue * 100.0f) * 0.01f;
           // }
        }
        currentPOSFeeChange_Text.text = currentChangeValue.ToString("C");
    }
    public void ChangeGivenMoneyText(float givenMoney)
    {
        currentGivenMoneyValue += givenMoney;
        //int firstDecimalDigit = (int)(currentGivenMoneyValue * 10) % 10;
       /* if (firstDecimalDigit != 0)
        {
            // Ondalık kısmın ilk rakamı sıfır değilse, 0.1'e yuvarla
            currentGivenMoneyValue = Mathf.Round(currentGivenMoneyValue * 10.0f) * 0.1f;
        }
        else
        {*/
            // Ondalık kısmın ilk rakamı sıfırsa, 0.01'e yuvarla
            currentGivenMoneyValue = Mathf.Round(currentGivenMoneyValue * 100.0f) * 0.01f;
 
        currentPOSFeeGivenMoney.text = currentGivenMoneyValue.ToString("C");

        if(currentGivenMoneyValue != currentChangeValue)
        {
            currentPOSFeeGivenMoney.color = Color.red;
        }
        else
        {
            currentPOSFeeGivenMoney.color = Color.green;

        }
    }
    public void ClearAllItemList()
    {
        currentPOSFeeTotal_Text.text = "$0.0";
        currentPOSFeeTotalReceived_Text.text = "$0.0";
        currentPOSFeeChange_Text.text = "$0.0";
        currentPOSFeeGivenMoney.text = "Giving...";
        currentPOSFeeGivenMoney.color = Color.red;

        currentTotalPOSItemsPrice = 0;
        currentTotalPOSItemsPrice_Text.text = currentTotalPOSItemsPrice.ToString("C");

        currentlyObtainedItems.Clear();

        for(int i = 0; i < POSItemBarParent.childCount; i++)
        {
            Destroy(POSItemBarParent.GetChild(i).transform.gameObject);   

        }
    }

  
}

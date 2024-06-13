using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AINav_Customer : MonoBehaviour
{
    public InteractableShelfPart ShelfTarget = null; // Müşterinin gitmesi gereken hedef

    private NavMeshAgent navMeshAgent;

    //Order Part
    public List<int> orderListGeneralItemIndx = new List<int>();
    public int currentOrder = 0;
    public List<int> customer_Items = new List<int>();
    // Event to be triggered when destination is reached and rotation is completed
    public UnityEvent onDestinationReachedAndRotated;
    public float distanceToTarget = 1.5f;
    private Vector3 currentTargetPos;
    public bool isOrderEnd = false;
    public bool found_Item = false;


    public Transform bagPoint;
    public int startIndex = 0; // Starting index for item search (randomized later)
    public int customerPOSQueueIndx;
    public bool showItems = false;

    public bool amICurrentCustomer = false;

    //DEBUG
    public InteractablePOS POS;
    public List<float> options = new List<float>();
    public float hundredsPlace;
    //Customer Money
    public float myMoney = 0;
    public float remainder = 0;
    public bool giveMoney = false;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // NavMeshAgent bileşenini al
        startIndex = Random.Range(0, FurnituresParent.Instance.itemsShelfParts.Count); // Set random starting index

    }

    private void Update()
    {
        if (currentOrder < orderListGeneralItemIndx.Count && Input.GetKeyDown(KeyCode.X))
        {
            SetDestination();
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending && ShelfTarget != null && !isOrderEnd)
        {

            if (navMeshAgent.destination.x != currentTargetPos.x || navMeshAgent.destination.z != currentTargetPos.z)
            {
                ShelfTarget = null;
                found_Item = false;
                SetDestination();
            }
            else
            {
                OnDestinationReached();

            }

        }
        else if (isOrderEnd && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending && !showItems)
        {

            OnDestinationReachedPOS();
        }
    }

    public void SetDestination()
    {
        if (currentOrder < orderListGeneralItemIndx.Count)
        {
            int currentIndex = startIndex; // Start search from the assigned index

            for (int i = 0; i < FurnituresParent.Instance.itemsShelfParts.Count; i++)
            {
                int index = (currentIndex + i) % FurnituresParent.Instance.itemsShelfParts.Count; // Loop back after reaching the end

                if (orderListGeneralItemIndx[currentOrder] == FurnituresParent.Instance.itemsShelfParts[index].myItemGeneralIndx && FurnituresParent.Instance.itemsShelfParts[index].transform.parent.GetComponent<InteractableShelf>().isInteractWithNPC == false)
                {

                    ShelfTarget = FurnituresParent.Instance.itemsShelfParts[index].transform.GetComponent<InteractableShelfPart>();

                    found_Item = true;
                    break;
                }
            }

            if (found_Item == true)
            {

                Vector3 globalForward = ShelfTarget.transform.TransformDirection(Vector3.forward);
                currentTargetPos = ShelfTarget.transform.position + globalForward * distanceToTarget;
                navMeshAgent.SetDestination(currentTargetPos);

            }
            else
            {
                ShelfTarget = null;
                //   Debug.Log("Aradığımı bulamadım");
                currentOrder++;
                SetDestination();
            }
        }
        else
        {
            if (customer_Items.Count > 0)
            {
                ShelfTarget = null;
                isOrderEnd = true;


                POS.AddCustomerToList(this.gameObject.GetComponent<AINav_Customer>());
            }
            else
            {
                GoExit();
            }


        }


    }

    public void OnDestinationReached()
    {

        // Ajanın hedefe dönmesi ve bakması
        Vector3 direction = (ShelfTarget.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 6); // rotationSpeed hızında dönme


        if (Quaternion.Angle(transform.rotation, lookRotation) < 1f)
        {

            if ((ShelfTarget.maxShelfFreeItemCount - ShelfTarget.shelfFreeItemCount) > 0 && navMeshAgent.destination.x == currentTargetPos.x && navMeshAgent.destination.z == currentTargetPos.z)
            {
                ShelfTarget.SendItemBacktoCustomer(ShelfTarget.shelfFreeItemCount, bagPoint);
                found_Item = false;
                customer_Items.Add(orderListGeneralItemIndx[currentOrder]);
                currentOrder++;
                ShelfTarget = null;
                startIndex = Random.Range(0, FurnituresParent.Instance.itemsShelfParts.Count); // Reset starting index for next order
                SetDestination();



            }
            else
            {
                ShelfTarget = null;
                found_Item = false;
                SetDestination();

            }

        }
    }
    public void OnDestinationReachedPOS()
    {

        // Ajanın hedefe dönmesi ve bakması
        Vector3 direction = (POS.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 6); // rotationSpeed hızında dönme


        if (Quaternion.Angle(transform.rotation, lookRotation) < 1f)
        {

            if (customerPOSQueueIndx == 0 && amICurrentCustomer)
            {
                showItems = true;

                POS.AddCustomerItemsOnPOS(customer_Items);

                bagPoint.gameObject.SetActive(false);

            }
        }
    }

    public void GoPOSQueue(Transform POSPos, int customer_Indx)
    {
        customerPOSQueueIndx = customer_Indx;
        navMeshAgent.SetDestination(POSPos.position);
    }
    public void GoExit()
    {
        bagPoint.gameObject.SetActive(true);
        navMeshAgent.SetDestination(Vector3.zero);
    }
    public void CalculateMyMoney(float totalOrderPrice)
    {

        bool isInteger = false;
        float hundredsPlace = totalOrderPrice;
        if (totalOrderPrice % 5 == 0)
        {
            isInteger = true;
        }
        if (hundredsPlace > 100)
        {
            hundredsPlace = (int)(totalOrderPrice % 100);
            remainder = totalOrderPrice - hundredsPlace;
      
        }

        if (isInteger)
        {  
            

            if (hundredsPlace == 0)
            {

                options.Add(totalOrderPrice);

            }

            else if (0 != hundredsPlace && hundredsPlace <= 5)
            {


                if (hundredsPlace != 5)
                {
                    options.Add(totalOrderPrice);
                }
                options.Add(5 + remainder);
                options.Add(10 + remainder);
                options.Add(20 + remainder);
                options.Add(50 + remainder);
                options.Add(100 + remainder);

            }
            else if (hundredsPlace <= 10)
            {

                if (hundredsPlace != 10)
                {
                    options.Add(totalOrderPrice);
                }
                options.Add(10 + remainder);
                options.Add(20 + remainder);
                options.Add(50 + remainder);
                options.Add(100 + remainder);

            }
            else if (hundredsPlace <= 20)
            {

                if (hundredsPlace != 20)
                {
                    options.Add(totalOrderPrice);
                }
                options.Add(20 + remainder);
                options.Add(50 + remainder);
                options.Add(100 + remainder);

            }
            else if (hundredsPlace <= 50)
            {
                if (hundredsPlace != 50)
                {
                    options.Add(totalOrderPrice);
                }
                options.Add(50 + remainder);
                options.Add(100 + remainder);
            }
            else if (50 < hundredsPlace && hundredsPlace <= 100)
            {

                if (hundredsPlace != 100)
                {
                    options.Add(totalOrderPrice);
                }
                options.Add(100 + remainder);
            }

        }
        else 
        {
            //BU KISIMDA KÜSURAT EKLENMELİ
            //MESELA 123.54$ İSE
            int remainderInt = (int)Mathf.Floor(remainder);
            float fractionalPart = totalOrderPrice - (int)totalOrderPrice;
            if (hundredsPlace == 0)
            {
                int hundredsPlaceInt = (int)Mathf.Floor(hundredsPlace);

                options.Add(totalOrderPrice);
                if (fractionalPart < 0.25)
                {
                    options.Add(hundredsPlaceInt + 0.25f + remainderInt);
                }
                if(fractionalPart < 0.5f)
                {
                    options.Add(hundredsPlaceInt + 0.5f + remainderInt);
                }
                if(0.5<fractionalPart && fractionalPart < 0.75f)
                {
                    options.Add(hundredsPlaceInt + 0.75f + remainderInt);
                }
                options.Add(hundredsPlaceInt + 1 + remainderInt);
            }

            else if (0 != hundredsPlace && hundredsPlace <= 5)
            {
                int hundredsPlaceInt = (int)Mathf.Floor(hundredsPlace);

                if (hundredsPlaceInt != 5)
                {
                    options.Add(totalOrderPrice);
                }
                if (fractionalPart < 0.25)
                {
                    options.Add(hundredsPlaceInt + 0.25f + remainderInt);
                }
                if (fractionalPart < 0.5f)
                {
                    options.Add(hundredsPlaceInt + 0.5f + remainderInt);
                }
                if (0.5 < fractionalPart && fractionalPart < 0.75f)
                {
                    options.Add(hundredsPlaceInt + 0.75f + remainderInt);
                }
                options.Add(hundredsPlaceInt + 1 + remainderInt);
                options.Add(5 + remainderInt);
                options.Add(10 + remainderInt);
                options.Add(20 + remainderInt);
                options.Add(50 + remainderInt);
                options.Add(100 + remainderInt);

            }
            else if (hundredsPlace <= 10)
            {
                int hundredsPlaceInt = (int)Mathf.Floor(hundredsPlace);
                if (hundredsPlaceInt != 10)
                {
                    options.Add(totalOrderPrice);
                }
                if (fractionalPart < 0.25)
                {
                    options.Add(hundredsPlaceInt + 0.25f + remainderInt);
                }
                if (fractionalPart < 0.5f)
                {
                    options.Add(hundredsPlaceInt + 0.5f + remainderInt);
                }
                if (0.5 < fractionalPart && fractionalPart < 0.75f)
                {
                    options.Add(hundredsPlaceInt + 0.75f + remainderInt);
                }
                options.Add(hundredsPlaceInt + 1 + remainderInt);
                options.Add(10 + remainderInt);
                options.Add(20 + remainderInt);
                options.Add(50 + remainderInt);
                options.Add(100 + remainderInt);

            }
            else if (hundredsPlace <= 20)
            {
                int hundredsPlaceInt = (int)Mathf.Floor(hundredsPlace);
                if (hundredsPlaceInt != 20)
                {
                    options.Add(totalOrderPrice);
                }
                if (fractionalPart < 0.25)
                {
                    options.Add(hundredsPlaceInt + 0.25f + remainderInt);
                }
                if (fractionalPart < 0.5f)
                {
                    options.Add(hundredsPlaceInt + 0.5f + remainderInt);
                }
                if (0.5 < fractionalPart && fractionalPart < 0.75f)
                {
                    options.Add(hundredsPlaceInt + 0.75f + remainderInt);
                }
                options.Add(hundredsPlaceInt + 1 + remainderInt);
                options.Add(20 + remainderInt);
                options.Add(50 + remainderInt);
                options.Add(100 + remainderInt);

            }
            else if (hundredsPlace <= 50)
            {
                int hundredsPlaceInt = (int)Mathf.Floor(hundredsPlace);
                if (hundredsPlaceInt != 50)
                {
                    options.Add(totalOrderPrice);
                }
                if (fractionalPart < 0.25)
                {
                    options.Add(hundredsPlaceInt + 0.25f + remainderInt);
                }
                if (fractionalPart < 0.5f)
                {
                    options.Add(hundredsPlaceInt + 0.5f + remainderInt);
                }
                if (0.5 < fractionalPart && fractionalPart < 0.75f)
                {
                    options.Add(hundredsPlaceInt + 0.75f + remainderInt);
                }
                options.Add(hundredsPlaceInt + 1 + remainderInt);
            
                options.Add(50 + remainderInt);
                options.Add(100 + remainderInt);
            }
            else if (50 < hundredsPlace && hundredsPlace <= 100)
            {
                int hundredsPlaceInt = (int)Mathf.Floor(hundredsPlace);
                if (hundredsPlaceInt != 100)
                {
                    options.Add(totalOrderPrice);
                }
                if (fractionalPart < 0.25)
                {
                    options.Add(hundredsPlaceInt + 0.25f + remainderInt);
                }
                if (fractionalPart < 0.5f)
                {
                    options.Add(hundredsPlaceInt + 0.5f + remainderInt);
                }
                if (0.5 < fractionalPart && fractionalPart < 0.75f)
                {
                    options.Add(hundredsPlaceInt + 0.75f + remainderInt);
                }
                options.Add(hundredsPlaceInt + 1 + remainderInt);
                options.Add(100 + remainderInt);
            }
           

           
        }
    }
    public void GiveMoney(BagPointPOS bagPointPOS)
    {
        int selectedIndx = Random.Range(0, options.Count);
        myMoney = options[selectedIndx];
        bagPointPOS.ShowPOSFeeScreen(myMoney);
        SaveManagerInGame.Instance.myData.player_Money += myMoney;
        UIManager.Instance.UpdateMoney();
        giveMoney = true;
    }
}

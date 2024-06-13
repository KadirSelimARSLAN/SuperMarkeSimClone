using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EPOOutline;
public class InteractablePOS : IInteractable, IFuncitonalSetup
{

    private Rigidbody rb;
    [SerializeField]
    private bool isPreviewMode = false;
    public LayerMask allowedLayer;
    public Outlinable epo;
    [SerializeField]
    public bool iscollision = false;
    public bool isOpened = false;
    [SerializeField]
    private bool isReadyForInteract = true;
    [SerializeField]
    private bool isInteracted = false;
    public GameObject mySetupBox;
    [SerializeField]
    public Transform myRealParent;
    private bool isInBox;
    private bool POSMode = false;
    public Transform POSpositionCam;

    public Transform POSpositionPlayer;

    public BoxCollider boxcollid1;
    public BoxCollider boxcollid2;

    public List<AINav_Customer> myCustomers = new List<AINav_Customer>();


    //Debugging
    public POSQueueCell customerQueuePoint;
    public List<POSQueueCell> queuePointsPositions = new List<POSQueueCell>();
    public int maxCustomerCount = 12;
    public int currentCustomerCount = 0;
    private bool hitCollision = false;
    public bool haveCustomer = false;
    public int currentWaitingCustomers = 0;
    public Transform itemsParentLoc;
    public bool itemsOnPOS = false;
    public GameObject bagPoint_POS;
    public int CurrentTotalPosItemCount;

    public PreviewCollider previewCollider;
    public bool ispreviewColliderCollision = false;

    private void Start()
    {
        isInBox = false;
        rb = GetComponent<Rigidbody>();
        isInteracted = false;
        epo = GetComponent<Outlinable>();
        epo.AddAllChildRenderersToRenderingList();
        epo.RenderStyle = RenderStyle.FrontBack;

     

    }

    private void Update()
    {
        if (POSMode)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LeaveFromPOS();
            }
            if(Input.GetKeyDown(KeyCode.Space) && myCustomers[0] !=null && bagPoint_POS.GetComponent<BagPointPOS>().currentlyObtainedItems.Count == myCustomers[0].customer_Items.Count)
            {
                myCustomers[0].GiveMoney(bagPoint_POS.GetComponent<BagPointPOS>());

            }
            if (Input.GetKeyDown(KeyCode.H) && itemsOnPOS && currentCustomerCount >0 && myCustomers[0].giveMoney )
            {
                CallNextCustomer();
            }
            epo.enabled = false;
          
        }
        else
        {
            if (isPreviewMode)
            {
                ispreviewColliderCollision = previewCollider.GetCollision();

                if (iscollision || !hitCollision || ispreviewColliderCollision)
                {

                    epo.FrontParameters.FillPass.SetColor("_PublicColor", new Color(1f, 0f, 0f, 0.5f));
                    epo.FrontParameters.Color = new Color(1f, 0f, 0f, 0f);
                    epo.BackParameters.Color = new Color(1f, 0f, 0f, 0f);
                }
                else
                {
                    epo.FrontParameters.FillPass.SetColor("_PublicColor", new Color(0f, 1f, 0f, 0.5f));
                    epo.FrontParameters.Color = new Color(1f, 0f, 0f, 0f);
                    epo.BackParameters.Color = new Color(1f, 0f, 0f, 0f);
                }

            }
            else
            {
                if (isInteracted)
                {
                    epo.enabled = true;

                    epo.FrontParameters.Color = Color.green;
                    epo.BackParameters.Color = new Color(1f, 0f, 0f, 0f);
                }
                else
                {

                    epo.enabled = false;
                }

            }

        }

        //CUSTOMER QUEUE
        if (currentWaitingCustomers > 0)
        {
         
            if (currentCustomerCount > 0)
            {
                if(queuePointsPositions[queuePointsPositions.Count - 1].iamDone)
                {

                    GiveCustomerQueuePosToCustomer();
                }
            }
            else
            {
                GiveCustomerQueuePosToCustomer();
            }
            
          

        }

    }
    public override void OnFocus()
    {
        isInteracted = true;
    }

    public override void OnInteract(Transform playerHandPos)
    {

        MovePlayerView(playerHandPos);
    }

    public override void OnLoseFocus()
    {
        isInteracted = false;
    }

    public void MovePlayerView(Transform playerViewPos)
    {

        if (isReadyForInteract)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject childObject = gameObject.transform.GetChild(i).gameObject;
                if (childObject != null && childObject.GetComponent<Collider>() !=null)
                {
                    childObject.GetComponent<Collider>().isTrigger = true;
                }
            }


            isReadyForInteract = false;
            transform.parent = playerViewPos;



            boxcollid1.isTrigger = true;
            boxcollid2.enabled = false;
            isInteracted = true;



            isInteracted = false;
            transform.DORotate((transform.rotation.eulerAngles), 0.2f).SetEase(Ease.OutQuad);
            isReadyForInteract = true;
        


        }


    }

    public void PreviewThisInteract()
    {
        previewCollider.gameObject.SetActive(true);
        RemoveAllCustomerToList(); 
        epo.enabled = true;
        epo.FrontParameters.Color = new Color(1f, 0f, 0f, 0f);
        epo.FrontParameters.FillPass.Shader = Resources.Load<Shader>("Easy performant outline/Shaders/Fills/ColorFill");
       
        isPreviewMode = true;
       


    }

    public void PlaceandDropThisInteract()
    {
        previewCollider.gameObject.SetActive(false);
        isReadyForInteract = true;
        boxcollid1.isTrigger = false;
        boxcollid2.enabled = true;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childObject = gameObject.transform.GetChild(i).gameObject;
            if (childObject != null && childObject.GetComponent<Collider>() !=null)
            {
                childObject.GetComponent<Collider>().isTrigger = false;
            }
        }
        epo.FrontParameters.FillPass.SetColor("_PublicColor", new Color(0f, 1f, 0f, 0f));
        epo.enabled = false;
        isPreviewMode = false;
        mySetupBox.transform.position = transform.position;
        transform.parent = myRealParent;
     
      // TODO: POS SavePcandDesk();



    }

    public void GetBackPreviewThisInteract()
    {
        throw new System.NotImplementedException();
    }

    public void MakeBox()
    {
        transform.parent = myRealParent;
        mySetupBox.gameObject.SetActive(true);
        FirstPersonController.Instance.currentHandTool = mySetupBox;
        mySetupBox.gameObject.transform.position = FirstPersonController.Instance.playerHandPos.transform.position;
        mySetupBox.gameObject.GetComponent<IFunctionalBox>().MovePlayerHand(FirstPersonController.Instance.playerHandPos);
        FirstPersonController.Instance.previewMode = false;
        gameObject.SetActive(false);
    }
    public void InteractWithPOS()
    {

        Debug.Log("POS MODE");
        POSMode = true;
        
        FirstPersonController.Instance.canMove = false;
        FirstPersonController.Instance.posMode = true;
        // PCGUIManager.Instance.IsPcMode = true;
      //   CameraManager.Instance.MovePosition(POSpositionPlayer);
        
        FirstPersonController.Instance.GoPOSDesk(POSpositionPlayer);
       

        isReadyForInteract = false;

    }

    public void LeaveFromPOS()
    {
        POSMode = false;

        UIManager.Instance.VisibleDefaultCursor(true);
        //   PCGUIManager.Instance.IsPcMode = false;
        //   PCGUIManager.Instance.ClearAllChangedOrderTexts();
        //  CameraManager.Instance.MoveDefaultPos();
        FirstPersonController.Instance.canMove = true;
        FirstPersonController.Instance.posMode = false;
        isReadyForInteract = true;

    }
    public bool IsCollision()
    {
        return iscollision;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPreviewMode)
        {
            // Temas eden nesnenin Layer'ı, izin verilen Layer'lar arasında mı kontrol et
            if (allowedLayer == (allowedLayer | (1 << other.gameObject.layer)))
            {
                iscollision = true;


            }
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (isPreviewMode)
        {// Temas eden nesnenin Layer'ı, izin verilen Layer'lar arasında mı kontrol et
            if (allowedLayer == (allowedLayer | (1 << other.gameObject.layer)))
            {
                iscollision = true;

            }
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (isPreviewMode)
        {
            // Temas eden nesnenin Layer'ı, izin verilen Layer'lar arasında mı kontrol et
            if (allowedLayer == (allowedLayer | (1 << other.gameObject.layer)))
            {
                iscollision = false;



            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isPreviewMode)
        {
            // Temas eden nesnenin Layer'ı, izin verilen Layer'lar arasında mı kontrol et
            if (allowedLayer == (allowedLayer | (1 << collision.gameObject.layer)))
            {
                iscollision = true;



            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (isPreviewMode)
        {
            // Temas eden nesnenin Layer'ı, izin verilen Layer'lar arasında mı kontrol et
            if (allowedLayer == (allowedLayer | (1 << collision.gameObject.layer)))
            {
                iscollision = true;



            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (isPreviewMode)
        {
            // Temas eden nesnenin Layer'ı, izin verilen Layer'lar arasında mı kontrol et
            if (allowedLayer == (allowedLayer | (1 << collision.gameObject.layer)))
            {
                iscollision = true;



            }
        }
    }

    public override bool readyForInteract()
    {
        return isReadyForInteract;
    }

    public void GetCollision(bool X)
    {
        hitCollision = X;
    }


    public void SavePcandDesk()
    {
        SaveGameManager.CurrentSaveData.pcAndDeskPosition = gameObject.transform.position;
        SaveGameManager.CurrentSaveData.pcAndDeskRotation = gameObject.transform.rotation;

    }


    public void LoadPcandDesk()
    {
        gameObject.transform.position = SaveGameManager.CurrentSaveData.pcAndDeskPosition;
        gameObject.transform.rotation = SaveGameManager.CurrentSaveData.pcAndDeskRotation;

    }





    ///POS FUNCS///

    public void AddCustomerToList(AINav_Customer newCustomer)
    {
     myCustomers.Add(newCustomer);
     currentWaitingCustomers++;
    
    }
    public void GiveCustomerQueuePosToCustomer()
    {
      
            if(queuePointsPositions.Count < 1)
            {
                var firstcustomerQuePos = Instantiate(customerQueuePoint, transform.position , transform.rotation);
                firstcustomerQuePos.transform.parent = this.gameObject.transform;
     
                firstcustomerQuePos.SetPosCellPos(transform.position , 0);
                 queuePointsPositions.Add(firstcustomerQuePos);
                currentCustomerCount++;
            myCustomers[0].amICurrentCustomer = true;

        }
            else
        {
            var customerQuePos = Instantiate(customerQueuePoint, queuePointsPositions[queuePointsPositions.Count - 1].transform.position, transform.rotation);
            customerQuePos.transform.parent = this.gameObject.transform;
            customerQuePos.transform.position = transform.position + transform.forward * queuePointsPositions.Count;
            customerQuePos.SetPosCellPos(queuePointsPositions[queuePointsPositions.Count - 1].transform.position, queuePointsPositions.Count);
            queuePointsPositions.Add(customerQuePos);
            currentCustomerCount++;
        }
        currentWaitingCustomers--;



    }
    public void RemoveAllCustomerToList()
    {
        for(int i = 0; i < queuePointsPositions.Count; i++)
        {
          Destroy(queuePointsPositions[i].gameObject);
        }
        queuePointsPositions.Clear();
        currentCustomerCount = 0;
    }
    
    public void CallNextCustomer()
    {
        bagPoint_POS.GetComponent<BagPointPOS>().ClearAllItemList();

        for (int i = 0; i < itemsParentLoc.childCount; i++)
        {

            itemsParentLoc.GetChild(i).transform.GetComponent<GameObjectChildController>().InVisibleItem();

        }
        myCustomers[0].amICurrentCustomer = false;
        myCustomers[0].GoExit();
        myCustomers.RemoveAt(0);
        Destroy(queuePointsPositions[queuePointsPositions.Count - 1].gameObject);
        queuePointsPositions.RemoveAt(queuePointsPositions.Count - 1);
        itemsOnPOS = false;
        for (int i = 0; i < myCustomers.Count; i++)
        {
            queuePointsPositions[i].CallMyCustomer();
        }
        bagPoint_POS.gameObject.SetActive(false);
        currentCustomerCount--;
        if (currentCustomerCount < 1)
        {
            LeaveFromPOS();
        }
        if(myCustomers.Count >0)
        myCustomers[0].amICurrentCustomer = true;
    }

    public void AddCustomerItemsOnPOS(List<int> items)
    {
        CurrentTotalPosItemCount = items.Count;
        for (int i = 0; i < items.Count; i++)
        {
          itemsParentLoc.GetChild(i).transform.GetComponent<GameObjectChildController>().VisibleItem(items[i]);
        }
        itemsOnPOS = true;
        bagPoint_POS.gameObject.SetActive(true);
    }

}

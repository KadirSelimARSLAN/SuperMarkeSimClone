using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EPOOutline;
public class InteractablePCandDesk : IInteractable, IFuncitonalSetup
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
    private bool pcMode = false;
    public Transform pcPosCam;

    public Transform pcPosPlayer;

    public BoxCollider boxcollid1;
    public BoxCollider boxcollid2;


    private bool hitCollision = false;

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
        if (pcMode)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LeaveFromPC();
            }

            epo.enabled = false;

        }
        else
        {
            if (isPreviewMode)
            {

                if (iscollision || !hitCollision)
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
                if (childObject != null)
                {
                    childObject.GetComponent<Collider>().isTrigger = true;
                }
            }
          

            isReadyForInteract = false;
            transform.parent = playerViewPos;


            /* transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
             {

                 isReadyForInteract = true;


             });
            */
         
            boxcollid1.isTrigger = true;
            boxcollid2.enabled = false;
            isInteracted = true;



            isInteracted = false;
            transform.DORotate((transform.rotation.eulerAngles), 0.2f).SetEase(Ease.OutQuad);
            isReadyForInteract = true;
            //  PreviewThisInteract();


        }




    }

    public void PreviewThisInteract()
    {
        epo.enabled = true;
        epo.FrontParameters.Color = new Color(1f, 0f, 0f, 0f);
        epo.FrontParameters.FillPass.Shader = Resources.Load<Shader>("Easy performant outline/Shaders/Fills/ColorFill");
        // epo.FrontParameters.FillPass.SetColor("_PublicColor", new Color(0f, 1f, 0f, 0.5f));
        isPreviewMode = true;



    }

    public void PlaceandDropThisInteract()
    {
        isReadyForInteract = true;
        boxcollid1.isTrigger = false;
        boxcollid2.enabled = true;
      
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childObject = gameObject.transform.GetChild(i).gameObject;
            if (childObject != null)
            {
                childObject.GetComponent<Collider>().isTrigger = false;
            }
        }
        epo.FrontParameters.FillPass.SetColor("_PublicColor", new Color(0f, 1f, 0f, 0f));
        epo.enabled = false;
        isPreviewMode = false;
        mySetupBox.transform.position = transform.position;
        transform.parent = myRealParent;
        SavePcandDesk();



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
    public void InteractWithPC()
    {

        Debug.Log("PC MODE");
        pcMode = true;
        UIManager.Instance.VisibleDefaultCursor(false);
        FirstPersonController.Instance.canMove = false;
        FirstPersonController.Instance.pcMode = true;
        PCGUIManager.Instance.IsPcMode = true;
        CameraManager.Instance.MovePosition(pcPosCam);
        FirstPersonController.Instance.GoPCDesk(pcPosPlayer);
        Cursor.lockState = CursorLockMode.None;
         Cursor.visible = false;
       
        isReadyForInteract = false;

      



    }

    public void LeaveFromPC()
    {
        pcMode = false;

        UIManager.Instance.VisibleDefaultCursor(true);
        PCGUIManager.Instance.IsPcMode = false;
        PCGUIManager.Instance.ClearAllChangedOrderTexts();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CameraManager.Instance.MoveDefaultPos();
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
}



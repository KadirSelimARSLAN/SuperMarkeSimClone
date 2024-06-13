using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EPOOutline;
public class InteractableShelf : IInteractable ,IFuncitonalSetup
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
    private  bool isReadyForInteract = true;
    [SerializeField]
    private bool isInteracted = false;
    public GameObject mySetupBox;
    [SerializeField]
    public Transform myRealParent;
    public GameObject[] myShelfParts;
    public bool isInteractWithNPC = false;

    private bool hitCollision=false;


    //preview mode collider
    public PreviewCollider previewCollider;
    public bool ispreviewColliderCollision = false;
    
    private void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        isInteracted = false;
        epo = GetComponent<Outlinable>();
       // epo.AddAllChildRenderersToRenderingList();
        epo.RenderStyle = RenderStyle.FrontBack;
       
    }

    private void Update()
    {
        if (isPreviewMode)
        {
            ispreviewColliderCollision =  previewCollider.GetCollision();


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
    public override void OnFocus()
    {
        isInteracted = true;
    }

    public override void OnInteract(Transform playerHandPos )
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


          
            if (this.gameObject.GetComponent<Collider>())
            {
                this.gameObject.GetComponent<Collider>().isTrigger = true;
            }
           
            isInteracted = true;



            isInteracted = false;
            transform.DORotate((transform.rotation.eulerAngles), 0.2f).SetEase(Ease.OutQuad);
            isReadyForInteract = true;
          


        }




    }

    public void PreviewThisInteract()
    {
        epo.enabled = true;
        epo.FrontParameters.Color = new Color(1f, 0f, 0f, 0f);
        epo.FrontParameters.FillPass.Shader = Resources.Load<Shader>("Easy performant outline/Shaders/Fills/ColorFill");
      
        isPreviewMode = true;

        previewCollider.gameObject.SetActive(true);
        for (int i = 0; i < myShelfParts.Length; i++)
        {
            myShelfParts[i].GetComponent<InteractableShelfPart>()?.UpdatePreviewMode(true);
        }
   


    }

    public void PlaceandDropThisInteract()
    {
        for (int i = 0; i < myShelfParts.Length; i++)
        {
            myShelfParts[i].GetComponent<InteractableShelfPart>()?.UpdatePreviewMode(false);
        }
        isReadyForInteract = true;
         if (this.gameObject.GetComponent<Collider>())
        {
            this.gameObject.GetComponent<Collider>().isTrigger = false;
        }
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
        previewCollider.gameObject.SetActive(false);
        transform.parent = myRealParent;
    }

    public void GetBackPreviewThisInteract()
    {
        throw new System.NotImplementedException();
    }

    public void MakeBox()
    {
        myRealParent.GetComponent<ShelfParent>().boxMode = true;
        transform.parent = myRealParent;
        mySetupBox.gameObject.SetActive(true);  
        FirstPersonController.Instance.currentHandTool = mySetupBox;            
        mySetupBox.gameObject.transform.position = FirstPersonController.Instance.playerHandPos.transform.position;
        mySetupBox.gameObject.GetComponent<IFunctionalBox>().MovePlayerHand(FirstPersonController.Instance.playerHandPos);
        FirstPersonController.Instance.previewMode = false;
        gameObject.SetActive(false);
    }

    public bool IsCollision()
    {
        return iscollision;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPreviewMode)
        {
            
            if (allowedLayer == (allowedLayer | (1 << other.gameObject.layer)))
            {
                iscollision = true;


            }
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (isPreviewMode)
        {
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

    public void UpdateEpoChild(Renderer renderer)
    {
       
        epo.TryAddTarget(new OutlineTarget(renderer));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EPOOutline;
public class InteractableSetupBox : IInteractable,IFunctionalBox
{
    public Rigidbody rb;
    private bool isPreviewMode = false;
    public LayerMask allowedLayer;
    public Outlinable epo;
    public bool iscollision = false;
    public bool isOpened = false;
    private bool isReadyForInteract = true;
    public GameObject mySetupObject;
    [SerializeField]
    public Transform myRealParent;
    private bool isInteracted = false;
    private void Start()
    {
       
        isReadyForInteract = true;
       
        isInteracted = false;
        epo = gameObject.GetComponent<Outlinable>();
        epo.AddAllChildRenderersToRenderingList();
        epo.RenderStyle = RenderStyle.FrontBack;
    }

    public override void OnFocus()
    {

        isInteracted = true;

    }

    public override void OnInteract(Transform playerHandPos)
    {

        MovePlayerHand(playerHandPos);

    }

    public override void OnLoseFocus()
    {
        isInteracted = false;


    }

    private void Update()
    {
        if (isPreviewMode)
        {
            if (iscollision)
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

    public void MovePlayerHand(Transform playerHandPos)
    {

        if (isReadyForInteract == true)
        {
            rb.isKinematic = true;
            isReadyForInteract = false;
            transform.parent= playerHandPos;
            transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutQuad).OnComplete(() =>
            {

            });
            transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
            {

                isReadyForInteract = true;


            });

            isInteracted = true;

            if (this.gameObject.GetComponent<Collider>())
            {
                this.gameObject.GetComponent<Collider>().isTrigger = true;
            }
            isInteracted = false;

        }

    }

    public void ThrowThisInteract(Transform playerHandPos)
    {
        rb.isKinematic = false;
        transform.parent = myRealParent;

        isReadyForInteract = true;

        if (this.gameObject.GetComponent<Collider>())
        {
            this.gameObject.GetComponent<Collider>().isTrigger = false;
        }

        rb.AddForce(rb.mass * 300 * playerHandPos.forward);

    }

    public void PreviewThisInteract()
    {

        epo.enabled = true;

        epo.FrontParameters.Color = new Color(1f, 0f, 0f, 0f);
        epo.FrontParameters.FillPass.Shader = Resources.Load<Shader>("Easy performant outline/Shaders/Fills/ColorFill");
        epo.FrontParameters.FillPass.SetColor("_PublicColor", new Color(0f, 1f, 0f, 0.5f));
        isPreviewMode = true;
    }


    public void PlaceandDropThisInteract()
    {
        isReadyForInteract = true;
        transform.parent = myRealParent;

        rb.isKinematic = false;
        if (this.gameObject.GetComponent<Collider>())
        {
            this.gameObject.GetComponent<Collider>().isTrigger = false;
        }

        epo.FrontParameters.FillPass.SetColor("_PublicColor", new Color(0f, 1f, 0f, 0f));
        epo.enabled = false;
        isPreviewMode = false;
    }

    public void GetBackPreviewThisInteract()
    {

        epo.FrontParameters.FillPass.SetColor("_PublicColor", new Color(0f, 1f, 0f, 0f));
        epo.enabled = false;
        isPreviewMode = false;
        transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutQuad);
        transform.DOLocalRotate(Vector3.zero, 0.3f).SetEase(Ease.OutQuad);
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

    public void OpenCloseBox()
    {
        if (myRealParent.GetComponent<ShelfParent>() != null)
        {
            myRealParent.GetComponent<ShelfParent>().boxMode = false;
        }
      
        FirstPersonController.Instance.previewMode = true;
        transform.parent = myRealParent;
        mySetupObject.gameObject.SetActive(true);
        FirstPersonController.Instance.currentHandTool = mySetupObject;       
        mySetupObject.gameObject.transform.position = FirstPersonController.Instance.playerViewPos.transform.position;
        mySetupObject.gameObject.GetComponent<IFuncitonalSetup>().MovePlayerView(FirstPersonController.Instance.playerViewPos);
        gameObject.SetActive(false);



    }

    public bool IsCollision()
    {
        return iscollision;

    }

    public bool IsOpen()
    {
        return isOpened;
    }

    public override bool readyForInteract()
    {
        return isReadyForInteract;
    }
}

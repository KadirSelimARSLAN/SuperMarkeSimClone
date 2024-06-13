using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EPOOutline;
public class InteractableItemBox : IInteractable, IFunctionalBox
{
    public int itemBoxIndx;
    private Rigidbody rb;
    private bool isPreviewMode = false;
    public LayerMask allowedLayer;
    public Outlinable epo;
    public bool iscollision = false;
    public bool isOpened = false;
    public bool isReadyForInteract = true;
    [SerializeField]
    private string itemName;
    public int generalItemIndex;
    public string itemType;
    [SerializeField]
    public int ItemCount;
    [SerializeField]
    private int maxItemCount;

    
    public List<InteractableItem> myItems = new List<InteractableItem>();
    private bool isInteracted = false;

  
    public string ItemName { get => itemName; set => itemName = value; }
    public int MaxItemCount { get => maxItemCount; set => maxItemCount = value; }

    private void Start()
    {
        maxItemCount = transform.childCount;

        for (int i = 0; i < ItemCount; i++)
         {
           
            // Orijinal objenin scale değerlerini saklayın
            Vector3 originalScale = InstantiateManager.Instance.items_all[generalItemIndex].transform.localScale;

                // Clone'u oluşturun
                GameObject itemClone = Instantiate(InstantiateManager.Instance.items_all[generalItemIndex], transform.GetChild(i).transform.position, transform.GetChild(i).transform.rotation);

                // Clone'un scale değerini orijinal scale değerleriyle ayarlayın
                itemClone.transform.localScale = originalScale;

            // Clone'u parent objeye bağlayın
            itemClone.transform.parent = transform.GetChild(i).transform;


            myItems.Add(itemClone.GetComponent<InteractableItem>());
            //            myItems.Add(transform.GetChild(i).transform.GetChild(0).GetComponent<InteractableItem>());

            itemName = transform.GetChild(i).transform.GetChild(0).GetComponent<InteractableItem>().itemName;
            itemType = transform.GetChild(i).transform.GetChild(0).GetComponent<InteractableItem>().itemType;
            



            for (int j = 0; j < myItems.Count; j++)
            {

                myItems[j].transform.GetComponent<InteractableItem>().getInteracted(false);
            }

        }



        isReadyForInteract = true;
        rb = GetComponent<Rigidbody>();
        isInteracted = false;
        epo = gameObject.GetComponent<Outlinable>();     
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
    public void MoveStorageShelfPart(Transform shelfPart)
    {
        if (isReadyForInteract == true)
        {
            FirstPersonController.Instance.currentHandTool = null;
            DOTween.Kill(transform);

            if (this.gameObject.GetComponent<Collider>())
            {
                this.gameObject.GetComponent<Collider>().enabled = false;
            }
            
            transform.parent = null;
            rb.isKinematic = true;
            isReadyForInteract = false;
         
            transform.DOLocalRotate(shelfPart.rotation.eulerAngles, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {

            });
            transform.DOLocalMove(shelfPart.position, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                DOTween.Kill(transform);
                transform.parent = shelfPart.transform;

                if (shelfPart.parent.parent.GetComponent<InteractableStorageShelfPart>() != null)
                {
                    if (FirstPersonController.Instance.currentInteractable == shelfPart.parent.parent.GetComponent<InteractableStorageShelfPart>())
                    {
                        shelfPart.parent.parent.GetComponent<InteractableStorageShelfPart>().OnFocus();

                    }


                }
                if (transform.position != Vector3.zero || transform.rotation.eulerAngles != Vector3.zero)
                {
                    //  transform.DOLocalJump(Vector3.zero, 0.1f,1/2,1f);
                    transform.DOLocalMove(Vector3.zero, 0.1f);
                    transform.DOLocalRotate(Vector3.zero, 0.1f);
                }
                isReadyForInteract = true;


            });

            isInteracted = true;

          
            isInteracted = false;

        }
    }
    public void getInteracted(bool interact)
    {
        isInteracted = interact;

    }
    public void MovePlayerHand(Transform playerHandPos)
    {
        if (isReadyForInteract == true)
        {
            DOTween.Kill(transform);
            this.gameObject.GetComponent<Collider>().enabled = true;
            rb.isKinematic = true;
            isReadyForInteract = false;
            transform.parent = playerHandPos;
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
        /*   if (isReadyForInteract == true )
           {
               //DOTween.Kill(transform);
               rb.isKinematic = true;
               isReadyForInteract = false;
               transform.parent = null;
               transform.DOLocalRotate(playerHandPos.transform.rotation.eulerAngles, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
               {

               });
               transform.DOLocalMove(playerHandPos.transform.position, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
               {

                   isReadyForInteract = true;
                   transform.parent = playerHandPos;

                   if (transform.position != Vector3.zero || transform.rotation.eulerAngles != Vector3.zero)
                   {
                       //  transform.DOLocalJump(Vector3.zero, 0.1f,1/2,1f);
                       transform.DOLocalMove(Vector3.zero, 0.1f);
                       transform.DOLocalRotate(Vector3.zero, 0.1f);
                   }

               });

               isInteracted = true;

               if (this.gameObject.GetComponent<Collider>())
               {
                   this.gameObject.GetComponent<Collider>().isTrigger = true;
               }
               isInteracted = false;

           }*/

    }

    public void ThrowThisInteract(Transform playerHandPos)
    {
        rb.isKinematic = false;
        transform.parent = InstantiateManager.Instance.itemBoxParent;

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
        transform.parent = InstantiateManager.Instance.itemBoxParent;

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
        if (isOpened)
        {
            isOpened = false;
            Debug.Log("Kutu Kapandı");
        }
        else
        {

            isOpened = true;
            Debug.Log("Kutu Açıldı");
        }
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

    public void moveItemtoShelfPart()
    {
        myItems.RemoveAt(--ItemCount);
                


    }

    public void AddItems(InteractableItem item)
    {
      
        myItems.Add(item);
                
        itemName = item.itemName;
        itemType = item.itemType;
        ItemCount++;
                                    
       
            
                for (int i = 0; i < myItems.Count; i++)
                {
   
                    myItems[i].transform.GetComponent<InteractableItem>().getInteracted(false);
                }

            
        
    }
}

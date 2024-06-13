using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using EPOOutline;
public class InteractableItem : IInteractable
{
    public int generalItemIndx;
    public string itemType;
    public string itemName;
    public Outlinable epo;
    public bool isInteracted = false;
    private bool itemUsed = false;
    private void Start()
    {
        epo = gameObject.GetComponent<Outlinable>();
        epo.AddAllChildRenderersToRenderingList();
        epo.RenderStyle = RenderStyle.FrontBack;
    }
    private void Update()
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
    public override void OnInteract(Transform playerHandPos)
    {
        if (!itemUsed)
        {
            MoveToBagPoint(playerHandPos);
        }
   
    }  

    public override void OnFocus()
    {
     
        if (FirstPersonController.Instance.posMode == true)
        {
          
            isInteracted = true;
        }
    }

    public override void OnLoseFocus()
    {
        
          
            isInteracted = false;
        
       
    }
    public void getInteracted(bool interact)
    {
        isInteracted = interact;

    }
    public override bool readyForInteract()
    {
        throw new NotImplementedException();
    }
    public void MoveToShelfPartPoint(Transform pointParent)
    {
       
        isInteracted = true;
        DOTween.Kill(transform);
        transform.parent = null;
        // Nesnenin global ölçeğini kaydet
        // pointParent ile obje arasındaki mesafenin 3/1'ini al
        Vector3 closeJumpPos = Vector3.Lerp(transform.position, pointParent.position, 1f / 3f);

        closeJumpPos = closeJumpPos + new Vector3(0, 0.5f, 0);

        transform.DOLocalRotate(pointParent.transform.eulerAngles, 0.15f).SetEase(Ease.OutQuad).OnComplete(() =>
        {

            // İşlemler buraya
        });

        transform.DOLocalMove(closeJumpPos, 0.15f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            DOTween.Kill(transform);
            transform.parent = pointParent.transform;

            
                isInteracted = false;
            if(pointParent.parent?.parent?.GetComponent<InteractableShelfPart>() != null )
            {
                if ( FirstPersonController.Instance.currentInteractable == pointParent.parent.parent.GetComponent<InteractableShelfPart>())
                {
                    pointParent.parent.parent.GetComponent<InteractableShelfPart>().OnFocus();

                }
             

            }
         

            if (transform.position != Vector3.zero || transform.rotation.eulerAngles != Vector3.zero)
            {
                //  transform.DOLocalJump(Vector3.zero, 0.1f,1/2,1f);
               
                transform.DOLocalRotate(Vector3.zero, 0.1f);
                transform.DOLocalMove(Vector3.zero, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    if (pointParent.transform.tag == "CustomerBag")
                    {
                        Destroy(this.gameObject);
                    }
                });

            }
            else
            {
                if (pointParent.transform.tag == "CustomerBag")
                {
                    Destroy(this.gameObject);
                }
            }

          
            // Nesnenin global ölçeğini tekrar uygula


        });

        DOTween.Sequence()
            .AppendInterval(0.45f) // Rotate animasyonunun süresi
            .OnComplete(() =>
            {

            });

    }

    public void MoveToBagPoint(Transform pointParent)
    {
        itemUsed = true;
        isInteracted = true;
        DOTween.Kill(transform);
        pointParent.GetComponent<BagPointPOS>().AddItemToList(generalItemIndx);
        
        transform.DOMove(pointParent.position, 0.8f).SetEase(Ease.OutQuad).OnComplete(() =>
        {

            //  transform.parent = pointParent.transform;
            
            this.gameObject.SetActive(false);
            isInteracted = false;
           
            transform.position = transform.parent.position;
            DOTween.Kill(transform);
            itemUsed = false;
        });

        DOTween.Sequence()
            .AppendInterval(0.45f) // Rotate animasyonunun süresi
            .OnComplete(() =>
            {

            });

    }


    /* public void MoveToShelfPartPoint(Transform pointParent)
    {

        

        transform.parent = null;
        // Nesnenin global ölçeğini kaydet

        transform.DOLocalRotate(pointParent.transform.eulerAngles, 0.15f).SetEase(Ease.OutQuad).OnComplete(() =>
        {

            // İşlemler buraya
        });



        transform.DOLocalJump(pointParent.transform.position, 0.2f, 1/2, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            DOTween.Kill(transform);
            transform.parent = pointParent.transform;

            if(transform.position != Vector3.zero || transform.rotation.eulerAngles !=Vector3.zero)
            {
              //  transform.DOLocalJump(Vector3.zero, 0.1f,1/2,1f);
                transform.DOLocalMove(Vector3.zero,0.1f);
                transform.DOLocalRotate(Vector3.zero, 0.1f);
            }
            

            // Nesnenin global ölçeğini tekrar uygula


        });

        DOTween.Sequence()
            .AppendInterval(0.45f) // Rotate animasyonunun süresi
            .OnComplete(() =>
            {

            });

    }*/
    /* public void MoveToShelfPartPoint(Transform pointParent)
     {

         FirstPersonController.Instance.canMove = false;

         transform.parent = null;
         // Nesnenin global ölçeğini kaydet

         transform.DOLocalRotate(pointParent.transform.eulerAngles, 0.15f).SetEase(Ease.OutQuad).OnComplete(() =>
         {

             // İşlemler buraya
         });



         transform.DOLocalJump(pointParent.transform.position, 0.5f, 1, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
         {
             DOTween.Kill(transform);
             transform.parent = pointParent.transform;
             FirstPersonController.Instance.canMove = true;

             // Nesnenin global ölçeğini tekrar uygula


         });

         DOTween.Sequence()
             .AppendInterval(0.45f) // Rotate animasyonunun süresi
             .OnComplete(() =>
             {

             });

     }*/



}

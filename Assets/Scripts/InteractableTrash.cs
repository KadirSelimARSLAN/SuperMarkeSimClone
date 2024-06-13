using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EPOOutline;

public class InteractableTrash : IInteractable
{
    public Outlinable epo;
    private bool isInteracted = false;
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
    public override void OnFocus()
    {
       // if (FirstPersonController.Instance.currentHandTool !=null)
     //   {
       //     if(FirstPersonController.Instance.currentHandTool.GetComponent<IFunctionalBox>() != null)
            isInteracted = true;
     //   }
      
    }

    public override void OnInteract(Transform playerHandPos)
    {
       
        if (FirstPersonController.Instance.currentHandTool != null && FirstPersonController.Instance.currentHandTool.GetComponent<IFunctionalBox>() != null && !FirstPersonController.Instance.currentHandTool.CompareTag("PCBox"))
        {
            for (int i = 0; i < playerHandPos.childCount; i++)
            {
                GameObject childObject = playerHandPos.GetChild(i).gameObject;
                if (childObject != null)
                {
                    if (playerHandPos.GetChild(i).gameObject.GetComponent<InteractableSetupBox>())

                    {
                        GameObject parentObject = FirstPersonController.Instance.currentHandTool.GetComponent<InteractableSetupBox>().myRealParent.gameObject;
                        Destroy(parentObject);
                       
                    }
                    Destroy(childObject);
                }
            }
            FirstPersonController.Instance.currentHandTool = null;
        }

    }

    public override void OnLoseFocus()
    {
        isInteracted = false;
    }

    public override bool readyForInteract()
    {
        return true;
    }
}

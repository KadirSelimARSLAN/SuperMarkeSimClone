using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPOOutline;

public class InteractableMoney : IInteractable
{
    public bool isInteracted = false;
    public Outlinable epo;
    public float myMoneyValue;
    public BagPointPOS bagPoint;
  
    private void Start()
    {
        epo = gameObject.GetComponent<Outlinable>();
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
        if (FirstPersonController.Instance.posMode == true)
        {

            isInteracted = true;
        }
    }

    public override void OnInteract(Transform playerHandPos)
    {
        bagPoint.ChangeGivenMoneyText(myMoneyValue);

    }

    public override void OnLoseFocus()
    {
       

            isInteracted = false;
        
    }

    public override bool readyForInteract()
    {
        return true;
    }
   public void CloneMoney()
    {


    }
}

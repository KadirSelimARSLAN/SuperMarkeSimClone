using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POSQueueCell : MonoBehaviour
{
    public bool hitCollider = false;
    private float distanceCount = 1f;
    public int trySetPosCount = 0;
    public int cellPosIndx;
    public bool setPos = true;
    public bool iamDone = false;

    
    private void OnTriggerEnter(Collider other)
    {
        
            if (other.gameObject.tag == "Functional" || other.gameObject.tag == "PCBox" || other.gameObject.tag == "CellPOS")
            {
                hitCollider = true;

            }
        
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Functional" || other.gameObject.tag == "PCBox" || other.gameObject.tag == "CellPOS")
        {
            hitCollider = true;

        }
       
    }
    private void OnTriggerExit(Collider other)
    {
        
            if (other.gameObject.tag == "Functional" || other.gameObject.tag == "PCBox" || other.gameObject.tag == "CellPOS")
            {
                hitCollider = false;

            }
        
      
    }
    private void Update()
    {
     
      /*  if (hitCollider )
        {
            iamDone = false;
            SetPosCellPos(transform.position, cellPosIndx);
            setPos = true;
           
        }
        else*/ if (hitCollider == false && setPos==false && iamDone == false)
        {
            CallMyCustomer();
        }
    }

    public void SetPosCellPos(Vector3 lastCellPos, int cellPos_Indx)
    {
        cellPosIndx = cellPos_Indx;
        gameObject.transform.position = lastCellPos + transform.forward * distanceCount;
        trySetPosCount++;

        
            StartCoroutine(CallSetPosCoroutine(0.2f)); // 1 saniye sonra CallSetPosCoroutine başlat
        
        
    }

    private IEnumerator CallSetPosCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay); // Belirtilen süreyi bekle
        if (hitCollider)
        {
            SetPosCellPos(transform.position, cellPosIndx);
            
        }
        else
        {
            setPos = false;

        }
      
    }

   

    public void CallMyCustomer()
    {
        iamDone = true;
        gameObject.GetComponentInParent<InteractablePOS>().myCustomers[cellPosIndx]?.GoPOSQueue(transform, cellPosIndx);
       // gameObject.GetComponentInParent<InteractablePOS>().GiveCustomerQueuePosToCustomer();
        gameObject.GetComponent<SphereCollider>().enabled = false;
        gameObject.GetComponent<POSQueueCell>().enabled = false;

    }
}

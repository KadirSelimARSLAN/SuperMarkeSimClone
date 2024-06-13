using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCollider : MonoBehaviour
{
    public bool iscollision;

    private void OnTriggerEnter(Collider other)
    {

        iscollision = true;

    }
    private void OnTriggerStay(Collider other)
    {
              iscollision = true;

    }
    private void OnTriggerExit(Collider other)
    {
      
              iscollision = false;

    }
    private void OnCollisionEnter(Collision collision)
    {
             iscollision = true;
    }
    private void OnCollisionStay(Collision collision)
    {
    
                iscollision = true;
    }
    private void OnCollisionExit(Collision collision)
    {

        iscollision = false;

    }

    public bool GetCollision()
    {
        return iscollision;
    }
}

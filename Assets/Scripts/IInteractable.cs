using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInteractable : MonoBehaviour
{
    public virtual void Awake()
    {
     //   gameObject.layer = 6;
    }
    public abstract void OnInteract(Transform playerHandPos);
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
    public abstract bool readyForInteract();
    
 
}

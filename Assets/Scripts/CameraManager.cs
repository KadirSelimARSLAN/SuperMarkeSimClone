using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField]
    private Vector3 myDefaultPos;
    [SerializeField]
    private Vector3 myDefaultRotation;
   
    private Transform pcPos;


    public void MovePosition(Transform newPos)
    {
        transform.parent = null;
        pcPos = newPos;
        transform.DOLocalMove(newPos.position, 0.6f);
        transform.DOLocalRotate(newPos.rotation.eulerAngles, 0.6f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            DOTween.Kill(transform);
        });
    }
    public void MoveJustRotation(Transform newPos)
    {
        transform.parent = null;
        pcPos = newPos;
    
        transform.DOLocalRotate(newPos.rotation.eulerAngles, 0.6f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            DOTween.Kill(transform);
        });
    }

    public void MoveDefaultPos()
    {
        transform.parent = FirstPersonController.Instance.gameObject.transform;
        
        transform.DOLocalMove(myDefaultPos, 0.4f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            FirstPersonController.Instance.canMove = true;
          

        });
       
        //   transform.rotation = transform.parent.rotation;

       
    }

}

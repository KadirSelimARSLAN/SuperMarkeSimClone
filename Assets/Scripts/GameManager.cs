using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public void SetAllPriceTag(int indx)
    {
        foreach (Transform furnitureChildTransform in InstantiateManager.Instance.furnitureParent.transform)
        {

            ShelfParent interactableShelfParent = furnitureChildTransform.gameObject.GetComponent<ShelfParent>();



            for (int i = 0; i < interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts.Length; i++)
            {


                if (indx == interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myItemGeneralIndx)
                {

                    interactableShelfParent.ShelfChild.GetComponent<InteractableShelf>().myShelfParts[i].GetComponent<InteractableShelfPart>().myPriceTag.SetPriceTagValues(indx);

                }

            }






        }
    }
}

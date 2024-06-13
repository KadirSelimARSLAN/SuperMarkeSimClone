using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnituresParent : Singleton<FurnituresParent>
{
    public List<InteractableShelfPart> itemsShelfParts = new List<InteractableShelfPart>();
}

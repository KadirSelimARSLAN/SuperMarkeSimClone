using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFuncitonalSetup 
{
    void MovePlayerView(Transform X);
    
    void PreviewThisInteract();
    void PlaceandDropThisInteract();
    void GetBackPreviewThisInteract();  
    void MakeBox();
    bool IsCollision();
    void GetCollision(bool X);
}

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

public interface IFunctionalBox
{

    void MovePlayerHand(Transform X);
    void ThrowThisInteract(Transform X);
    void PreviewThisInteract();
    void PlaceandDropThisInteract();
    void GetBackPreviewThisInteract();
    void OpenCloseBox();
    bool IsCollision();
    bool IsOpen();
}

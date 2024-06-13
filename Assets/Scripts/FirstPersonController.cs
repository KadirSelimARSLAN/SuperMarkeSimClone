using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;
using DG.Tweening;


public class FirstPersonController : Singleton<FirstPersonController>
{
    public bool canMove { get; set; } = true;
    private bool isSprinting => canSprint && Input.GetKey(sprintKey);
    private bool shouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    private bool ShouldThrowTool => Input.GetKeyDown(throwToolKey) && !previewMode;
    private bool ShouldDropTool => Input.GetKeyDown(dropToolKey);
    private bool ShouldOpenBox => (Input.GetKeyDown(openBoxKey) && !previewMode) || (Input.GetKeyDown(openBoxKey) && currentHandTool.GetComponent<IFuncitonalSetup>() != null);
    [Header("Funcitonal Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool willSlideOnSlopes = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useFootsteps = true;
    [SerializeField] private bool useStamina = true;
    [HideInInspector] public bool pcMode { get; set; } = false;
     public bool posMode { get; set; } = false;
    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode selectKey = KeyCode.Mouse0;

    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode reInteractKey = KeyCode.Q;
    [SerializeField] private KeyCode throwToolKey = KeyCode.R;
    [SerializeField] private KeyCode dropToolKey = KeyCode.G;
    [SerializeField] private KeyCode openBoxKey = KeyCode.C;
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeSpeed = 8f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Stamina Parameters")]
    [SerializeField] private float maxStamina = 30;
    [SerializeField] private float staminaMultipler = 5f;
    [SerializeField] private float timeBeforeStaminaRegenStarts = 3f;
    [SerializeField] private float staminaValueIncrement = 2f;
    [SerializeField] private float staminaTimeIncrement = 2f;
    private float currentStamina;
    private Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;


    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.11f;
    [SerializeField] private float crouchBobSpeed = 6f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYpOS = 0;
    private float timer;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;
    private float defaulFOV;
    private Coroutine zoomROutine;

    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private float sprintStepMultipler = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] metalClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    private float footstepTimer = 0f;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultipler : isSprinting ? baseStepSpeed * sprintStepMultipler : baseStepSpeed;

    //SLIDING PARAMETERS

    private Vector3 hitPointNormal;
    private bool ISSliding
    {
        get
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f, LayerMask.GetMask("Default")))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
        set
        {

        }

    }

    [Header("Interaction")]
    [SerializeField] public Vector3 interactionRayPoint = default;
    [SerializeField] public float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    [SerializeField] public IInteractable currentInteractable;

    [Header("LongInteraction")]
    private float interactionHoldDuration = 1.5f;

    [Header("HandTool")]
    private bool isHitCollid = false;
    public GameObject currentHandTool;
    [SerializeField] public Transform playerHandPos;
    [SerializeField] private float offSetPreview = 1f;
    [SerializeField] private LayerMask previewLayer;
    [SerializeField] private float handToolRotSpeed = 15f;
    public Vector3 Place;
    [Header("ViewPos")]
    [SerializeField] public Transform playerViewPos;

    private Camera playerCamera;
    private CharacterController characterController;
    [HideInInspector]
    public bool previewMode = false;

    private Vector3 moveDirection;
    private Vector2 currenInput;

    private float rotationX = 0;



    public float proximityThreshold = 0.01f;

    public override void Awake()
    {

        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYpOS = playerCamera.gameObject.transform.localPosition.y;
        defaulFOV = playerCamera.fieldOfView;
        currentStamina = maxStamina;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        base.Awake();
    }

    void Update()
    {

        if (canMove)
        {
            canZoom = true;
            UpdateForMoveFunc();

            if (canInteract && !previewMode)
            {
                UpdateForInteractionFunc();

            }
            if (currentHandTool != null && currentHandTool.GetComponent<IFunctionalBox>() != null)
            {
                UpdateForBoxFunc();
            }
            else if (currentHandTool != null && currentHandTool.GetComponent<IFuncitonalSetup>() != null)
            {
                UpdateForSetupFunc();

            }
            ApplyFinalMovements();
        }
        else if (pcMode)
        {
            
            rotationX = 0;
        }
        else if (posMode)
        {

            HandleMouseLook();
            UpdateForPOSInteractionFunc();


        }

    }
    private void UpdateForSetupFunc()
    {
        bool readyForInt = currentHandTool.GetComponent<IInteractable>().readyForInteract();
        if (readyForInt)
        {
            currentHandTool.GetComponent<IFuncitonalSetup>().PreviewThisInteract();

            if (previewMode)
            {
                bool isCollision = currentHandTool.GetComponent<IFuncitonalSetup>().IsCollision();
                HandlePreview(8f, playerViewPos);
                if (isCollision != true && Input.GetKeyDown(selectKey) && isHitCollid)
                {
                    AINavManager.Instance.UpdateNavMesh();
                    currentHandTool.GetComponent<IFuncitonalSetup>().PlaceandDropThisInteract();
                    currentHandTool = null;
                    previewMode = false;




                }
                if (Input.GetAxis("Mouse ScrollWheel") != 0f && currentHandTool != null)
                {
                    float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
                    float roundedAmount = Mathf.Round(scrollAmount * handToolRotSpeed / 10f) * 10f; // 10'un katlarına yuvarla
                    currentHandTool.gameObject.transform.rotation *= Quaternion.Euler(0, roundedAmount, 0);
                }
                if (ShouldOpenBox)
                {
                    if (currentHandTool.GetComponent<InteractablePCandDesk>() == null)
                    {
                        currentHandTool.GetComponent<IFuncitonalSetup>().MakeBox();
                    }

                }
            }
        }




    }
    private void UpdateForMoveFunc()
    {
            HandleMovementLook();
        HandleMouseLook();

        if (canJump && !isSprinting)
            {
                HandleJump();
            }
            if (canCrouch)
            {

                HandleCrouch();
            }
            if (canUseHeadbob)
            {
                HandleHeadBob();
            }
            if (canZoom)
            {
                HandleZoom();
            }
            if (useFootsteps)
            {
                HandleFootsteps();

            }
            if (useStamina)
            {
                HandleStamina();

            }
        
     


    }

    private void UpdateForInteractionFunc()
    {

        HandleInteractionCheck();
        HandleInteractionInput();


    }
    private void UpdateForPOSInteractionFunc()
    {
        HandlePOSInteractionCheck();
        HandlePOSInteractionInput();


    }
    private void UpdateForBoxFunc()
    {
        bool readyForInt = currentHandTool.GetComponent<IInteractable>().readyForInteract();
        if (readyForInt)
        {

            if (ShouldThrowTool && currentHandTool.GetComponent<IFunctionalBox>() != null)
            {
                DOTween.Kill(currentHandTool.transform);
                currentHandTool.GetComponent<IFunctionalBox>().ThrowThisInteract(playerHandPos);
                currentHandTool = null;

            }

            if (previewMode == true && currentHandTool.GetComponent<IFunctionalBox>() != null)
            {

                bool isCollision = currentHandTool.GetComponent<IFunctionalBox>().IsCollision();
                HandlePreview(5f, playerHandPos);
                if (isCollision != true && Input.GetKeyDown(selectKey))
                {
                    if (currentHandTool.GetComponent<InteractableItemBox>() != null)

                    {
                        currentHandTool.layer = 6;

                    }

                    currentHandTool.GetComponent<IFunctionalBox>().PlaceandDropThisInteract();

                    currentHandTool = null;
                    previewMode = false;

                }
                if (Input.GetAxis("Mouse ScrollWheel") != 0f && currentHandTool != null)
                {
                    float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
                    float roundedAmount = Mathf.Round(scrollAmount * handToolRotSpeed / 10f) * 10f; // 10'un katlarına yuvarla
                    currentHandTool.gameObject.transform.rotation *= Quaternion.Euler(0, roundedAmount, 0);
                }

            }

            if (ShouldDropTool)
            {

                if (previewMode)
                {
                    DOTween.Kill(currentHandTool.transform);
                    currentHandTool.GetComponent<IFunctionalBox>().GetBackPreviewThisInteract();
                    previewMode = false;

                }
                else
                {
                    currentHandTool.GetComponent<IFunctionalBox>().PreviewThisInteract();
                    previewMode = true;

                }
            }
            if (ShouldOpenBox && currentHandTool.GetComponent<IFunctionalBox>() != null)
            {

                currentHandTool.GetComponent<IFunctionalBox>().OpenCloseBox();
                if (currentHandTool.GetComponent<InteractableItemBox>() != null)
                {


                }



            }



        }



    }
    private void HandlePreview(float hitDistance, Transform playerHandPos)
    {
        RaycastHit hit;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Define layer mask to exclude layers 6 and 9
        int layerMask = ~(1 << 6 | 1 << 7 | 1 << 9 | 1 << 10 | 1 << 11);
        if (currentHandTool.GetComponent<InteractableItemBox>() != null)

        {
            layerMask |= 1 << 6;
            currentHandTool.layer = 11;

        }
        if (Physics.Raycast(ray, out hit, hitDistance, layerMask))
        {
            if (hit.collider)
            {

                float yOffset = currentHandTool.transform.localScale.y / 2 + 0.001f;
                Vector3 spawnPosition = hit.point + Vector3.up * yOffset;
                currentHandTool.transform.DOMove(spawnPosition, 0.4f);

                if (hit.collider.transform.CompareTag("Terrain") && Mathf.Abs(currentHandTool.transform.position.y - spawnPosition.y) < proximityThreshold)
                {
                    isHitCollid = true;
                }
                if (currentHandTool.GetComponent<IFuncitonalSetup>() != null)
                {
                    currentHandTool.GetComponent<IFuncitonalSetup>().GetCollision(isHitCollid);
                }
            }
        }
        else if (currentHandTool.GetComponent<IFuncitonalSetup>() != null)
        {
            isHitCollid = false;
            currentHandTool.GetComponent<IFuncitonalSetup>().GetCollision(isHitCollid);



            Vector3 originalPosition = playerViewPos.transform.position;
            Vector3 newPosition = new Vector3(originalPosition.x, playerViewPos.transform.position.y, originalPosition.z);
            // Yüksekliği koru
            currentHandTool.transform.DOMove(newPosition, 0.4f);
        }
        else
        {
            isHitCollid = false;
            Vector3 originalPosition = playerHandPos.transform.position;
            currentHandTool.transform.DOMove(originalPosition, 0.05f);
        }
    }

    private void HandleStamina()
    {
        if (isSprinting && currenInput != Vector2.zero && !isCrouching)
        {
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }
            currentStamina -= staminaMultipler * Time.deltaTime;

            if (currentStamina < 0)
            {
                currentStamina = 0;
            }

            OnStaminaChange?.Invoke(currentStamina);
            if (currentStamina <= 0)
            {
                canSprint = false;
            }
        }
        if ((!isSprinting || isCrouching) && currentStamina < maxStamina && regeneratingStamina == null)
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
            canCrouch = true;
        }
    }

    private void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1 || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultYpOS + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount), playerCamera.transform.localPosition.z);
        }
    }

    private void HandleMovementLook()
    {
        currenInput = new Vector2((isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward * currenInput.x) + (transform.TransformDirection(Vector3.right * currenInput.y)));
        moveDirection.y = moveDirectionY;



    }

    private void HandleMouseLook()

    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        if (currentHandTool != null)
        {
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit / 2, upperLookLimit);
        }
        else
        {
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit, upperLookLimit);

        }


        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        if (previewMode)
        {
            playerHandPos.transform.localRotation = Quaternion.Euler(-rotationX, 0, 0);
            playerViewPos.transform.localRotation = Quaternion.Euler(-rotationX, 0, 0);
        }
        else
        {
            playerViewPos.transform.localRotation = Quaternion.Euler(0, 0, 0);
            playerHandPos.transform.localRotation = Quaternion.Euler(0, 0, 0);

        }
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);





    }
    private void HandleJump()
    {
        if (shouldJump)
        {
            if (isCrouching)
            {
                StartCoroutine(CrouchStand());
            }
            else
            {
                moveDirection.y = jumpForce;
            }
        }
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());

    }
    private void HandleZoom()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            if (zoomROutine != null)
            {

                StopCoroutine(zoomROutine);
                zoomROutine = null;
            }
            zoomROutine = StartCoroutine(ToggleZoom(true));
        }
        if (Input.GetKeyUp(zoomKey))
        {
            if (zoomROutine != null)
            {

                StopCoroutine(zoomROutine);
                zoomROutine = null;
            }
            zoomROutine = StartCoroutine(ToggleZoom(false));
        }

    }


    private void HandleInteractionCheck()
    {

        int layerMask = 1 << 6; // İlk olarak, 6. katmanı (layer) ekliyoruz
        layerMask |= 1 << 11;
        if (currentHandTool != null)
        {
            layerMask |= 1 << 9;    // Daha sonra, 9. katmanı da ekliyoruz
            layerMask &= ~(1 << 6);
        }
        else
        {
            layerMask |= 1 << 6;
            layerMask &= ~(1 << 9);

        }
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, layerMask))
        {
            // Raycast tarafından vurulan objenin layer'ı 6 ise
            if (hit.collider.gameObject.layer == 6 || hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 11)
            {
                // Eğer şu anki etkileşimli obje yoksa veya vurulan obje, şu anki objeden farklıysa
                if (currentInteractable == null || hit.collider.gameObject != currentInteractable.gameObject)
                {
                    // Eğer şu anki etkileşimli obje varsa, odaklanma kaybedilir
                    if (currentInteractable != null)
                    {
                        currentInteractable.OnLoseFocus();
                    }

                    // Yeni etkileşimli obje atanır ve odaklanma sağlanır
                    currentInteractable = hit.collider.gameObject.GetComponent<IInteractable>();
                    if (currentInteractable != null)
                    {
                        currentInteractable.OnFocus();
                    }
                }
            }
            else if (currentInteractable != null)
            {
                // Eğer vurulan obje belirli bir layer'a sahip değilse ve şu anki etkileşimli obje varsa, odaklanma kaybedilir
                currentInteractable.OnLoseFocus();
                currentInteractable = null;
            }
        }
        else if (currentInteractable != null)
        {
            // Eğer raycast hiçbir şeye vurmuyorsa ve şu anki etkileşimli obje varsa, odaklanma kaybedilir
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }

    }
    private void HandlePOSInteractionCheck()
    {

        int layerMask = 1 << 7; // İlk olarak, 6. katmanı (layer) ekliyoruz
    
       
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, layerMask))
        {
            // Raycast tarafından vurulan objenin layer'ı 6 ise
            if (hit.collider.gameObject.layer == 7)
            {
                // Eğer şu anki etkileşimli obje yoksa veya vurulan obje, şu anki objeden farklıysa
                if (currentInteractable == null || hit.collider.gameObject != currentInteractable.gameObject)
                {
                    // Eğer şu anki etkileşimli obje varsa, odaklanma kaybedilir
                    if (currentInteractable != null)
                    {
                        currentInteractable.OnLoseFocus();
                    }

                    // Yeni etkileşimli obje atanır ve odaklanma sağlanır
                    currentInteractable = hit.collider.gameObject.GetComponent<IInteractable>();
                    if (currentInteractable != null)
                    {
                        currentInteractable.OnFocus();
                    }
                }
            }
            else if (currentInteractable != null)
            {
                // Eğer vurulan obje belirli bir layer'a sahip değilse ve şu anki etkileşimli obje varsa, odaklanma kaybedilir
                currentInteractable.OnLoseFocus();
                currentInteractable = null;
            }
        }
        else if (currentInteractable != null)
        {
            // Eğer raycast hiçbir şeye vurmuyorsa ve şu anki etkileşimli obje varsa, odaklanma kaybedilir
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }


    }
    private void HandlePOSInteractionInput()
    {

        if (posMode && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {


            if (Input.GetKeyDown(interactKey))
            {

                
               
                if (currentInteractable?.GetComponent<InteractableItem>() != null)
                {

                    currentInteractable.OnInteract(currentInteractable?.transform.parent.GetComponent<GameObjectChildController>().bagPoint);

                }
                if (currentInteractable?.GetComponent<InteractableMoney>() != null)
                {

                    currentInteractable.OnInteract(playerHandPos);

                }

            }
         
        }

    }

   
    private void HandleInteractionInput()
    {

        if (currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {


            if (Input.GetKeyUp(interactKey))
            {

                interactionHoldDuration = 1f;
                if ((currentInteractable.GetComponent<IFunctionalBox>() != null && currentHandTool == null))
                {
                    currentInteractable.OnInteract(playerHandPos);
                    if (currentInteractable.GetComponent<InteractableItemBox>() != null)
                    {


                    }
                    currentHandTool = currentInteractable.gameObject;

                }
                else if (currentInteractable?.GetComponent<PriceTagData>() != null)
                {

                    currentInteractable.OnInteract(playerViewPos);



                }
                else if (currentInteractable?.GetComponent<InteractableShelfPart>() != null && currentHandTool?.GetComponent<InteractableItemBox>() != null && currentHandTool != null)
                {

                    currentInteractable.OnInteract(playerViewPos);



                }
                else if (currentInteractable?.GetComponent<InteractableTrash>() && currentHandTool != null)
                {


                    currentInteractable.OnInteract(playerHandPos);

                }
                else if (currentInteractable?.GetComponent<InteractableStorageShelfPart>() != null)
                {


                    currentInteractable.OnInteract(playerViewPos);




                }

                else if (currentInteractable?.GetComponent<InteractablePCandDesk>() != null && currentHandTool == null)
                {

                    currentInteractable.GetComponent<InteractablePCandDesk>().InteractWithPC();

                }
                else if (currentInteractable?.GetComponent<InteractablePOS>() != null && currentHandTool == null )
                {
                    
                        currentInteractable.GetComponent<InteractablePOS>().InteractWithPOS();

                 

                }
            }
            else if (Input.GetKeyDown(reInteractKey))
            {
                if (currentInteractable?.GetComponent<InteractableShelfPart>() != null && currentHandTool?.GetComponent<InteractableItemBox>() != null && currentHandTool != null)
                {
                    currentInteractable?.GetComponent<InteractableShelfPart>().OnReInteract(playerViewPos);

                }
                if (currentInteractable?.GetComponent<InteractableStorageShelfPart>() != null && currentHandTool == null)
                {


                    currentInteractable?.GetComponent<InteractableStorageShelfPart>().OnReInteract(playerViewPos);


                }
            }

            if (Input.GetKey(interactKey))
            {
                if (currentInteractable.GetComponent<IFuncitonalSetup>() != null && currentHandTool == null)
                {

              
                if (currentInteractable?.GetComponent<InteractablePOS>()?.myCustomers.Count >0)
                {
                        Debug.Log("Kasa dolu");
                        return;
                    }
                    interactionHoldDuration -= Time.deltaTime;
                    Debug.Log(interactionHoldDuration);
                    if (interactionHoldDuration <= 0)
                    {
                        currentInteractable.OnInteract(playerViewPos);
                        currentHandTool = currentInteractable.gameObject;
                        previewMode = true;
                        interactionHoldDuration = 1f;
                    }

                }


            }
        }




    }
    private void HandleFootsteps()
    {
        if (!characterController.isGrounded) return;
        if (currenInput == Vector2.zero) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footstep/WOOD":
                        footstepAudioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, woodClips.Length - 1)]);
                        break;
                    case "Footstep/METAL":
                        footstepAudioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, metalClips.Length - 1)]);
                        break;
                    case "Footstep/GRASS":
                        footstepAudioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, grassClips.Length - 1)]);
                        break;
                    default:
                        footstepAudioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, grassClips.Length - 1)]);

                        break;
                }
                footstepTimer = GetCurrentOffset;
            }
        }
    }
    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;

        }
        if (willSlideOnSlopes && ISSliding)
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;

        }
        characterController.Move(moveDirection * Time.deltaTime);
    }


    public void GoPCDesk(Transform newPos)
    {
      
        transform.DOMove(newPos.position, 0.05f);
        Vector3 pcRot = newPos.parent.parent.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, pcRot.y, 0f);

    }
    public void GoPOSDesk(Transform newPos)
    { 
        transform.DOMove(newPos.position, 0.3f);
        Vector3 posRot = newPos.rotation.eulerAngles;
        transform.DORotate(new Vector3(0f, posRot.y, 0f),0.4f);


    }
    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
        {
            yield break;
        }

        duringCrouchAnimation = true;

        
        if (isSprinting || Input.GetKey(sprintKey))
        {
            isCrouching = false; 
            duringCrouchAnimation = false;
            yield break;
        }

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / timeToCrouch;
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, t);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, t);
            yield return null;
        }

        // Eğer crouch durumu bittiğinde karakterin başka bir engel tarafından kalkık kalması engelleniyor
        if (Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
        {
            isCrouching = true; // Eğer engel varsa crouch durumunu tekrar etkinleştir
            duringCrouchAnimation = false;
            yield break;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;
        duringCrouchAnimation = false;

    }
    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaulFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;
        while (timeElapsed < timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        zoomROutine = null;
    }

    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);

        while (currentStamina < maxStamina)
        {
            if (currentStamina > 0)
            {

                canSprint = true;
            }

            currentStamina += staminaValueIncrement;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
            OnStaminaChange?.Invoke(currentStamina);
            yield return timeToWait;
        }

        regeneratingStamina = null;

    }



}

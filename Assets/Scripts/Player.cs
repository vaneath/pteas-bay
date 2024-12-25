using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IKitchenObjectParent
{


    public static Player Instance { get; private set; }



    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private Image energyBarImage;


    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private bool isSpeedBoosted;

    private float maxEnergy = 100f; // Total energy
    private float currentEnergy; // Current energy level
    private float energyCost = 20f; // Energy needed for each speed boost
    private float energyRegenerationRate = 10f;
    private Coroutine energyRegenerationCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
        currentEnergy = maxEnergy;
        energyBarImage.gameObject.SetActive(true);
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        gameInput.OnSpeedBoostAction += GameInput_OnSpeedBoostAction;
        gameInput.OnSpeedBoostCancel += GameInput_OnSpeedBoostCancel;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        Debug.Log("Current energy: " + currentEnergy + ", SpeedBoost: " + isSpeedBoosted);
        HandleMovement();
        HandleInteractions();
        UpdateEnergyBar();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Has ClearCounter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);

            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    private void UpdateEnergyBar() {
        if (energyBarImage != null) {
            energyBarImage.fillAmount = currentEnergy / maxEnergy; // Update the fill amount of UI element
        }
    }

    private void GameInput_OnSpeedBoostAction(object sender, EventArgs e) {
        MoveSpeedBoost();
    }

    private void MoveSpeedBoost() {
        if (isSpeedBoosted) return; // Prevent multiple boosts

        if (currentEnergy >= energyCost) {
            moveSpeed = 12f; // Boost speed
            currentEnergy -= energyCost; // Deduct energy cost
            isSpeedBoosted = true;

            // Start energy consumption coroutine
            StartCoroutine(ConsumeEnergy());
            // Start energy regeneration if it's not already running
            if (energyRegenerationCoroutine != null) {
                StopCoroutine(energyRegenerationCoroutine);
            }
            energyRegenerationCoroutine = StartCoroutine(RegenerateEnergy());
        }
    }

    // New Coroutine to consume energy continuously
    private IEnumerator ConsumeEnergy() {
        while (isSpeedBoosted) {
            // Deduct energy continuously
            if (currentEnergy > 0) {
                currentEnergy -= energyCost * Time.deltaTime; // Deduct energy over time
                currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy); // Clamp to max energy
                UpdateEnergyBar(); // Update UI
            } else {
                ResetSpeed(); // Reset speed when energy runs out
                break; // Exit the loop if energy is depleted
            }

            yield return null; // Wait for the next frame
        }
    }

    private IEnumerator RegenerateEnergy() {
        while (currentEnergy < maxEnergy) {
            // Check if the player is currently boosted
            if (!isSpeedBoosted) {
                currentEnergy += energyRegenerationRate * Time.deltaTime; // Regenerate energy
                currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy); // Clamp to max energy
                
                // Update energy bar UI for the visual feedback
                UpdateEnergyBar();
            }
            
            // Wait for a frame before checking again
            yield return null; 
        }

        energyRegenerationCoroutine = null; // Reset coroutine reference when done
    }

    private void GameInput_OnSpeedBoostCancel(object sender, EventArgs e) {
        ResetSpeed();
    }

    private void ResetSpeed() {
        if (isSpeedBoosted) {
            moveSpeed = 7f;
            isSpeedBoosted = false;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using KitchenObjects;
using KitchenObjects.Container;
using KitchenObjects.ScriptableObjects;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

namespace Players {
    public class PlayerBot : MonoBehaviour {
        private DeliveryManager deliveryManager;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private Player player;

        public bool HasPlate => Plate != null;
        public PlateKitchenObject Plate { get; private set; }

        public bool HasRecipe => CurrentRecipe != null;
        public CompleteRecipeSO CurrentRecipe { get; private set; }


        public event EventHandler<ValueChangedEvent<CompleteRecipeSO>> OnRecipeChanged;
        public event EventHandler<ValueChangedEvent<PlateKitchenObject>> OnPlateChanged;
        public event EventHandler<KitchenObjectsChangedEvent> OnPlateIngredientsChanged;

        private void Start() {
            deliveryManager = DeliveryManager.Instance;
            agent.speed = playerMovement.MovementSpeed;
            agent.angularSpeed = playerMovement.RotationSpeed;

            // todo: make a HigherLevelAI that handles recipe selection and coordination
            TrySelectRecipe();

            deliveryManager.OnRecipeCreated += OnRecipeCreated;
            deliveryManager.OnRecipeSuccess += OnRecipeSuccess;

            player.Container.OnKitchenObjectAdded += OnPlayerKOAdded;
        }

        private void OnPlayerKOAdded(object sender, KitchenObject e) {
            if (e is PlateKitchenObject plate) {
                if (Plate != plate)
                    SetPlate(plate);
            }
        }

        private void SetPlate(PlateKitchenObject plate) {
            var oldValue = Plate;
            if (oldValue == plate)
                return;

            if (oldValue != null) {
                oldValue.IngredientsContainer.OnKitchenObjectsChanged -= InvokePlateIngredientsChanged;
                oldValue.OnDestroyed -= OnPlateDestroyed;
            }

            Plate = plate;

            if (Plate != null) {
                Plate.IngredientsContainer.OnKitchenObjectsChanged += InvokePlateIngredientsChanged;
                Plate.OnDestroyed += OnPlateDestroyed;
            }

            OnPlateChanged?.Invoke(this, new ValueChangedEvent<PlateKitchenObject>(oldValue, Plate));
            
            if (Plate != null) {
                var ingredientsContainer = Plate.IngredientsContainer;
                OnPlateIngredientsChanged?.Invoke(this, new KitchenObjectsChangedEvent(
                    ingredientsContainer, 
                    ingredientsContainer.KitchenObjects, 
                    ingredientsContainer.KitchenObject
                ));
            }
        }

        private void InvokePlateIngredientsChanged(object sender, KitchenObjectsChangedEvent e) {
            OnPlateIngredientsChanged?.Invoke(this, e);
        }

        private void OnPlateDestroyed(object o, EventArgs eventArgs) => SetPlate(null);


        void Update() {
            if (Input.GetMouseButtonDown(1)) {
                SetDestinationToMousePosition();
            }

            if (Input.GetMouseButtonDown(2)) {
                player.TryAlternateInteract();
                player.StartAlternateInteract();
            } else if (Input.GetMouseButtonUp(2)) {
                player.StopAlternateInteract();
            }

            if (Input.GetMouseButtonDown(0)) {
                player.TryInteract();
            }


            // playerMovement.desiredMoveDirection = agent.desiredVelocity;
        }


        private void OnRecipeSuccess(object sender, RecipeDeliveryEvent e) {
            if (e.Player == player) {
                TrySelectRecipe(true);
            }
        }

        private void OnRecipeCreated(object sender, CompleteRecipeSO e) {
            if (HasRecipe)
                return;
            TrySelectRecipe();
        }

        private void TrySelectRecipe(bool forceEvent = false) {
            var oldRecipe = CurrentRecipe;
            CurrentRecipe = deliveryManager.WaitingRecipeSOs.FirstOrDefault();

            if (forceEvent || oldRecipe != CurrentRecipe)
                OnRecipeChanged?.Invoke(this, new ValueChangedEvent<CompleteRecipeSO>(oldRecipe, CurrentRecipe));
        }


        void SetDestinationToMousePosition() {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                playerMovement.TryMoveTo(hit.point);

                playerMovement.LookAt(hit.collider.transform);
            }
        }
    }
}

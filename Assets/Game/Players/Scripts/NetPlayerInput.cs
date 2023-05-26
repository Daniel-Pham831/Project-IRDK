using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Interfaces;
using Game.Weapons;
using ToolBox.Tags;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayerInput : NetworkBehaviour
    {
        private Vector2 _rawInput;
        private Vector2 _smoothInput;
        
        public Vector2 RawInputVector
        {
            get => IsOwner ? _rawInput : NetRawInputVector.Value;
            private set => _rawInput = value;
        }

        public Vector2 SmoothInputVector 
        {
            get => IsOwner ? _smoothInput : NetSmoothInputVector.Value;
            private set => _smoothInput = value;
        }

        public Vector2ReactiveProperty RawInputVectorReactive { get; private set; } = new Vector2ReactiveProperty();
        public BoolReactiveProperty IsFirePressed { get; private set; } = new BoolReactiveProperty(false);
        public BoolReactiveProperty IsInteractable { get; private set; } = new BoolReactiveProperty(false);

        [HideInInspector]
        public NetworkVariable<Vector2> NetRawInputVector = new NetworkVariable<Vector2>(default,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [HideInInspector]
        public NetworkVariable<Vector2> NetSmoothInputVector = new NetworkVariable<Vector2>(default,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        private ReactiveCollection<IInteractableMono> _interactables = new ReactiveCollection<IInteractableMono>();
        
        private void Awake()
        {
            _interactables.ObserveCountChanged(true).Subscribe(count =>
            {
                IsInteractable.Value = count > 0;
            }).AddTo(this);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;

            base.OnNetworkSpawn();
        }
        
        public void Update()
        {
            if (IsOwner)
            {
                SetRawInput(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized);
                SetSmoothInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized);

                IsFirePressed.Value = Input.GetKeyDown(KeyCode.Space);
            }
        }
        
        public void SetRawInput(Vector2 rawInput)
        {
            RawInputVector = rawInput;
            NetRawInputVector.Value = rawInput;
            RawInputVectorReactive.Value = rawInput;
        }
        
        public void SetSmoothInput(Vector2 smoothInput)
        {
            SmoothInputVector = smoothInput;
            NetSmoothInputVector.Value = smoothInput;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(col.TryGetComponent<IInteractableMono>(out var interactable))
            {
                _interactables.Add(interactable);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.TryGetComponent<IInteractableMono>(out var interactable))
            {
                if(_interactables.Contains(interactable))
                    _interactables.Remove(interactable);
            }
        }
        
        public async UniTask InteractWithInteractable()
        {
            if (_interactables.Count > 0 && IsInteractable.Value)
            {
                var interactable = GetClosestAndLowestPriorityInteractable();
                await interactable.Interact(this);
            }
        }

        private IInteractableMono GetClosestAndLowestPriorityInteractable()
        {
            IInteractableMono closestInteractableMono = null;
            var closestDistance = float.MaxValue;
            var lowestPriority = int.MaxValue;
            
            foreach (var interactable in _interactables)
            {
                var distance = Vector2.Distance(transform.position, interactable.Mono.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractableMono = interactable;
                    lowestPriority = interactable.InteractPriority;
                }
                else if (Mathf.Abs(distance - closestDistance) <= 0.01f)
                {
                    if (interactable.InteractPriority < lowestPriority)
                    {
                        closestInteractableMono = interactable;
                        lowestPriority = interactable.InteractPriority;
                    }
                }
            }

            return closestInteractableMono;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using PuzzleDungeon.Input;
using UnityEngine;

namespace PuzzleDungeon.Character
{
    public class CharacterHub : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        private List<CharacterComponent> _allCharacterComponents;
        private List<IInputReceiver>     _inputReceivers;
        private InputManager             _inputManager;
        
        public InputManager P_InputManager => _inputManager;
        public Animator     P_Animator     => animator;
        
        public T GetCharacterComponent<T>() where T : CharacterComponent
        {
            if (_allCharacterComponents == null)
            {
                return null;
            }

            var find = _allCharacterComponents.FirstOrDefault(x => x.GetType() == typeof(T));
            return find as T;
        }

        private void Awake()
        {
            _allCharacterComponents = new List<CharacterComponent>();
            _inputReceivers         = new List<IInputReceiver>();
            _inputManager           = InputManager.Instance;
            
            _allCharacterComponents.AddRange(GetComponentsInChildren<CharacterComponent>());
            _inputReceivers.AddRange(GetComponentsInChildren<IInputReceiver>());
            
            foreach (var cc in _allCharacterComponents)
            {
                cc.P_CharacterHub = this;
                cc.Initialize();
            }
        }

        private void OnEnable()
        {
            foreach (var cc in _allCharacterComponents)
            {
                cc.ProcessHubEnable();
            }
            
            foreach (var ir in _inputReceivers)
            {
                ir.ListenToInputEvents(_inputManager);
            }
        }

        private void OnDisable()
        {
            foreach (var cc in _allCharacterComponents)
            {
                cc.ProcessHubDisable();
            }

            foreach (var ir in _inputReceivers)
            {
                ir.StopListeningToInputEvents(_inputManager);
            }
        }

        private void ProcessMovementState()
        {
        }

        private void Update()
        {
            foreach (var ir in _inputReceivers)
            {
                ir.ReceiveInputUpdate(_inputManager);
            }
            
            foreach (var cc in _allCharacterComponents)
            {
                cc.ProcessUpdate();
            }

            foreach (var cc in _allCharacterComponents)
            {
                cc.UpdateAnimator();
            }
        }

        private void LateUpdate()
        {
            foreach (var cc in _allCharacterComponents)
            {
                cc.ProcessLateUpdate();
            }
        }

        private void FixedUpdate()
        {
            foreach (var cc in _allCharacterComponents)
            {
                cc.ProcessFixedUpdate();
            }
        }
    }
}
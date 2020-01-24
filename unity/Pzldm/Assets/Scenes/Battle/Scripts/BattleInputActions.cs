// GENERATED AUTOMATICALLY FROM 'Assets/Scenes/Battle/Scripts/BattleInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Pzldm
{
    public class @BattleInputActions : IInputActionCollection, IDisposable
    {
        private InputActionAsset asset;
        public @BattleInputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""BattleInputActions"",
    ""maps"": [
        {
            ""name"": ""Battle"",
            ""id"": ""68a36ec1-ae75-4e2d-9762-b1b38e4cad2d"",
            ""actions"": [
                {
                    ""name"": ""RotateLeft"",
                    ""type"": ""Button"",
                    ""id"": ""44b7c751-09fe-4b64-8353-eb8c0561c9b7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RotateRight"",
                    ""type"": ""Button"",
                    ""id"": ""38cd0df5-1a81-44e5-9122-bd44977b4284"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dpad"",
                    ""type"": ""Value"",
                    ""id"": ""97a3ed40-da85-4b87-947e-57c86593a0e6"",
                    ""expectedControlType"": ""Dpad"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""64b2aaf2-69a3-49ff-a31c-79f3877b3c8c"",
                    ""path"": ""<Keyboard>/#(K)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4749e538-e839-4b4c-97a6-fb2e33ec0a5d"",
                    ""path"": ""<Keyboard>/#(J)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""05106ad4-de03-442c-a5c7-37535724d200"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""de236253-bcfb-4caf-9d4b-36d24bae2a91"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a4fa8ac1-d23d-4b16-9cbb-8fa53685fff2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b7f9e851-3edc-4a69-8896-3ae82c33881c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""cffcf15c-9a9e-49e3-97b9-2d2359c9da6c"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""e5aa3fb7-5e65-45f7-9dbf-7dc595f0e3a4"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2543788b-7fe7-4bed-8e7f-7b647c84d95c"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a2603fc1-3bf2-48bb-980f-795bacdef650"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""aae6ff4c-ab2d-4e05-a9ef-dcb2b89bbb9d"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""10a253dd-e1b7-4719-a4e8-ceb2bd715a0c"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dpad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Battle
            m_Battle = asset.FindActionMap("Battle", throwIfNotFound: true);
            m_Battle_RotateLeft = m_Battle.FindAction("RotateLeft", throwIfNotFound: true);
            m_Battle_RotateRight = m_Battle.FindAction("RotateRight", throwIfNotFound: true);
            m_Battle_Dpad = m_Battle.FindAction("Dpad", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Battle
        private readonly InputActionMap m_Battle;
        private IBattleActions m_BattleActionsCallbackInterface;
        private readonly InputAction m_Battle_RotateLeft;
        private readonly InputAction m_Battle_RotateRight;
        private readonly InputAction m_Battle_Dpad;
        public struct BattleActions
        {
            private @BattleInputActions m_Wrapper;
            public BattleActions(@BattleInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @RotateLeft => m_Wrapper.m_Battle_RotateLeft;
            public InputAction @RotateRight => m_Wrapper.m_Battle_RotateRight;
            public InputAction @Dpad => m_Wrapper.m_Battle_Dpad;
            public InputActionMap Get() { return m_Wrapper.m_Battle; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(BattleActions set) { return set.Get(); }
            public void SetCallbacks(IBattleActions instance)
            {
                if (m_Wrapper.m_BattleActionsCallbackInterface != null)
                {
                    @RotateLeft.started -= m_Wrapper.m_BattleActionsCallbackInterface.OnRotateLeft;
                    @RotateLeft.performed -= m_Wrapper.m_BattleActionsCallbackInterface.OnRotateLeft;
                    @RotateLeft.canceled -= m_Wrapper.m_BattleActionsCallbackInterface.OnRotateLeft;
                    @RotateRight.started -= m_Wrapper.m_BattleActionsCallbackInterface.OnRotateRight;
                    @RotateRight.performed -= m_Wrapper.m_BattleActionsCallbackInterface.OnRotateRight;
                    @RotateRight.canceled -= m_Wrapper.m_BattleActionsCallbackInterface.OnRotateRight;
                    @Dpad.started -= m_Wrapper.m_BattleActionsCallbackInterface.OnDpad;
                    @Dpad.performed -= m_Wrapper.m_BattleActionsCallbackInterface.OnDpad;
                    @Dpad.canceled -= m_Wrapper.m_BattleActionsCallbackInterface.OnDpad;
                }
                m_Wrapper.m_BattleActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @RotateLeft.started += instance.OnRotateLeft;
                    @RotateLeft.performed += instance.OnRotateLeft;
                    @RotateLeft.canceled += instance.OnRotateLeft;
                    @RotateRight.started += instance.OnRotateRight;
                    @RotateRight.performed += instance.OnRotateRight;
                    @RotateRight.canceled += instance.OnRotateRight;
                    @Dpad.started += instance.OnDpad;
                    @Dpad.performed += instance.OnDpad;
                    @Dpad.canceled += instance.OnDpad;
                }
            }
        }
        public BattleActions @Battle => new BattleActions(this);
        public interface IBattleActions
        {
            void OnRotateLeft(InputAction.CallbackContext context);
            void OnRotateRight(InputAction.CallbackContext context);
            void OnDpad(InputAction.CallbackContext context);
        }
    }
}

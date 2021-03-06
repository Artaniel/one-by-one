// GENERATED AUTOMATICALLY FROM 'Assets/External/Input System/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""gameplay"",
            ""id"": ""47241876-4bf8-414d-b656-2ab6e685cca0"",
            ""actions"": [
                {
                    ""name"": ""fire1"",
                    ""type"": ""Button"",
                    ""id"": ""86c793fa-2cf2-46b7-bfd7-8ad55f511785"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""move"",
                    ""type"": ""Value"",
                    ""id"": ""4767f72d-b050-4e54-a13f-bedecf966bc3"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": ""AxisDeadzone(min=0.001)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""look"",
                    ""type"": ""Value"",
                    ""id"": ""d866057e-4d75-4669-95f6-980c1393e9e4"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": ""AxisDeadzone(min=0.001)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""reload"",
                    ""type"": ""Button"",
                    ""id"": ""88bbeeb6-4b74-4eca-aefc-9274e6695dc2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""AxisDeadzone(min=0.001)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""skill1"",
                    ""type"": ""Button"",
                    ""id"": ""85ac8141-0eb7-494a-a9fa-f1f7c35c28ba"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""AxisDeadzone(min=0.001)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""skill2"",
                    ""type"": ""Button"",
                    ""id"": ""f9fa2527-278b-494a-9297-c1ce6bd5458e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""AxisDeadzone(min=0.001)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""skill3"",
                    ""type"": ""Button"",
                    ""id"": ""1275ba41-5301-4c0a-8b8a-3e6a0561feed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""AxisDeadzone(min=0.001)"",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8a691e66-0c6c-4147-973a-b004063499ac"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""fire1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0b1893bd-71e2-44a7-9b92-6baf206ff0b9"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""fire1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""233e5958-8514-43e2-b8fd-14ce22621596"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2b7dd23a-9646-4a5a-b732-4700ca148256"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a42feb87-6a56-4c44-8e16-f111116abdeb"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b11a5867-4cf7-4274-9d6d-307f1e687af9"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e87d6084-4f97-4e14-bc1e-cac66be7592f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f961503d-98aa-40a7-9981-f59d22f96c79"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""11714820-f017-4721-a1ab-6c867a49c432"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5fcfbfae-56f1-4818-b881-d3c222c4ae2f"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7538d6b9-d267-4d2e-a58d-3980c2e85cf9"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e699d3e1-8e99-4251-bb45-7f6d96864889"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""3a61b8af-73a9-4d50-9da0-1d472580ecf1"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""look"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c3a669af-4bde-4038-8468-feaeb5fc80f6"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f4c1723d-e58b-47c1-88d6-f9fd2c4e54fb"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5de8effa-dd77-4301-8529-32e883cfd771"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7218ab28-8756-4d20-8e81-a690cb1ee197"",
                    ""path"": ""<Gamepad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9f837eff-c059-4266-bf70-21c4abff4a0a"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""915a14a1-12b1-4e09-9dd4-8eeea3dbe0fd"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce8fd39b-02d0-484b-8ed6-78a8bd0d4593"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""skill1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ad5fcc9-00a4-4db5-b1bf-6ff46b1d0b1f"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""skill1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49ee4e60-a61f-4437-b145-7e4dbd8702a1"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""skill2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""268b7860-4eff-4da9-b176-9b9ed6abe92e"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""skill2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df19330d-9557-4afa-b90d-906e502ff0ed"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""skill3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""42385e21-a87b-4943-b64c-b0a6e918bef4"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""skill3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // gameplay
        m_gameplay = asset.FindActionMap("gameplay", throwIfNotFound: true);
        m_gameplay_fire1 = m_gameplay.FindAction("fire1", throwIfNotFound: true);
        m_gameplay_move = m_gameplay.FindAction("move", throwIfNotFound: true);
        m_gameplay_look = m_gameplay.FindAction("look", throwIfNotFound: true);
        m_gameplay_reload = m_gameplay.FindAction("reload", throwIfNotFound: true);
        m_gameplay_skill1 = m_gameplay.FindAction("skill1", throwIfNotFound: true);
        m_gameplay_skill2 = m_gameplay.FindAction("skill2", throwIfNotFound: true);
        m_gameplay_skill3 = m_gameplay.FindAction("skill3", throwIfNotFound: true);
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

    // gameplay
    private readonly InputActionMap m_gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_gameplay_fire1;
    private readonly InputAction m_gameplay_move;
    private readonly InputAction m_gameplay_look;
    private readonly InputAction m_gameplay_reload;
    private readonly InputAction m_gameplay_skill1;
    private readonly InputAction m_gameplay_skill2;
    private readonly InputAction m_gameplay_skill3;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @fire1 => m_Wrapper.m_gameplay_fire1;
        public InputAction @move => m_Wrapper.m_gameplay_move;
        public InputAction @look => m_Wrapper.m_gameplay_look;
        public InputAction @reload => m_Wrapper.m_gameplay_reload;
        public InputAction @skill1 => m_Wrapper.m_gameplay_skill1;
        public InputAction @skill2 => m_Wrapper.m_gameplay_skill2;
        public InputAction @skill3 => m_Wrapper.m_gameplay_skill3;
        public InputActionMap Get() { return m_Wrapper.m_gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @fire1.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFire1;
                @fire1.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFire1;
                @fire1.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFire1;
                @move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @look.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLook;
                @look.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLook;
                @look.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLook;
                @reload.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                @reload.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                @reload.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                @skill1.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkill1;
                @skill1.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkill1;
                @skill1.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkill1;
                @skill2.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkill2;
                @skill2.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkill2;
                @skill2.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkill2;
                @skill3.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkill3;
                @skill3.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkill3;
                @skill3.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSkill3;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @fire1.started += instance.OnFire1;
                @fire1.performed += instance.OnFire1;
                @fire1.canceled += instance.OnFire1;
                @move.started += instance.OnMove;
                @move.performed += instance.OnMove;
                @move.canceled += instance.OnMove;
                @look.started += instance.OnLook;
                @look.performed += instance.OnLook;
                @look.canceled += instance.OnLook;
                @reload.started += instance.OnReload;
                @reload.performed += instance.OnReload;
                @reload.canceled += instance.OnReload;
                @skill1.started += instance.OnSkill1;
                @skill1.performed += instance.OnSkill1;
                @skill1.canceled += instance.OnSkill1;
                @skill2.started += instance.OnSkill2;
                @skill2.performed += instance.OnSkill2;
                @skill2.canceled += instance.OnSkill2;
                @skill3.started += instance.OnSkill3;
                @skill3.performed += instance.OnSkill3;
                @skill3.canceled += instance.OnSkill3;
            }
        }
    }
    public GameplayActions @gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnFire1(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnSkill1(InputAction.CallbackContext context);
        void OnSkill2(InputAction.CallbackContext context);
        void OnSkill3(InputAction.CallbackContext context);
    }
}

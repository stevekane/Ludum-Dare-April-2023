//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Input/PlayerActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerActions"",
    ""maps"": [
        {
            ""name"": ""InGame"",
            ""id"": ""de0d8774-01b7-4c6a-b719-8f8effd8e91a"",
            ""actions"": [
                {
                    ""name"": ""Red"",
                    ""type"": ""Button"",
                    ""id"": ""df7d98bb-fe55-44da-aadc-7d095e93430b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Green"",
                    ""type"": ""Button"",
                    ""id"": ""af17b314-4e75-4109-acb6-129f571bdf5a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Blue"",
                    ""type"": ""Button"",
                    ""id"": ""31f3f14b-60f2-409c-b4da-c20cebbc9397"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Swing"",
                    ""type"": ""Button"",
                    ""id"": ""4ad5c57c-dc46-4e92-ae43-2f3d4bbaeec3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""ac3f4491-a9ec-4419-829e-54d3b02f238f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""StickDeadzone"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Restart"",
                    ""type"": ""Button"",
                    ""id"": ""0c8e305c-941f-4d6e-ae82-cc4118f9b346"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9332f1df-fc1d-4722-ad89-70e30b510781"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Swing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef79e4d9-9aef-45c9-a157-aa19348e2dc0"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard+Mouse"",
                    ""action"": ""Swing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5dbf9ae7-f3db-4a0b-945c-fe60541a6b10"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard+Mouse"",
                    ""action"": ""Swing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6d312177-e585-4be8-9657-e51926ec63de"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""badf7db2-0237-4394-b53d-2bda077ead70"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false),ScaleVector2(x=0.5,y=0.5)"",
                    ""groups"": ""Keyboard+Mouse"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""60d05b31-b68d-458f-95e5-13b8131a6c39"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Green"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62742e9c-46d1-4f42-b0c7-a66192969956"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Green"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4ba30eb-97eb-4c60-bc69-9f83c3709b51"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Blue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09f2d7ba-272a-4690-b2da-d6e372df00e1"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Blue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46db5697-07ed-4e6c-8dbc-8ae96e65fb77"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Restart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""60569179-a0f8-40c7-bce2-d49a9b09b0de"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard+Mouse"",
                    ""action"": ""Restart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""63224bb0-ef7a-404c-90c6-096bc861e7d8"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Red"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""804088e8-0c5a-4226-bd0a-7e27fe5b16be"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Red"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard+Mouse"",
            ""bindingGroup"": ""Keyboard+Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // InGame
        m_InGame = asset.FindActionMap("InGame", throwIfNotFound: true);
        m_InGame_Red = m_InGame.FindAction("Red", throwIfNotFound: true);
        m_InGame_Green = m_InGame.FindAction("Green", throwIfNotFound: true);
        m_InGame_Blue = m_InGame.FindAction("Blue", throwIfNotFound: true);
        m_InGame_Swing = m_InGame.FindAction("Swing", throwIfNotFound: true);
        m_InGame_Aim = m_InGame.FindAction("Aim", throwIfNotFound: true);
        m_InGame_Restart = m_InGame.FindAction("Restart", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // InGame
    private readonly InputActionMap m_InGame;
    private IInGameActions m_InGameActionsCallbackInterface;
    private readonly InputAction m_InGame_Red;
    private readonly InputAction m_InGame_Green;
    private readonly InputAction m_InGame_Blue;
    private readonly InputAction m_InGame_Swing;
    private readonly InputAction m_InGame_Aim;
    private readonly InputAction m_InGame_Restart;
    public struct InGameActions
    {
        private @PlayerActions m_Wrapper;
        public InGameActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Red => m_Wrapper.m_InGame_Red;
        public InputAction @Green => m_Wrapper.m_InGame_Green;
        public InputAction @Blue => m_Wrapper.m_InGame_Blue;
        public InputAction @Swing => m_Wrapper.m_InGame_Swing;
        public InputAction @Aim => m_Wrapper.m_InGame_Aim;
        public InputAction @Restart => m_Wrapper.m_InGame_Restart;
        public InputActionMap Get() { return m_Wrapper.m_InGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InGameActions set) { return set.Get(); }
        public void SetCallbacks(IInGameActions instance)
        {
            if (m_Wrapper.m_InGameActionsCallbackInterface != null)
            {
                @Red.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnRed;
                @Red.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnRed;
                @Red.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnRed;
                @Green.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnGreen;
                @Green.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnGreen;
                @Green.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnGreen;
                @Blue.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnBlue;
                @Blue.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnBlue;
                @Blue.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnBlue;
                @Swing.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnSwing;
                @Swing.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnSwing;
                @Swing.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnSwing;
                @Aim.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnAim;
                @Restart.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnRestart;
                @Restart.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnRestart;
                @Restart.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnRestart;
            }
            m_Wrapper.m_InGameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Red.started += instance.OnRed;
                @Red.performed += instance.OnRed;
                @Red.canceled += instance.OnRed;
                @Green.started += instance.OnGreen;
                @Green.performed += instance.OnGreen;
                @Green.canceled += instance.OnGreen;
                @Blue.started += instance.OnBlue;
                @Blue.performed += instance.OnBlue;
                @Blue.canceled += instance.OnBlue;
                @Swing.started += instance.OnSwing;
                @Swing.performed += instance.OnSwing;
                @Swing.canceled += instance.OnSwing;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Restart.started += instance.OnRestart;
                @Restart.performed += instance.OnRestart;
                @Restart.canceled += instance.OnRestart;
            }
        }
    }
    public InGameActions @InGame => new InGameActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard+Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IInGameActions
    {
        void OnRed(InputAction.CallbackContext context);
        void OnGreen(InputAction.CallbackContext context);
        void OnBlue(InputAction.CallbackContext context);
        void OnSwing(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnRestart(InputAction.CallbackContext context);
    }
}

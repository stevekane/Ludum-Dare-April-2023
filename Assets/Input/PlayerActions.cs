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
                    ""name"": ""Serve"",
                    ""type"": ""Button"",
                    ""id"": ""df7d98bb-fe55-44da-aadc-7d095e93430b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
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
                    ""expectedControlType"": ""Stick"",
                    ""processors"": ""StickDeadzone"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""63224bb0-ef7a-404c-90c6-096bc861e7d8"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Base"",
                    ""action"": ""Serve"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7fc6177b-1ac7-473a-83f8-4254636c65f9"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Base"",
                    ""action"": ""Serve"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9332f1df-fc1d-4722-ad89-70e30b510781"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Base"",
                    ""action"": ""Swing"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6337f693-a0d0-4922-9063-ee25f825504d"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Base"",
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
                    ""groups"": ""Base"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Base"",
            ""bindingGroup"": ""Base"",
            ""devices"": []
        }
    ]
}");
        // InGame
        m_InGame = asset.FindActionMap("InGame", throwIfNotFound: true);
        m_InGame_Serve = m_InGame.FindAction("Serve", throwIfNotFound: true);
        m_InGame_Swing = m_InGame.FindAction("Swing", throwIfNotFound: true);
        m_InGame_Aim = m_InGame.FindAction("Aim", throwIfNotFound: true);
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
    private readonly InputAction m_InGame_Serve;
    private readonly InputAction m_InGame_Swing;
    private readonly InputAction m_InGame_Aim;
    public struct InGameActions
    {
        private @PlayerActions m_Wrapper;
        public InGameActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Serve => m_Wrapper.m_InGame_Serve;
        public InputAction @Swing => m_Wrapper.m_InGame_Swing;
        public InputAction @Aim => m_Wrapper.m_InGame_Aim;
        public InputActionMap Get() { return m_Wrapper.m_InGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InGameActions set) { return set.Get(); }
        public void SetCallbacks(IInGameActions instance)
        {
            if (m_Wrapper.m_InGameActionsCallbackInterface != null)
            {
                @Serve.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnServe;
                @Serve.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnServe;
                @Serve.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnServe;
                @Swing.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnSwing;
                @Swing.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnSwing;
                @Swing.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnSwing;
                @Aim.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnAim;
            }
            m_Wrapper.m_InGameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Serve.started += instance.OnServe;
                @Serve.performed += instance.OnServe;
                @Serve.canceled += instance.OnServe;
                @Swing.started += instance.OnSwing;
                @Swing.performed += instance.OnSwing;
                @Swing.canceled += instance.OnSwing;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
            }
        }
    }
    public InGameActions @InGame => new InGameActions(this);
    private int m_BaseSchemeIndex = -1;
    public InputControlScheme BaseScheme
    {
        get
        {
            if (m_BaseSchemeIndex == -1) m_BaseSchemeIndex = asset.FindControlSchemeIndex("Base");
            return asset.controlSchemes[m_BaseSchemeIndex];
        }
    }
    public interface IInGameActions
    {
        void OnServe(InputAction.CallbackContext context);
        void OnSwing(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
    }
}
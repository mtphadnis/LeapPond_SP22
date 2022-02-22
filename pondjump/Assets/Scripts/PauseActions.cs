// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/PauseActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PauseActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PauseActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PauseActions"",
    ""maps"": [
        {
            ""name"": ""ui"",
            ""id"": ""be75edb4-c2aa-413f-980d-47129bd3d759"",
            ""actions"": [
                {
                    ""name"": ""PauseGame"",
                    ""type"": ""Button"",
                    ""id"": ""682e7013-15c3-460d-9108-491176475108"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e0ff1c93-4421-481b-8e1c-f0d66a87c384"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // ui
        m_ui = asset.FindActionMap("ui", throwIfNotFound: true);
        m_ui_PauseGame = m_ui.FindAction("PauseGame", throwIfNotFound: true);
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

    // ui
    private readonly InputActionMap m_ui;
    private IUiActions m_UiActionsCallbackInterface;
    private readonly InputAction m_ui_PauseGame;
    public struct UiActions
    {
        private @PauseActions m_Wrapper;
        public UiActions(@PauseActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @PauseGame => m_Wrapper.m_ui_PauseGame;
        public InputActionMap Get() { return m_Wrapper.m_ui; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UiActions set) { return set.Get(); }
        public void SetCallbacks(IUiActions instance)
        {
            if (m_Wrapper.m_UiActionsCallbackInterface != null)
            {
                @PauseGame.started -= m_Wrapper.m_UiActionsCallbackInterface.OnPauseGame;
                @PauseGame.performed -= m_Wrapper.m_UiActionsCallbackInterface.OnPauseGame;
                @PauseGame.canceled -= m_Wrapper.m_UiActionsCallbackInterface.OnPauseGame;
            }
            m_Wrapper.m_UiActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PauseGame.started += instance.OnPauseGame;
                @PauseGame.performed += instance.OnPauseGame;
                @PauseGame.canceled += instance.OnPauseGame;
            }
        }
    }
    public UiActions @ui => new UiActions(this);
    public interface IUiActions
    {
        void OnPauseGame(InputAction.CallbackContext context);
    }
}

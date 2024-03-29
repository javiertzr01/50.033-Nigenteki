//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Input System/PlayerControls.inputactions
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

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""7c10bbc6-ec8e-4c7a-8e74-4021a3c23eb3"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""c59c6842-cc68-453c-9a16-0d63b70dc5c0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""5405234d-b3b8-40b7-b59b-219134659faa"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Left Arm Basic Use"",
                    ""type"": ""Button"",
                    ""id"": ""8ffbdd5e-5fad-48fa-8cf2-95d531f7ef49"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Right Arm Basic Use"",
                    ""type"": ""Button"",
                    ""id"": ""d7a5549d-3858-4310-b16e-756566819a1b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Left Arm Skill Use"",
                    ""type"": ""Button"",
                    ""id"": ""f00a825c-870f-4283-b8be-5ac5a89a24f6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Right Arm Skill Use"",
                    ""type"": ""Button"",
                    ""id"": ""c1119d9f-fac0-4427-a6ec-1052e2bd4080"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Left Arm Ultimate Use"",
                    ""type"": ""Button"",
                    ""id"": ""fdac98a6-6c25-4cd3-b858-fbcbb723e6ff"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Right Arm Ultimate Use"",
                    ""type"": ""Button"",
                    ""id"": ""74fa379e-e007-4bd1-9196-5b39be7c12ed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Upgrade Right Arm"",
                    ""type"": ""Button"",
                    ""id"": ""8dc8e52f-31db-4ffd-96f5-0594e9eae84e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Upgrade Left Arm"",
                    ""type"": ""Button"",
                    ""id"": ""f2d8f355-25f3-408f-94d1-d749b53c4821"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""16942529-9fb4-44fb-8253-29f89279f958"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""65bd3023-1b8b-41d4-88c0-c3fe85dd83cf"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3732afac-b837-4dff-ad5d-bd287a2f6fd3"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b84bb991-a963-46b5-b4cb-08aeca6a91be"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7e3a7e93-c424-4bf4-876c-75f40700f47a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""bc7b16b1-f4ec-49ca-a20f-773d7e49f9e4"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left Arm Basic Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3dbd9105-ea0b-4257-ab3d-834f832333f3"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right Arm Basic Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03244df8-ff55-4d01-a9d1-136a6ba3f976"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a83c7f48-4a8d-4ec4-9f5e-d3b81dfc0534"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left Arm Skill Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2708c1d7-57c6-4754-9cc3-d5a049a0fd28"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right Arm Skill Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5a3f7fc8-9697-4a9c-a5ee-2f53280a221f"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left Arm Ultimate Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""50df4e67-2869-4942-a128-b425f597f99e"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right Arm Ultimate Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b8e44aa-d2f8-4008-9d4d-dd45aaab3e8d"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Upgrade Right Arm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bf4b6f85-3089-4281-92ce-0802b7050309"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Upgrade Left Arm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Stunned"",
            ""id"": ""2e260a36-18c2-498c-bc10-14ba6fdea6eb"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""33b9326e-ca90-43bf-9625-01ae7981e14c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""753e4180-092f-4d8c-b91c-cade8fedd8d3"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_LeftArmBasicUse = m_Player.FindAction("Left Arm Basic Use", throwIfNotFound: true);
        m_Player_RightArmBasicUse = m_Player.FindAction("Right Arm Basic Use", throwIfNotFound: true);
        m_Player_LeftArmSkillUse = m_Player.FindAction("Left Arm Skill Use", throwIfNotFound: true);
        m_Player_RightArmSkillUse = m_Player.FindAction("Right Arm Skill Use", throwIfNotFound: true);
        m_Player_LeftArmUltimateUse = m_Player.FindAction("Left Arm Ultimate Use", throwIfNotFound: true);
        m_Player_RightArmUltimateUse = m_Player.FindAction("Right Arm Ultimate Use", throwIfNotFound: true);
        m_Player_UpgradeRightArm = m_Player.FindAction("Upgrade Right Arm", throwIfNotFound: true);
        m_Player_UpgradeLeftArm = m_Player.FindAction("Upgrade Left Arm", throwIfNotFound: true);
        // Stunned
        m_Stunned = asset.FindActionMap("Stunned", throwIfNotFound: true);
        m_Stunned_Newaction = m_Stunned.FindAction("New action", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_LeftArmBasicUse;
    private readonly InputAction m_Player_RightArmBasicUse;
    private readonly InputAction m_Player_LeftArmSkillUse;
    private readonly InputAction m_Player_RightArmSkillUse;
    private readonly InputAction m_Player_LeftArmUltimateUse;
    private readonly InputAction m_Player_RightArmUltimateUse;
    private readonly InputAction m_Player_UpgradeRightArm;
    private readonly InputAction m_Player_UpgradeLeftArm;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @LeftArmBasicUse => m_Wrapper.m_Player_LeftArmBasicUse;
        public InputAction @RightArmBasicUse => m_Wrapper.m_Player_RightArmBasicUse;
        public InputAction @LeftArmSkillUse => m_Wrapper.m_Player_LeftArmSkillUse;
        public InputAction @RightArmSkillUse => m_Wrapper.m_Player_RightArmSkillUse;
        public InputAction @LeftArmUltimateUse => m_Wrapper.m_Player_LeftArmUltimateUse;
        public InputAction @RightArmUltimateUse => m_Wrapper.m_Player_RightArmUltimateUse;
        public InputAction @UpgradeRightArm => m_Wrapper.m_Player_UpgradeRightArm;
        public InputAction @UpgradeLeftArm => m_Wrapper.m_Player_UpgradeLeftArm;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @LeftArmBasicUse.started += instance.OnLeftArmBasicUse;
            @LeftArmBasicUse.performed += instance.OnLeftArmBasicUse;
            @LeftArmBasicUse.canceled += instance.OnLeftArmBasicUse;
            @RightArmBasicUse.started += instance.OnRightArmBasicUse;
            @RightArmBasicUse.performed += instance.OnRightArmBasicUse;
            @RightArmBasicUse.canceled += instance.OnRightArmBasicUse;
            @LeftArmSkillUse.started += instance.OnLeftArmSkillUse;
            @LeftArmSkillUse.performed += instance.OnLeftArmSkillUse;
            @LeftArmSkillUse.canceled += instance.OnLeftArmSkillUse;
            @RightArmSkillUse.started += instance.OnRightArmSkillUse;
            @RightArmSkillUse.performed += instance.OnRightArmSkillUse;
            @RightArmSkillUse.canceled += instance.OnRightArmSkillUse;
            @LeftArmUltimateUse.started += instance.OnLeftArmUltimateUse;
            @LeftArmUltimateUse.performed += instance.OnLeftArmUltimateUse;
            @LeftArmUltimateUse.canceled += instance.OnLeftArmUltimateUse;
            @RightArmUltimateUse.started += instance.OnRightArmUltimateUse;
            @RightArmUltimateUse.performed += instance.OnRightArmUltimateUse;
            @RightArmUltimateUse.canceled += instance.OnRightArmUltimateUse;
            @UpgradeRightArm.started += instance.OnUpgradeRightArm;
            @UpgradeRightArm.performed += instance.OnUpgradeRightArm;
            @UpgradeRightArm.canceled += instance.OnUpgradeRightArm;
            @UpgradeLeftArm.started += instance.OnUpgradeLeftArm;
            @UpgradeLeftArm.performed += instance.OnUpgradeLeftArm;
            @UpgradeLeftArm.canceled += instance.OnUpgradeLeftArm;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @LeftArmBasicUse.started -= instance.OnLeftArmBasicUse;
            @LeftArmBasicUse.performed -= instance.OnLeftArmBasicUse;
            @LeftArmBasicUse.canceled -= instance.OnLeftArmBasicUse;
            @RightArmBasicUse.started -= instance.OnRightArmBasicUse;
            @RightArmBasicUse.performed -= instance.OnRightArmBasicUse;
            @RightArmBasicUse.canceled -= instance.OnRightArmBasicUse;
            @LeftArmSkillUse.started -= instance.OnLeftArmSkillUse;
            @LeftArmSkillUse.performed -= instance.OnLeftArmSkillUse;
            @LeftArmSkillUse.canceled -= instance.OnLeftArmSkillUse;
            @RightArmSkillUse.started -= instance.OnRightArmSkillUse;
            @RightArmSkillUse.performed -= instance.OnRightArmSkillUse;
            @RightArmSkillUse.canceled -= instance.OnRightArmSkillUse;
            @LeftArmUltimateUse.started -= instance.OnLeftArmUltimateUse;
            @LeftArmUltimateUse.performed -= instance.OnLeftArmUltimateUse;
            @LeftArmUltimateUse.canceled -= instance.OnLeftArmUltimateUse;
            @RightArmUltimateUse.started -= instance.OnRightArmUltimateUse;
            @RightArmUltimateUse.performed -= instance.OnRightArmUltimateUse;
            @RightArmUltimateUse.canceled -= instance.OnRightArmUltimateUse;
            @UpgradeRightArm.started -= instance.OnUpgradeRightArm;
            @UpgradeRightArm.performed -= instance.OnUpgradeRightArm;
            @UpgradeRightArm.canceled -= instance.OnUpgradeRightArm;
            @UpgradeLeftArm.started -= instance.OnUpgradeLeftArm;
            @UpgradeLeftArm.performed -= instance.OnUpgradeLeftArm;
            @UpgradeLeftArm.canceled -= instance.OnUpgradeLeftArm;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Stunned
    private readonly InputActionMap m_Stunned;
    private List<IStunnedActions> m_StunnedActionsCallbackInterfaces = new List<IStunnedActions>();
    private readonly InputAction m_Stunned_Newaction;
    public struct StunnedActions
    {
        private @PlayerControls m_Wrapper;
        public StunnedActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Stunned_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Stunned; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(StunnedActions set) { return set.Get(); }
        public void AddCallbacks(IStunnedActions instance)
        {
            if (instance == null || m_Wrapper.m_StunnedActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_StunnedActionsCallbackInterfaces.Add(instance);
            @Newaction.started += instance.OnNewaction;
            @Newaction.performed += instance.OnNewaction;
            @Newaction.canceled += instance.OnNewaction;
        }

        private void UnregisterCallbacks(IStunnedActions instance)
        {
            @Newaction.started -= instance.OnNewaction;
            @Newaction.performed -= instance.OnNewaction;
            @Newaction.canceled -= instance.OnNewaction;
        }

        public void RemoveCallbacks(IStunnedActions instance)
        {
            if (m_Wrapper.m_StunnedActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IStunnedActions instance)
        {
            foreach (var item in m_Wrapper.m_StunnedActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_StunnedActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public StunnedActions @Stunned => new StunnedActions(this);
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnLeftArmBasicUse(InputAction.CallbackContext context);
        void OnRightArmBasicUse(InputAction.CallbackContext context);
        void OnLeftArmSkillUse(InputAction.CallbackContext context);
        void OnRightArmSkillUse(InputAction.CallbackContext context);
        void OnLeftArmUltimateUse(InputAction.CallbackContext context);
        void OnRightArmUltimateUse(InputAction.CallbackContext context);
        void OnUpgradeRightArm(InputAction.CallbackContext context);
        void OnUpgradeLeftArm(InputAction.CallbackContext context);
    }
    public interface IStunnedActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}

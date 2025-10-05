using Ivyyy.Network;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Ivyyy
{
    internal static class NetworkBootstrapper
    {
        static PlayerLoopSystem m_networkSystem;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            PlayerLoopSystem playerLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
            
            if (!InsertNetworkManager<PostLateUpdate> (ref playerLoopSystem, 0))
                Debug.LogWarning("Failed to register NetworkManager into PostLateUpdate loop.");

            PlayerLoop.SetPlayerLoop(playerLoopSystem);

            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlaymodeChanged;
            EditorApplication.playModeStateChanged += OnPlaymodeChanged;
            #endif
        }

        static bool InsertNetworkManager<T>(ref PlayerLoopSystem loop, int index)
        {
            m_networkSystem = new PlayerLoopSystem()
            {
                type = typeof(NetworkManager),
                updateDelegate = NetworkManager.NetworkSendData,
                subSystemList = null
            };

            return PlayerLoopUtils.InsertSystem<T>(ref loop, in m_networkSystem, index);
        }

        static void RemoveNetworkManager<T>(ref PlayerLoopSystem loop)
        {
            PlayerLoopUtils.RemoveSystem <T> (ref loop, in m_networkSystem);
        }

#if UNITY_EDITOR
        static void OnPlaymodeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                PlayerLoopSystem playerLoopSystem = PlayerLoop.GetCurrentPlayerLoop();
                RemoveNetworkManager<PostLateUpdate>(ref playerLoopSystem);
                PlayerLoop.SetPlayerLoop(playerLoopSystem);
            }
        }
#endif
    }
}

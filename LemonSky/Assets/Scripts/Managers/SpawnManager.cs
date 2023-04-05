using StarterAssets;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] List<SpawnZone> spawnZones;
    public static SpawnManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    [ClientRpc]
    public void SpawnPlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        SpawnPlayerServerRpc((int)PlayerInitializer.Instance.GetSafePlayerType(SelectCharacterManager.Instance.SelectedPlayer));
    }

    public Vector3 NextPosition()
    {
        return spawnZones[new System.Random().Next(spawnZones.Count)].NextRandomPosition();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(int playerType, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            Debug.Log($"Spawn ID {clientId}");

            var gayObject = Instantiate(
                PlayerInitializer.Instance.GetBasePlayerPrefab(),
                NextPosition(),
                Quaternion.identity
            );
            gayObject.GetComponent<ThirdPersonController>().SkinType.Value = playerType;
            gayObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
}

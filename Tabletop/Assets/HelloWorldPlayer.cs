using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Move();
            }
        }

        public void Move()
        {
            if (NetworkManager.Singleton.IsServer)
            { // If this program is the server...
                var randomPosition = GetRandomPositionOnPlane(); // Make random pos
                transform.position = randomPosition; // Server moves players to their Vector3
                Position.Value = randomPosition; // Server saves Position in Vector3
            }
            else
            { // If this is NOT the server...
                SubmitPositionRequestServerRpc(); // Ask the server to do it
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = GetRandomPositionOnPlane(); // Server picks random pos
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update()
        {
            transform.position = Position.Value; // Client moves players to their Vector3
        }
    }
}
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

[AddComponentMenu("Netcode/" + nameof(NetworkSpriteRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
public class NetworkSpriteRenderer : NetworkBehaviour {
    internal struct ObjectSpriteRenderer : INetworkSerializable {
        public float Sprite;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref Sprite);
        }
    }

    [SerializeField] private SpriteRenderer m_spriteRenderer;

    public override void OnNetworkSpawn() {
        var sprite = m_spriteRenderer.sprite;

    }
}
using System;
using EcsAnimation.Tools;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace EcsAnimation.Components
{
    [System.Serializable]
    struct AnimationRenderer : ISharedComponentData, IEquatable<AnimationRenderer>
    {
        public Mesh mesh;
        public Material material;
        public AnimationTextures AnimationTexture;
        public ShadowCastingMode castShadows;
        
        public bool receiveShadows;

        public bool Equals(AnimationRenderer other)
        {
            return material == other.material && AnimationTexture.Equals(other.AnimationTexture) &&
                   mesh == other.mesh && receiveShadows == other.receiveShadows && castShadows == other.castShadows;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ReferenceEquals(material, null) ? 0 : material.GetHashCode());
                hashCode = (hashCode * 397) ^ AnimationTexture.GetHashCode();
                hashCode = (hashCode * 397) ^ (ReferenceEquals(mesh, null) ? 0 : mesh.GetHashCode());
                return hashCode;
            }
        }
    }
}
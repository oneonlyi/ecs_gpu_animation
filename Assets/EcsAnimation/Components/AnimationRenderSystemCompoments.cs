using System;
using EcsAnimation.Tools;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsAnimation.Components
{
    [Serializable]
    public struct AnimationState : IComponentData
    {
        public float playTime;
        public int playClipIdx;

        public BlobAssetReference<BakedAnimationClipSet> AnimationClipSet;
    }

    public struct BakedAnimationClipSet
    {
        public BlobArray<AnimationClipBaker> Clips;
    }

    struct AnimationTextureCoordinate : IComponentData
    {
        public float3 Coordinate;
    }
}
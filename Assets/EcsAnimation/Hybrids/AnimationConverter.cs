using EcsAnimation.Components;
using EcsAnimation.Tools;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using AnimationState = EcsAnimation.Components.AnimationState;

namespace EcsAnimation.Hybrids
{
    public static partial class AnimationConverter
    {
        private static BlobAssetReference<BakedAnimationClipSet> CreateClipSet(AnimationTextureBaker.BakedData data)
        {
            using (var builder = new BlobBuilder(Allocator.Temp))
            {
                ref var root = ref builder.ConstructRoot<BakedAnimationClipSet>();
                var clips = builder.Allocate(ref root.Clips, data.Animations.Count);
                for (int i = 0; i != data.Animations.Count; i++)
                    clips[i] = new AnimationClipBaker(data.AnimationTextures, data.Animations[i]);

                return builder.CreateBlobAssetReference<BakedAnimationClipSet>(Allocator.Persistent);
            }
        }

        public static void AddComponents(EntityManager manager, Entity entity, GameObject characterRig,
            AnimationClip[] clips, float framerate)
        {
            var bakedData = AnimationTextureBaker.BakeClips(characterRig, clips, framerate);

            var animState = default(AnimationState);
            animState.AnimationClipSet = CreateClipSet(bakedData);
            manager.AddComponentData(entity, animState);
            manager.AddComponentData(entity, default(AnimationTextureCoordinate));

            var renderer = characterRig.GetComponentInChildren<SkinnedMeshRenderer>();
            var renderCharacter = new AnimationRenderer
            {
                material = renderer.sharedMaterial,
                AnimationTexture = bakedData.AnimationTextures,
                mesh = bakedData.NewMesh,
                receiveShadows = renderer.receiveShadows,
                castShadows = renderer.shadowCastingMode
            };
            manager.AddSharedComponentData(entity, renderCharacter);
        }
    }
}
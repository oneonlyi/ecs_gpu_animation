using System;
using System.Collections.Generic;
using EcsAnimation.Components;
using EcsAnimation.Tools;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;

namespace EcsAnimation.Systems
{
    public static unsafe class NativeExtensionTemp
    {
        public static NativeArray<U> Reinterpret_Temp<T, U>(this NativeArray<T> array) where U : struct where T : struct
        {
            var tSize = UnsafeUtility.SizeOf<T>();
            var uSize = UnsafeUtility.SizeOf<U>();

            var byteLen = ((long)array.Length) * tSize;
            var uLen = byteLen / uSize;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (uLen * uSize != byteLen)
            {
                throw new InvalidOperationException(
                    $"Types {typeof(T)} (array length {array.Length}) and {typeof(U)} cannot be aliased due to size constraints. The size of the types and lengths involved must line up.");
            }

#endif
            var ptr = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array);
            var result =
                NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<U>(ptr, (int)uLen, Allocator.Invalid);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var handle = NativeArrayUnsafeUtility.GetAtomicSafetyHandle(array);
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref result, handle);
#endif

            return result;
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(CalculateTextureCoordinateSystem))]
    public partial class GpuCharacterRenderSystem : SystemBase
    {
        private List<AnimationRenderer> _Characters = new List<AnimationRenderer>();

        private Dictionary<AnimationRenderer, DrawSkinning> _Drawers =
            new Dictionary<AnimationRenderer, DrawSkinning>();

        private EntityQuery m_Characters;


        protected override void OnUpdate()
        {
            _Characters.Clear();
            EntityManager.GetAllUniqueSharedComponentData(_Characters);

            foreach (var character in _Characters)
            {
                if (character.material == null || character.mesh == null)
                    continue;

                //@TODO: Currently we never cleanup the _Drawers cache when the last entity with that renderer disappears.
                DrawSkinning drawer;
                if (!_Drawers.TryGetValue(character, out drawer))
                {
                    drawer = new DrawSkinning(character.material, character.mesh,
                        character.AnimationTexture);
                    _Drawers.Add(character, drawer);
                }

                m_Characters.SetSharedComponentFilter(character);

                Profiler.BeginSample("ExtractState");

                var coords = m_Characters.ToComponentDataArray<AnimationTextureCoordinate>(Allocator.TempJob);
                var localToWorld = m_Characters.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

                Profiler.EndSample();

                drawer.Draw(coords.Reinterpret_Temp<AnimationTextureCoordinate, float3>(),
                    localToWorld.Reinterpret_Temp<LocalToWorld, float4x4>(), character.castShadows,
                    character.receiveShadows);

                coords.Dispose();
                localToWorld.Dispose();
            }
        }

        protected override void OnCreate()
        {
            m_Characters = GetEntityQuery(ComponentType.ReadOnly<AnimationRenderer>(),
                ComponentType.ReadOnly<AnimationState>(), ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<AnimationTextureCoordinate>());
        }

        protected override void OnDestroy()
        {
            foreach (var drawer in _Drawers.Values)
                drawer.Dispose();
            _Drawers = null;
        }
    }
}
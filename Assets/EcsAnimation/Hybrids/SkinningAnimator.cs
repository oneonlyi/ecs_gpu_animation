using EcsAnimation.Components;
using Unity.Entities;
using UnityEngine;

namespace EcsAnimation.Hybrids
{
    [RequireComponent(typeof(AnimationAuthoring))]
    public class SkinningAnimator : MonoBehaviour, IConvertGameObjectToEntity
    {
        public AnimationClip[] clips;
        public float framerate = 60.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            AnimationConverter.AddComponents(dstManager, entity, gameObject, clips, framerate);
        }
    }
}
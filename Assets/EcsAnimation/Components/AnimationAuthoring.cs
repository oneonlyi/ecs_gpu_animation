using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EcsAnimation.Components
{
    [DisallowMultipleComponent]
    public class AnimationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool randomizeStartTime = false;
        public bool isLoop = false;
        public int clipIndex = 0;
        public float speed = 1.0f;
        public Range SpeedRandomRange = new Range(1f, 1f);
      
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new AnimationInfo
            {
                RandomizeStartTime = randomizeStartTime,
                ClipIndex = clipIndex,
                Speed = speed,
                IsFirstFrame = true,
                IsLoop = isLoop,
                SpeedRandomRange = SpeedRandomRange
            });
        }
    }
}
using System;
using Unity.Entities;

namespace EcsAnimation.Components
{
    public struct AnimationInfo : IComponentData
    {
        public int ClipIndex;
        public float Speed;
        public bool IsLoop;
        public bool IsFirstFrame;
        public bool RandomizeStartTime;
        public Range SpeedRandomRange;
    }

    [Serializable]
    public struct Range
    {
        public Range(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }

        public float Min;
        public float Max;
    }
}
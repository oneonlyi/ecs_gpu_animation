using EcsAnimation.Components;
using Unity.Entities;

namespace EcsAnimation.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CalculateTextureCoordinateSystem : SystemBase
    {
        partial struct CalculateTextureCoordJob : IJobEntity
        {
            public void Execute(ref AnimationState animstate,
                ref AnimationTextureCoordinate textureCoordinate)
            {
                ref var clips = ref animstate.AnimationClipSet.Value.Clips;
                if ((uint)animstate.playClipIdx < (uint)clips.Length)
                {
                    var normalizedTime = clips[animstate.playClipIdx].ComputeNormalizedTime(animstate.playTime);
                    textureCoordinate.Coordinate =
                        clips[animstate.playClipIdx].ComputeCoordinate(normalizedTime);
                }
                else
                {
                    animstate.playClipIdx = 0;
                    animstate.playTime = 0f;
                }
            }
        }

        protected override void OnUpdate()
        {
            new CalculateTextureCoordJob().Schedule();
        }
    }
}
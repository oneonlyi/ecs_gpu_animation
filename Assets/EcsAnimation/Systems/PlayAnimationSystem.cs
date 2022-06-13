using EcsAnimation.Components;
using Unity.Entities;

namespace EcsAnimation.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(CalculateTextureCoordinateSystem))]
    public partial class PlayAnimationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach(
                (Entity entity, int entityInQueryIndex, ref AnimationInfo info,
                    ref AnimationState animstate) =>
                {
                    animstate.playClipIdx = info.ClipIndex;

                    ref var clips = ref animstate.AnimationClipSet.Value.Clips;
                    if ((uint)animstate.playClipIdx < (uint)clips.Length)
                    {
                        var clip = animstate.AnimationClipSet.Value.Clips[animstate.playClipIdx];
                        if ((animstate.playTime + 0.2f) < clip.AnimationLength)
                        {
                            if (!info.IsFirstFrame)
                            {
                                animstate.playTime += deltaTime * info.Speed;
                            }
                            else
                            {
                                var length = 10.0F;

                                var random = new Unity.Mathematics.Random((uint)entityInQueryIndex + 1);
                                random.NextInt();

                                if (info.RandomizeStartTime)
                                    animstate.playTime = random.NextFloat(0, length);

                                info.Speed = random.NextFloat(
                                    info.SpeedRandomRange.Min,
                                    info.SpeedRandomRange.Max);

                                info.IsFirstFrame = false;
                            }
                        }
                        else
                        {
                            if (info.IsLoop)
                            {
                                animstate.playTime = 0f;
                            }
                        }
                    }
                }).Schedule();
        }
    }
}
using EcsAnimation.Components;
using Unity.Entities;
using Unity.Mathematics;
using UtilityAI.Components;

namespace UtilityAI.Systems.ActionsGroup
{
    public partial class IdleActionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities.ForEach((ref Enegry enegry,
                ref Tiredness tired, ref AnimationInfo anim,
                in IdleAction idleAct) =>
            {
                anim.ClipIndex = (int)ActionType.Idle;
                anim.IsLoop = true;
                
                tired.Value = math.clamp(
                    tired.Value - idleAct.TirednessCostPerSecond * deltaTime, 0f, 100f);
            }).ScheduleParallel();
        }
    }
}
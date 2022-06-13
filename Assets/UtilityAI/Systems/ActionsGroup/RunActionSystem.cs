using EcsAnimation.Components;
using Unity.Entities;
using Unity.Mathematics;
using UtilityAI.Components;

namespace UtilityAI.Systems.ActionsGroup
{
    public partial class RunActionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities.ForEach((ref Enegry enegry,
                ref Tiredness tired, ref AnimationInfo anim,
                in RunAction runAct) =>
            {
                anim.ClipIndex = (int)ActionType.Run;
                anim.IsLoop = true;

                enegry.Value = math.clamp(
                    enegry.Value - runAct.EnegryCostPerSecond * deltaTime, 0f, 100f);
                tired.Value = math.clamp(
                    tired.Value + runAct.TirednessCostPerSecond * deltaTime, 0f, 100f);

                if (runAct.TirednessCostPerSecond <= 0f)
                    enegry.Value = math.clamp(enegry.Value - 10 * deltaTime, 0f, 100f);
            }).ScheduleParallel();
        }
    }
}
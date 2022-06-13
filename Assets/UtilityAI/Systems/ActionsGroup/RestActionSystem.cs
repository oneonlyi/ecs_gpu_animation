using EcsAnimation.Components;
using Unity.Entities;
using Unity.Mathematics;
using UtilityAI.Components;

namespace UtilityAI.Systems.ActionsGroup
{
    public partial class RestActionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities.ForEach((ref Tiredness tired,
                ref Enegry enegry, ref AnimationInfo anim,
                in RestAction restAct) =>
            {
                anim.ClipIndex = (int)ActionType.Rest;
                anim.IsLoop = false;

                enegry.Value = math.clamp(
                    enegry.Value + restAct.EnegryRecoverPerSecond * deltaTime, 0f, 100f);
            }).ScheduleParallel();
        }
    }
}
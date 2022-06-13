using Unity.Entities;
using Unity.Mathematics;
using UtilityAI.Components;

namespace UtilityAI.Systems.LogicGroup
{
    public partial class CalActionScoreSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref IdleScore idleScore,
                in Tiredness tired, in Enegry enegry,
                in CurrentAction decision) =>
            {
                if (decision.Action == ActionType.Idle)
                {
                    idleScore.Score = tired.Value <= float.Epsilon ? 0f : 1f;
                    if (enegry.Value <= 0f)
                    {
                        idleScore.Score = 0f;
                    }
                }
                else
                {
                    var input = math.clamp(tired.Value * 0.01f, 0f, 1f);
                    idleScore.Score = ResponseCurve.RaiseFastToSlow(input, 2f);
                }
            }).ScheduleParallel();

            Entities.ForEach((ref RestScore restScore,
                in Enegry enegry,
                in CurrentAction decision) =>
            {
                if (decision.Action == ActionType.Rest)
                {
                    var enegryValue = ResponseCurve.RaiseFastToSlow(math.clamp(enegry.Value * 0.01f, 0f, 1f));
                    restScore.Score = math.clamp(1f - enegryValue, 0f, 1f);
                }
                else
                {
                    var input = math.clamp(enegry.Value * 0.01f, 0f, 1f);
                    restScore.Score = ResponseCurve.RaiseFastToSlow(input, 4);
                    if (enegry.Value <= 0f)
                    {
                        restScore.Score = 1f;
                    }
                }
            }).ScheduleParallel();

            Entities.ForEach((ref RunScore runScore,
                in Tiredness tired,
                in Enegry enegry) =>
            {
                var enegryValue = ResponseCurve.Exponentional(math.clamp(enegry.Value * 0.01f, 0f, 1f));
                var restValue = ResponseCurve.RaiseFastToSlow(math.clamp(tired.Value * 0.01f, 0f, 1f));

                var concernBothersPlaying = enegryValue * 0.4f + restValue * 0.6f;

                runScore.Score = math.clamp(1f - concernBothersPlaying, 0f, 1f);
            }).ScheduleParallel();

            this.CompleteDependency();
        }
    }
}
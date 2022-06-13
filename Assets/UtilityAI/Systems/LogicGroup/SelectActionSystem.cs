using Unity.Entities;
using UtilityAI.Components;

namespace UtilityAI.Systems.LogicGroup
{
    public partial class SelectActionSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
            Entities.ForEach((Entity entity,
                int entityInQueryIndex,
                ref CurrentAction decision,
                in IdleScore idleScore,
                in RestScore restScore,
                in RunScore runScore) =>
            {
                float highestScore = 0.0f;
                ActionType actionToDo = ActionType.Run;
                if (idleScore.Score > highestScore)
                {
                    highestScore = idleScore.Score;
                    actionToDo = ActionType.Idle;
                }

                if (restScore.Score > highestScore)
                {
                    highestScore = restScore.Score;
                    actionToDo = ActionType.Rest;
                }

                if (runScore.Score > highestScore)
                {
                    highestScore = runScore.Score;
                    actionToDo = ActionType.Run;
                }

                if (decision.Action != actionToDo)
                {
                    decision.Action = actionToDo;

                    switch (actionToDo)
                    {
                        case ActionType.Idle:
                            ecb.RemoveComponent<RestAction>(entityInQueryIndex, entity);
                            ecb.RemoveComponent<RunAction>(entityInQueryIndex, entity);

                            ecb.AddComponent<IdleAction>(entityInQueryIndex, entity);
                            ecb.SetComponent(entityInQueryIndex, entity, new IdleAction()
                            {
                                TirednessCostPerSecond = 2.0f
                            });
                            break;
                        case ActionType.Rest:
                            ecb.RemoveComponent<IdleAction>(entityInQueryIndex, entity);
                            ecb.RemoveComponent<RunAction>(entityInQueryIndex, entity);

                            ecb.AddComponent<RestAction>(entityInQueryIndex, entity);
                            ecb.SetComponent(entityInQueryIndex, entity, new RestAction()
                            {
                                TirednessRecoverPerSecond = 3.0f,
                                EnegryRecoverPerSecond = 2.0f
                            });
                            break;
                        case ActionType.Run:
                            ecb.RemoveComponent<IdleAction>(entityInQueryIndex, entity);
                            ecb.RemoveComponent<RestAction>(entityInQueryIndex, entity);

                            ecb.AddComponent<RunAction>(entityInQueryIndex, entity);
                            ecb.SetComponent(entityInQueryIndex, entity, new RunAction()
                            {
                                EnegryCostPerSecond = 2.0f,
                                TirednessCostPerSecond = 4.0f
                            });
                            break;
                    }
                }
            }).ScheduleParallel();

            _endSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}
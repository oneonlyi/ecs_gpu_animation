using Formation.ComponentData;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Formation.Systems
{
    public partial class CircleFormationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            const float twoPi = 2 * math.PI;
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref Translation translation,
                in FormationIdxData formationIdx) =>
            {
                var index = formationIdx.idx;
                var entityCount = formationIdx.totalCnt;
                var radius = formationIdx.distance * entityCount / twoPi;
                var radians = twoPi * index / entityCount;
                var x = math.sin(radians) * radius;
                var z = math.cos(radians) * radius;

                translation.Value = new float3(x * formationIdx.distance, 0f, -z * formationIdx.distance);
            }).Schedule();
        }
    }
}
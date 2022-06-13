using Formation.ComponentData;
using Formation.Tags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Formation.Systems
{
    public partial class SquareFormationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            const float twoPi = 2 * math.PI;
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref Translation translation,
                in FormationIdxData formationIdx, in SquareFormation formation) =>
            {
                var index = formationIdx.idx;
                float midX = (formationIdx.gridWidth - 1) / 2f;
                var x = index % formationIdx.gridWidth - midX;
                var z = index / formationIdx.gridWidth + 1;

                translation.Value = new float3(x * formationIdx.distance, 0f, -z * formationIdx.distance);
            }).Schedule();
        }
    }
}
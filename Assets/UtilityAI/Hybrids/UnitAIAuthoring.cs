using Unity.Entities;
using UnityEngine;
using UtilityAI.Components;

namespace UtilityAI.Hybrids
{
    [DisallowMultipleComponent]
    public class UnitAIAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent(entity, ComponentType.ReadOnly<SoilderUnitTag>());
            dstManager.AddComponentData(entity, new Enegry { Value = Random.Range(20f, 30f) });
            dstManager.AddComponentData(entity, new Tiredness { Value =  Random.Range(0f, 20f) });
            dstManager.AddComponentData(entity, new CurrentAction{ Action = ActionType.Idle });

            dstManager.AddComponent(entity, ComponentType.ReadWrite<IdleScore>()); 
            dstManager.AddComponent(entity, ComponentType.ReadWrite<RestScore>());
            dstManager.AddComponent(entity, ComponentType.ReadWrite<RunScore>());
        }
    }
}
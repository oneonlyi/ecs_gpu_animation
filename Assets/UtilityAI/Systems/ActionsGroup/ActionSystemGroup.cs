using Unity.Entities;

namespace UtilityAI.Systems.ActionsGroup
{
    class ActionSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            AddSystemToUpdateList(World.CreateSystem<IdleActionSystem>());
            AddSystemToUpdateList(World.CreateSystem<RestActionSystem>());
            AddSystemToUpdateList(World.CreateSystem<RunActionSystem>());
        }
    }

}
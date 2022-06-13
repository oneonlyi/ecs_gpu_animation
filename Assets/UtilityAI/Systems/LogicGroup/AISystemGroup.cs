using Unity.Entities;

namespace UtilityAI.Systems.LogicGroup
{
    public class AISystemGroup : ComponentSystemGroup
    {
        private const float AIUpdateFrequancy = 1f; 
        private readonly float _aiUpdateInterval = 1f / AIUpdateFrequancy;
        private float _aiUpdateCd = 0f;

        protected override void OnCreate()
        {
            base.OnCreate();

            AddSystemToUpdateList(World.CreateSystem<CalActionScoreSystem>());
            AddSystemToUpdateList(World.CreateSystem<SelectActionSystem>());
        }

        protected override void OnUpdate()
        {
            if (_aiUpdateCd <= 0.0f)
            {
                _aiUpdateCd += _aiUpdateInterval;

                base.OnUpdate();
            }

            _aiUpdateCd -= Time.DeltaTime;
        }
    }
}
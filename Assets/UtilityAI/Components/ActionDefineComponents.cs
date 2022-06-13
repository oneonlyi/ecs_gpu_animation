using Unity.Entities;

namespace UtilityAI.Components
{
    enum ActionType
    {
        Idle,
        Rest,
        Run,
    }

    struct RunAction : IComponentData
    {
        public float TirednessCostPerSecond;
        public float EnegryCostPerSecond;
    }
    
    struct RestAction : IComponentData
    {
        public float TirednessRecoverPerSecond;
        public float EnegryRecoverPerSecond;
    }
    
    struct IdleAction : IComponentData
    {
        public float TirednessCostPerSecond;
    }
    
    struct RunScore : IComponentData { public float Score;}
    struct RestScore : IComponentData { public float Score;}
    struct IdleScore : IComponentData { public float Score;}
    
    struct Tiredness : IComponentData
    {
        public float Value; // 0: not tired, 100: tired to death
    }
    
    struct Enegry : IComponentData
    {
        public float Value; // 0: not tired, 100: tired to death
    }
    
    struct CurrentAction : IComponentData
    {
        public ActionType Action; // current action to perform
        
    }
}
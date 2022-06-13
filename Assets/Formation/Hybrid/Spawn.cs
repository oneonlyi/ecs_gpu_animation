using Formation.ComponentData;
using Formation.Tags;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Formation.Hybrid
{
    public class Spawn : MonoBehaviour
    {
    
        public GameObject soldierPrefab;
        public int gridWidth;
        public int gridHeight;
        public float distance = 1f;

        private EntityManager _entityManager;
        private World _world;

        void Awake()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;
        }
    
        // Start is called before the first frame update
        void Start()
        {
            var settings = GameObjectConversionSettings.FromWorld(_world, null);
            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(soldierPrefab, settings);
        
            _entityManager.AddComponentData(entity, new SquareFormation());
        
            int totalCnt = gridHeight * gridHeight;
            var entities = new NativeArray<Entity>(totalCnt, Allocator.Temp);
            _entityManager.Instantiate(entity, entities);
        
            for (int i = 0; i < totalCnt; i++)
            {
                _entityManager.AddComponentData(entities[i], new FormationIdxData
                {
                    idx = i, totalCnt = totalCnt, gridHeight = this.gridHeight, gridWidth = this.gridWidth, distance = this.distance
                });
            }
        
            entities.Dispose();
            _entityManager.DestroyEntity(entity);
        }
    }
}


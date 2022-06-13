using System;
using Unity.Entities;

namespace Formation.ComponentData
{
   [Serializable]
   public struct FormationIdxData : IComponentData
   {
      public int idx;
      public int totalCnt;
      public int gridWidth;
      public int gridHeight;
      public float distance;

   }
}

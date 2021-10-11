using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PinkBlob
{
    public static class AgentUtil
    {
        private const int BaseEnemyId = 0;

        public static int AgentTypeToId(AgentType type)
        {
            return type switch
                   {
                       AgentType.BaseEnemy => BaseEnemyId,
                       _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                   };
        }
    }
}

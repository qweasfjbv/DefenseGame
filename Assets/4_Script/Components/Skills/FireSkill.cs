using System.Collections.Generic;
using UnityEngine;


namespace Defense.Components
{
    public class FireSkill : Skillable
    {
        public override void ExecuteSkill(List<Transform> targets, int targetCount)
        {
            Debug.Log("파이어~~~~~~~~~~~~~~~");
        }
    }

}

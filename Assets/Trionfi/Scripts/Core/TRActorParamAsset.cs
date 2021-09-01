using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Trionfi
{
    [CreateAssetMenu(menuName = "Trionfi/ScriptableObject/Createa ActorParamAsset")]
    public class TRActorParamAsset : ScriptableObject
    {
        public TRActorInfoes actorInfo = new TRActorInfoes();
    }
}

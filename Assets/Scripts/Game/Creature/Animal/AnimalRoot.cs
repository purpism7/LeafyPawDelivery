using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creature
{
    public class AnimalRoot : MonoBehaviour
    {
        public Transform RewardRootTm = null;
        [SerializeField] private RectTransform editRootRectTm = null;
        [SerializeField] private RectTransform speechBubbleRootRectTm = null;

        public RectTransform EditRootRectTm { get { return editRootRectTm; } }
    }
}


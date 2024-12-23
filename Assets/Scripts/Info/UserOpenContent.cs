using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Type = Game.Type;

namespace Info
{
    public partial class User
    {
        [Serializable]
        public class OpenContent
        {
            public Type.EContent eContent = Type.EContent.None;
            public bool isOpen = false;
        }

        [SerializeField] 
        private List<OpenContent> openContentList = null;

        public void SetOpen(Type.EContent eContent)
        {
            if (openContentList == null)
            {
                openContentList = new();
                openContentList.Clear();
            }
            
            int findIndex = openContentList.FindIndex(openContent => openContent?.eContent == eContent);
            if (findIndex >= 0)
                openContentList[findIndex].isOpen = true;
            else
            {
                openContentList.Add(
                    new OpenContent
                    {
                        eContent = eContent,
                        isOpen = true,
                    });
            }
        }

        public bool CheckOpen(Type.EContent eContent)
        {
            if (openContentList == null)
                return false;
            
            int findIndex = openContentList.FindIndex(openContent => openContent?.eContent == eContent);
            if (findIndex >= 0)
                return openContentList[findIndex].isOpen;
            else
            {
                openContentList.Add(
                    new OpenContent
                    {
                        eContent = eContent,
                        isOpen = false,
                    });
            }

            return false;
        }
    }
}

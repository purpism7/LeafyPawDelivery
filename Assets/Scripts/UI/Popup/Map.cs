using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Map : BasePopup<Map.Data>, MapIcon.IListener
    {
        public class Data : BaseData
        {

        }

        [SerializeField]
        private MapIcon[] mapIcons = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            InternalInitialize();
        }
        
        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            InternalInitialize();
            
            yield break;
        }

        private void InternalInitialize()
        {
            SetMapIcons();
        }

        private void SetMapIcons()
        {
            if (mapIcons == null)
                return;

            foreach(var mapIcon in mapIcons)
            {
                if (mapIcon == null)
                    continue;

                mapIcon?.Initialize(new MapIcon.Data()
                {
                    IListener = this,
                });
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        // public void OnChanged(string tabType)
        // {
        //     if(System.Enum.TryParse(tabType, out Type.ETab eTabType))
        //     {
        //         if(_currETabType == eTabType)
        //         {
        //             return;
        //         }
        //
        //         _currETabType = eTabType;
        //
        //         ActiveContents();
        //     }
        // }

        #region MapIcon.IListener
        void MapIcon.IListener.SelectPlace(int id)
        {
            Deactivate();

            MainGameManager.Instance?.MovePlace(id);
        }
        #endregion
    }
}
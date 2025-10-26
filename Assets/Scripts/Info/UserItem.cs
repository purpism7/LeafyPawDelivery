using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public partial class User
    {
        [Serializable]
        public class Item
        {
            [SerializeField] private int id = 0;
            [SerializeField] private int count = 0;

            public int Id
            {
                get { return id; }
            }

            public Item(int id)
            {
                this.id = id;
            }

            public Item AddCount(int cnt)
            {
                count += cnt;

                return this;
            }
        }

        [SerializeField] private List<Item> itemList = new();

        public List<Item> ItemList {
            get
            {
                return itemList;
            }
        }

        public void AddItem(int id, int count)
        {
            if (itemList == null)
            {
                itemList = new();
                itemList?.Clear();
            }
            
            foreach (var item in itemList)
            {
                if (item == null)
                    continue;

                if (item.Id == id)
                {
                    item.AddCount(count);

                    return;
                }
            }
            
            itemList?.Add(new Item(id).AddCount(count));
        }
        
        public Item GetItem(int id)
        {
            if (itemList == null)
                return null;

            return itemList.Find(item => item.Id == id);
        }
    }
}

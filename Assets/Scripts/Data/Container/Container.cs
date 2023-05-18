using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Data
{
    public class Container : GameSystem.Processing
    {
        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            yield return StartCoroutine(CoLoadData());

            Debug.Log(GetType().Name);

            yield return null;
        }

        //public void AddAnimaData(Animal addAnimalData)
        //{
        //    if(addAnimalData == null)
        //    {
        //        return;
        //    }

        //    foreach(var animalData in AnimalDataList)
        //    {              
        //        if(animalData == null)
        //        {
        //            continue;
        //        }

        //        if(animalData.Id == addAnimalData.Id)
        //        {
        //            return;
        //        }
        //    }

        //    AnimalDataList.Add(addAnimalData);
        //}

        //public Animal GetAnimal(int id)
        //{
        //    return AnimalDataList?.Find(data => data?.Id == id);
        //}

        private IEnumerator CoLoadData()
        {
            var addressableAssetLoader = GameSystem.ResourceManager.Instance?.AddressableAssetLoader;
            if(addressableAssetLoader == null)
            {
                yield break;
            }

            yield return addressableAssetLoader.CoLoadAssetAsync<TextAsset>(addressableAssetLoader.AssetLabelData,
                (asyncOperationHandle) =>
                {
                    var result = asyncOperationHandle.Result;
                    if(result == null)
                    {
                        return;
                    }

                    var typeName = addressableAssetLoader.AssetLabelData + "." + asyncOperationHandle.Result.name + "Container";
                    var type = System.Type.GetType(typeName);

                    if(type != null)
                    {
                        var container = System.Activator.CreateInstance(type) as GameData.BaseContainer;
                        container?.Init(result.text);

                        Debug.Log(typeName);
                    }
                });

            yield return null;
        }
    }
}


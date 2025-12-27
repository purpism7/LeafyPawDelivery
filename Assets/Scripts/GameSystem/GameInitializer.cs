using UnityEngine;

namespace GameSystem
{
    public class GameInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            Debug.Log("앱 실행: 씬 로드 전 초기화 시작");
            // 여기서 GardenManager 등을 생성하거나 의존성 설정을 준비할 수 있습니다.
        }

        // 첫 씬이 로드된 직후에 실행됨
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Debug.Log("앱 실행: 씬 로드 완료");
        }
    }
}


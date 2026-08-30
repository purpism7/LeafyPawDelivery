# Leafy Paw Parcels

동물 수집, 장소 꾸미기, 선물/친밀도, 작물, 짧은 스토리 진행을 중심으로 한 Unity 모바일 캐주얼 게임 프로젝트입니다.

## Project Info

- Engine: Unity 6000.3.7f1
- Product Name: leafypawdelivery
- Company: popo
- Version: 3.0.1
- Android Package: com.popo.leafyparcels
- iOS Bundle ID: com.popo.leafy-paw-delivery
- Main Target: Android / iOS

## Core Gameplay

- 동물 해금 및 스킨 수집
- 오브젝트 배치와 장소 꾸미기
- 장소별 스토리/컷신 진행
- 선물 지급과 동물 친밀도 시스템
- 작물 재배 및 수확
- 일일 미션, 업적, 보상 수령
- 광고 보상, 부스트, 인앱 결제 상점

## Main Scenes

Build Settings 기준:

- `Assets/Scenes/BeginScene.unity`
- `Assets/Scenes/GameScene.unity`

현재 Build Settings에서는 `BeginScene`이 활성화되어 있습니다.

## Important Folders

- `Assets/Scripts/Game`: 게임 플레이, 동물, 오브젝트, 장소, 스토리, 미션 관련 코드
- `Assets/Scripts/GameSystem`: 공통 시스템, 입력, 리소스, 광고, Firebase 관련 코드
- `Assets/Scripts/UI`: 팝업, 상점, 꾸미기, 도감, 상단/하단 UI 코드
- `Assets/Data`: 동물, 오브젝트, 상점, 스토리, 미션 등 정적 데이터
- `Assets/ScriptableObject/Data`: 장소, 광고 보상, 부스트, 선물 등 ScriptableObject 데이터
- `Assets/Prefabs/Game`: 동물, 오브젝트, 장소, 드랍 아이템 프리팹
- `Assets/Res`: UI, 동물, 오브젝트, 스토리, 사운드 리소스
- `Assets/AddressableAssetsData`: Addressables 설정과 그룹

## Major Packages

- Addressables
- Localization
- Cinemachine
- UniTask
- DOTween
- Unity Purchasing
- Unity Analytics
- Unity Cloud Save / Authentication
- Google Mobile Ads Unity Plugin
- Google Play Games

## Android Build Notes

- Android SDK/NDK/JDK는 Unity Hub의 Android Build Support 구성을 사용합니다.
- Android App Bundle 또는 APK 빌드 전 Addressables 설정과 Android 플랫폼 번들을 확인해야 합니다.
- AdMob 앱 ID와 광고 단위 ID는 프로젝트에 설정되어 있습니다.
- 내부 테스트 중에는 실제 광고 단위 ID 반복 호출 대신 Google 테스트 광고 ID 또는 테스트 디바이스 등록을 사용하는 것을 권장합니다.
- Firebase Analytics/Crashlytics는 코드 흔적이 있으나 현재 활성화 상태는 별도 확인이 필요합니다.

## QA Checklist

- Game View와 Device Simulator 양쪽에서 월드 터치/드래그 입력 확인
- 동물/오브젝트 배치, 회수, 재배치 확인
- 일일 미션과 업적 보상 수령 확인
- 광고 보상 로드, 시청, 보상 콜백 확인
- IAP 상품 로드와 구매 실패/성공 콜백 확인
- 신규 설치, 기존 저장 데이터, 데모 데이터 상태 확인
- Android 실기기에서 크래시/강제 종료 로그 확인

## Demo / Publisher Build Notes

외부 전달용 빌드는 모든 콘텐츠를 열어둔 데모 데이터로 구성하는 것이 좋습니다.

- 모든 동물과 스킨 보유
- 모든 장소와 스토리 오픈
- 주요 꾸미기 오브젝트 보유
- 충분한 동물/오브젝트 재화와 보석 지급
- 광고/IAP는 실제 결제 없이 테스트 가능한 상태로 분리
- 데모 빌드임을 식별할 수 있는 버전명 또는 내부 플래그 적용

## Repository Notes

- `UserSettings/` 아래 Unity 에디터 레이아웃 파일은 개인 환경 설정이므로 일반적으로 커밋하지 않습니다.
- 큰 리소스 변경 전에는 Addressables 그룹과 빌드 결과물을 함께 확인합니다.

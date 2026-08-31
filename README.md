# Leafy Paw Delivery

Leafy Paw Delivery는 숲속 우체부와 동물 친구들이 머무는 공간을 꾸미고, 선물과 작물을 통해 친밀도와 이야기를 열어가는 Unity 모바일 힐링 수집 게임입니다.

플레이어는 동물을 수집하고, 장소별 오브젝트를 배치하며, 짧은 스토리와 보상 루프를 통해 자신만의 숲속 배달 마을을 완성해 나갑니다.

<p align="center">
  <img src="docs/images/leafy-paw-delivery-overview.png" alt="Leafy Paw Delivery gameplay, story, collection, and mission screens" width="900" />
</p>

## Overview

- Genre: Mobile casual, collection, decoration, idle reward
- Platform: Android / iOS
- Engine: Unity 6000.3.7f1
- Product Name: leafypawdelivery
- Version: 3.0.1
- Android Package: com.popo.leafyparcels
- iOS Bundle ID: com.popo.leafy-paw-delivery

## Game Loop

1. 동물과 장소를 해금합니다.
2. 오브젝트와 작물로 공간을 꾸밉니다.
3. 동물에게 선물을 주고 친밀도를 올립니다.
4. 재화와 보상을 모아 새로운 콘텐츠를 엽니다.
5. 장소별 스토리와 컷신을 감상하며 진행감을 얻습니다.

## Key Features

- Animal Collection: 장소별 동물 해금, 동물 스킨 수집, 동물 상호작용
- Decoration: 오브젝트 배치, 꾸미기 편집, 숨겨진 오브젝트 발견
- Friendship: 선물 지급, 친밀도 기반 보상, 동물별 감성 상호작용
- Story: 장소별 스토리, 컷신, 대화 연출
- Garden: 작물 배치, 성장, 수확 기반 보상 루프
- Mission: 일일 미션과 업적 보상
- Monetization: 보상형 광고, 부스트, 인앱 결제 상점

## Implemented Systems

- Data-driven content using text data and ScriptableObject assets
- Addressables-based asset grouping for animals, objects, places, UI, stories, and localization
- Manager structure for animals, objects, places, garden, story, missions, UI, input, ads, and IAP
- Mobile touch input with editor Game View mouse input support
- Popup-based UI flow for shop, book, missions, map, boost, arrangement, and settings
- Localization tables for Korean, English, and Japanese
- Google Mobile Ads rewarded ad integration
- Unity Purchasing integration

## Technical Stack

- Unity 6000.3.7f1
- Universal Render Pipeline
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

## Project Structure

- `Assets/Scripts/Game`: gameplay systems for animals, objects, places, story, garden, missions, and rewards
- `Assets/Scripts/GameSystem`: common systems such as input, resources, ads, Firebase hooks, grid, and utilities
- `Assets/Scripts/UI`: popup, shop, arrangement, book, mission, top/bottom UI, and shared UI components
- `Assets/Data`: static data for animals, objects, shops, stories, missions, crops, and open conditions
- `Assets/ScriptableObject/Data`: place, boost, rewarded ad, and gift configuration assets
- `Assets/Prefabs/Game`: animal, object, place, drop item, and gameplay prefabs
- `Assets/Res`: UI, story, animal, object, sound, currency, and cutscene resources
- `Assets/AddressableAssetsData`: Addressables settings and asset groups

## Main Scenes

Build Settings 기준:

- `Assets/Scenes/BeginScene.unity`
- `Assets/Scenes/GameScene.unity`

현재 Build Settings에서는 `BeginScene`이 활성화되어 있습니다.

## Build Notes

- Android SDK/NDK/JDK는 Unity Hub의 Android Build Support 구성을 사용합니다.
- Android App Bundle 또는 APK 빌드 전 Addressables Android 번들 상태를 확인해야 합니다.
- 내부 테스트 중에는 실제 AdMob 광고 단위 ID 반복 호출 대신 Google 테스트 광고 ID 또는 테스트 디바이스 등록을 권장합니다.
- 외부 전달용 데모 빌드는 모든 동물, 스킨, 장소, 스토리, 주요 꾸미기 오브젝트가 열린 상태로 구성하는 것이 좋습니다.

## QA Checklist

- Game View와 Device Simulator 양쪽에서 월드 터치/드래그 입력 확인
- 동물/오브젝트 배치, 회수, 재배치 확인
- 작물 성장/수확과 재화 지급 확인
- 일일 미션과 업적 보상 수령 확인
- 광고 보상 로드, 시청, 보상 콜백 확인
- IAP 상품 로드와 구매 성공/실패 콜백 확인
- 신규 설치, 기존 저장 데이터, 데모 데이터 상태 확인
- Android 실기기에서 크래시/강제 종료 로그 확인

## Future Improvements

- 오늘의 소포: 매일 맵에 등장하는 랜덤 편지/소포 이벤트
- 길 잃은 동물 방문: 보유하지 않은 동물이 잠시 방문하는 짧은 이벤트
- 테마 컬렉션 앨범: 동물, 스킨, 꾸미기, 작물을 세트로 묶는 장기 목표
- 2주 시즌 이벤트: 기존 리소스를 활용한 이벤트 재화와 보상 트랙
- Firebase Analytics / Crashlytics: 유저 행동, 광고, 구매, 크래시 분석 강화

## Repository Notes

- `UserSettings/` 아래 Unity 에디터 레이아웃 파일은 개인 환경 설정이므로 일반적으로 커밋하지 않습니다.
- 큰 리소스 변경 전에는 Addressables 그룹과 빌드 결과물을 함께 확인합니다.

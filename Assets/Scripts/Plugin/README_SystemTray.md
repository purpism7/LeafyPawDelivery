# 시스템 트레이/메뉴바 인터랙션 가이드

## 개요
이 프로젝트는 Windows와 macOS에서 시스템 트레이/메뉴바를 통한 인터랙션 게임을 지원합니다.

## 기능

### Windows
- 시스템 트레이로 윈도우 최소화
- 백그라운드 실행 지원
- 윈도우 표시/숨김 제어

### macOS
- 메뉴바 아이콘 표시
- 메뉴바를 통한 인터랙션
- 윈도우 표시/숨김 제어

## 사용 방법

### 기본 사용
```csharp
// 시스템 트레이 매니저 가져오기
var trayMgr = Plugin.SystemTrayManager.Instance;

// 윈도우를 트레이로 최소화
trayMgr.MinimizeToTray();

// 윈도우 표시
trayMgr.ShowWindow();

// 윈도우 숨김
trayMgr.HideWindow();

// 윈도우 토글
trayMgr.ToggleWindow();
```

### 키보드 단축키
- `Ctrl + Shift + T`: 윈도우 표시/숨김 토글

## 구현 세부사항

### Windows
- `SystemTray.cs`: Win32 API를 사용하여 윈도우 제어
- 현재는 기본적인 최소화/표시 기능만 구현
- 향후 실제 시스템 트레이 아이콘 추가 필요

### macOS
- `MenuBar.cs`: Objective-C 네이티브 플러그인과 연동
- 실제 구현을 위해서는 `Plugins/macOS/` 폴더에 Objective-C 플러그인 추가 필요

## 향후 개선 사항

1. **Windows 시스템 트레이 아이콘**
   - System.Windows.Forms.NotifyIcon 사용
   - 또는 Win32 API로 직접 구현
   - 트레이 아이콘 클릭 이벤트 처리

2. **macOS 네이티브 플러그인**
   - Objective-C/Swift로 NSStatusItem 구현
   - 메뉴바 아이콘 클릭 이벤트 처리
   - 메뉴 아이템 액션 콜백 구현

3. **통합 기능**
   - 게임 상태 표시 (예: 동물 수, 수집품 등)
   - 알림 기능
   - 설정 메뉴

## 주의사항

- Unity Editor에서는 일부 기능이 제한될 수 있습니다
- 실제 빌드에서 테스트해야 합니다
- macOS의 경우 네이티브 플러그인 구현이 필요합니다

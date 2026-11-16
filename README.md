# Not Hero

덱을 구성해서 무작위로 생성된 맵을 진행하는 덱빌딩 로그라이크 게임입니다.

| 항목 | 내용 |
|---|---|
| 인원 | 4인 팀 프로젝트 |
| 개발 기간 | 2026.03 ~ 2026.04 |
| 플랫폼 | PC |
| 사용 기술 | Unity, C#, ScriptableObject, DOTween |
| 플레이 영상 | https://www.youtube.com/watch?v=ChbWpCXmwUg |

## 담당 파트

제가 작성한 코드는 [`Assets/03_Scripts/Biotite`](Assets/03_Scripts/Biotite)에 있습니다.

| 파트 | 주요 파일 |
|---|---|
| 카드 데이터, 카드 효과 | `Data/Card/*`, `Core/CardEffects/*`, `Utils/DescBuilder.cs` |
| 카드 드로우, 손패 | `System/CardSystem.cs`, `Core/Card/Card.cs` |
| 절차적 맵 생성 | `System/Map/MapGeneratorSystem.cs`, `MapNode.cs`, `UI/MapUI.cs` |
| 매니저, 이벤트, 데이터 | `Managers/*` |
| 상점 | `UI/ShopUI.cs`, `CardShopUI.cs`, `EquipmentShopUI.cs` |
| 오브젝트 풀링 | `Managers/PoolingManager.cs`, `Interface/IPoolingObject.cs` |

## 주요 구현

### 효과를 조합해 만드는 카드

- 카드는 `CardData`(ScriptableObject)로 만들고, 실제 기능은 `CardEffect`를 상속한 효과 클래스로 분리했습니다. 데미지, 방어도, 드로우, 회복, 코스트 효과가 있습니다.
- 효과 리스트는 `[SerializeReference]`와 `SubclassSelector`로 인스펙터에서 골라 조합합니다. 새 카드는 코드 없이 효과만 골라서 만들 수 있습니다.
- 다단 히트는 DOTween의 `DelayedCall`로 시간차를 두고 데미지를 줍니다.

### 카드 설명 자동 생성

- 카드 설명에 `{Damage}`, `{Armor}`, `{DrawCount}` 같은 키워드를 넣어 두면, `DescBuilder`가 실제 수치로 바꾸고 종류별로 색을 입힙니다.
- 수치는 시전자의 스탯을 반영해서 계산하기 때문에, 힘이 오르면 카드 설명의 데미지도 같이 바뀝니다. 수치를 바꿔도 설명을 따로 고칠 필요가 없습니다.

### 연속 드로우 큐

- 턴 시작 드로우와 카드 효과로 인한 추가 드로우가 연달아 들어오면 연출과 손패 정렬이 꼬이는 문제가 있었습니다.
- 드로우 요청을 큐에 쌓고, 코루틴이 하나씩 꺼내서 앞 연출이 끝난 다음에 처리하도록 바꿨습니다.
- 드로우 연출 시작과 끝을 이벤트로 알려서, 연출 중에는 카드 사용과 덱 보기를 막습니다.

### 절차적 맵 생성

- 2차원 배열 위에 노드를 만들고, 시작 노드에서 위쪽으로 경로를 무작위로 연결합니다.
- 새로 연결할 경로가 옆 경로와 교차하면 다른 노드를 다시 고릅니다.
- 맨 위에는 보스방을 두고, 중간에 보물방과 휴식방을 넣은 다음 나머지 방의 종류는 인스펙터에서 정한 가중치로 뽑습니다.

### 매니저 초기화 순서와 이벤트

- `Awake`, `Start`만으로는 매니저 간 초기화 순서를 보장할 수 없어서, `Managers`에서 데이터 → UI → 플레이어 스탯 → 사운드 → 스테이지 → 풀링 → 전투 순서로 직접 초기화합니다.
- 이벤트 매니저로 팀원끼리 스크립트를 직접 참조하지 않게 했습니다. 예를 들어 카드 시스템은 턴 시작, 적 턴, 전투 승리를 이벤트로만 받습니다.
- 데이터는 Resources 폴더에서 타입별로 한 번에 불러와 딕셔너리에 저장하고, 카드 데이터는 복사본을 만들어 써서 원본 에셋이 바뀌지 않게 했습니다.

## 다음 프로젝트에서 개선한 점

이 프로젝트에서 아쉬웠던 부분은 다음 프로젝트인 [Figure Out](https://github.com/lbiotitel/FigureOut)에서 고쳤습니다.

- object 매개변수를 쓰는 이벤트 → 구조체와 제네릭을 쓰는, 박싱 없는 이벤트
- Resources 로드 → Addressable 로드

## 저장소 안내

- 팀 프로젝트라 다른 팀원의 코드도 함께 있습니다. 제 코드는 위의 `Biotite` 폴더입니다.
- `Biotite/Utils/UiRoundedCorners`는 외부 오픈소스 코드입니다.
- 에셋 스토어 리소스가 포함되어 있습니다.

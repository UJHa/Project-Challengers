# Project-Challengers
Unity 엔진을 사용하여 게임을 개발하는 프로젝트입니다.

작업 진행 현황(12.05 : 작업 중단 // "EnemyFindIdleState, PlayerMoveState 내 리펙토링 작업" 부터 추후 재시작)
○○○○○○○○○○○○○작업할 일
- 현재 이름으로 private Dictionary<string, ChessTile> prevPathTileNodeMap; >> hash 키로 적용하기 >> guid 구현
- 탐색 쿨타임 제거로 인한 무한 공격 해결할 것!
- 라운드 데이터 외부로 추출하기
- 마우스 입력을 리펙토링(어떻게?) >> InputManager 통하여
- 게임 내 요소 설계하기(Game, input, player 등 데이터 별 매니져 분리)
- 대기실 스폰 기획 정리
- 애니메이션 끝나고 state 변경되는 현상 원인 찾기
- ChessWaitCharacter 제거 ChessCharacter 상태 값으로 판단할 방법 찾기
○○○○○○○○○○○○○작업 중인 내용
- state idle, find, move, attack, dead 분리하기(+네이밍 리펙토링)
	- EnemyFindIdleState 리펙토링 진행중
		- 탐색 쿨타임 제거
		- (진행중)길찾기 함수 하나로 묶기(EnemyFindIdleState, PlayerMoveState 내 리펙토링 작업)
			- (완료)길찾기 자료구조(Stack, Queue)의 관리를 ChessCharacter로 이동
			- GetPathQueueCount 함수들의 조건을 변경하여 함수 제거
			- FindPath 관련 함수들 내용 ChessCharacter로 이동을 위한 중복내용 분석 및 정리
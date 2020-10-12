### 사용법 ###
좌클릭 + 드래그 : 스티커 위치 이동
휠클릭 : 종료
우클릭 : 캐릭터 순서대로 변경
시스템 트레이 : 시스템 트레이 참고


### 캐릭터 추가 방법 ###
Resources/Config.json 파일에 추가하려는 캐릭터 정보를 추가.
Resources 폴더에 한 프레임이 100 x 100 규격인 커스텀 캐릭터 스프라이트 추가.

Config.json 파일 요소)
"Type" : 해당 리소스의 목적. (캐릭터를 추가하는 경우 무조건 "Resource")
"Frame" : 스프라이트에 있는 캐릭터 프레임 수
"Count" : 한 번에 그려지는 캐릭터의 수 (분신술)
"TrayName" : 시스템 트레이에 표시될 이름
"Path" : 스프라이트 파일 경로

주의사항)
스프라이트는 한 프레임당 100 x 100 픽셀 규격이며, 이에 맞지 않는 스프라이트를 사용하려 할 경우 프로그램이 실행되지 않을 수 있음.
스프라이트의 규격은 가로 100 x 프레임수, 세로 100 의 규격이어야 함. (한 줄에 모든 프레임이 일렬로 있어야 함)
Config.json 파일은 json 형식을 준수해야 하며, 잘못된 타입 이름, 잘못된 프레임, 잘못된 파일 경로를 입력할 경우 프로그램이 실행되지 않음.
모든 요소는 대/소문자를 구분해 입력해야 함.
단 요소들 사이의 공백은 상관없음.
스프라이트보다 더 많은 프레임을 설정하거나, 0 이하의 프레임을 설정한 스프라이트는 실행되지 않음.
스프라이트 경로는 반드시 "Resources/" 를 포함해야 함.

모든 요소가 올바른 예시)
{"Type" : "Resource", "Frame" : 10, "Count" :  10, "TrayName" : "시스템 트레이에 표시될 이름", "Path" : "Resources/파일이름.png" },

모든 요소가 잘못된 예시)
{"Type" : "resource", "Frame" : -9, "Count" :  -3, "TrayName" : "시스템 트레이에 표시될 이름", "Path" : "파일이름.png" }


## 제작자 ##
Naruell


## 제작에 도움을 받은 영상 ##
개발자 라라님)
https://www.youtube.com/watch?v=UkflQCKjDdg&lc=z22utzjp2luadlo4r04t1aokgj1he0j4p3fsrjmv2yeirk0h00410


## 헬테이커 악마들 스프라이트 출처 ##
https://www.spriters-resource.com/pc_computer/helltaker/


## 크레딧 ##
[ WhiteWindy.png ] : WhiteWindy 님의 오너캐 하얀바람


## 수정 내역 ##
2020/06/21 프로젝트 시작
2020/06/21 프로그램 항상 최상단 표시
2020/06/21 케르베로스를 포함한 다른 악마들 추가
2020/06/22 시스템 트레이 인터페이스 한글화
2020/10/05 Config.json 파일에서 리소스 정보를 읽어오도록 변경
2020/10/05 코드 리팩토링
2020/10/06 코드 리팩토링
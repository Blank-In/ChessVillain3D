using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ObjectList{
    public GameObject[] data;
}

public class MainController : MonoBehaviour {
    public ObjectList[] FloorList;//체스판의 물리적 목록
    public ObjectList[] PieceList;//체스피스의 물리적 목록
    public CameraMove cameraScript;//카메라 스크립트
    public GameObject printText;//텍스트

    private AIController AIScript = new AIController();//인공지능 스크립트
    private int[,] code = new int[8, 8];//말의 코드
    private string[,] team = new string[8, 8];//말의 팀
    private bool[,] move = new bool[8, 8];//선택한 말의 움직일 수 있는 위치
    private bool[] buttonFlag = { false, false, false, false, false, false };//버튼 UI 클릭 체크
    private int x = 0, y = 0, cx = 0, cy = 0, nx = 0, ny = 0;//cx,cy를 x,y로 이동한다 | nx,ny는 보조 위치표시자
    private bool Selected = false, airun = true, gameend = true;//기억
    private string mat = "Black";//체스판의 색 기억

    //1 룩, 2 나이트, 3 비숍, 4 퀸, 5 킹, 6 폰
    //흑팀 "Black", 백팀 "White"

    IEnumerator WaitForIt(int nowx,int nowy,int gox,int goy)
    {
        airun = false;
        //카메라이동
        cameraScript.CameraTurn(true);
        //시작위치 바닥색 변경
        yield return new WaitForSeconds(1);
        Color_change(FloorList[nowx].data[nowy], "EnemyMove");
        //도착위치 바닥색 변경
        yield return new WaitForSeconds(0.5f);
        Color_change(FloorList[gox].data[goy], "EnemyMove");
        //말 이동
        yield return new WaitForSeconds(0.5f);
        if (PieceList[gox].data[goy] != null)
        {
            PieceList[gox].data[goy].gameObject.transform.position = new Vector3(100, 100, 100);
        }
        //말 이동
        yield return new WaitForSeconds(0.5f);
        PieceList[nowx].data[nowy].gameObject.transform.position = new Vector3(-3.5f + gox * 1.2f, 0.1f, -3.5f + goy * 1.2f);
        yield return new WaitForSeconds(0.5f);
        Board_reset();
        //카메라 회전
        cameraScript.CameraTurn(true);
        //추상적 이동
        PieceList[gox].data[goy] = PieceList[nowx].data[nowy];
        PieceList[nowx].data[nowy] = null;
        if (code[gox, goy] == 5) {
            //인공지능의 승리로 게임종료
            StartCoroutine(GameEnd("체스빌런의 승리\n인간시대의 끝이 도래했다"));
        }
        code[gox, goy] = code[nowx, nowy];
        if (code[gox, goy] == 6 && goy == 0) {
            code[gox, goy] = 4;
        }
        code[nowx, nowy] = 0;
        team[gox, goy] = team[nowx, nowy];
        team[nowx, nowy] = null;
        Board_reset();
        airun = true;
    }

    IEnumerator GameEnd(string endText)
    {
        gameend = false;
        printText.GetComponent<Text>().text = endText;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void AI() {
        int[] aiControll = AIScript.BlackControl(code, team);
        if (aiControll[0] + aiControll[1] + aiControll[2] + aiControll[3] != 0)
        {
            StartCoroutine(WaitForIt(aiControll[0], aiControll[1], aiControll[2], aiControll[3]));
        }
    }
    
    void Start(){
        for (int a = 0; a < 8; a++){
            //흑말설정
            code[a, 6] = 6;
            team[a, 6] = "Black";
            if (a < 5) {
                code[a, 7] = a + 1;
            }
            else {
                code[a, 7] = 8 - a;
            }
            team[a, 7] = "Black";
            //흰말설정
            code[a, 1] = 6;
            team[a, 1] = "White";
            if (a < 5) {
                code[a, 0] = a + 1;
            }
            else {
                code[a, 0] = 8 - a;
            }
            team[a, 0] = "White";
        }
        Board_reset();
    }

    public void ButtonOnClick(string code) //버튼 UI 액션
    {
        switch (code)
        {
            case "right":
                buttonFlag[0] = true;
                break;
            case "left":
                buttonFlag[1] = true;
                break;
            case "up":
                buttonFlag[2] = true;
                break;
            case "down":
                buttonFlag[3] = true;
                break;
            case "select":
                buttonFlag[4] = true;
                break;
            case "cancel":
                buttonFlag[5] = true;
                break;
        }
    }
    
    void Update(){
        if (Input.GetKeyDown(KeyCode.RightArrow)||buttonFlag[0]) {
            if (x < 7) {
                x++;
                Change_curser();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || buttonFlag[1]) {
            if (x > 0) {
                x--;
                Change_curser();
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || buttonFlag[2]) {
            if (y < 7) {
                y++;
                Change_curser();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || buttonFlag[3]) {
            if (y > 0) {
                y--;
                Change_curser();
            }
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || buttonFlag[4]) && airun && gameend) {
            if (team[x,y]=="White"&&!Selected) {
                cx = x;
                cy = y;
                Selected = true;
                Color_change(PieceList[x].data[y], "PlayerMove");
                switch (code[x, y]){
                    case 1:
                        Rook();
                        break;
                    case 2:
                        int[,] list2 = { { cx - 2, cy - 1 }, { cx - 1, cy - 2 }, { cx + 1, y - 2 }, { cx + 2, cy - 1 }, { cx + 2, cy + 1 }, { cx + 1, cy + 2 }, { cx - 1, cy + 2 }, { cx - 2, cy + 1 } };
                        for (int a = 0; a < 8; a++) {
                            if (list2[a, 0] < 8 && list2[a, 0] > -1 && list2[a, 1] < 8 && list2[a, 1] > -1 && team[list2[a, 0], list2[a, 1]] != "White") {
                                Color_change(FloorList[list2[a, 0]].data[list2[a, 1]], "PlayerMove");
                                move[list2[a, 0],list2[a, 1]] = true;
                            }
                        }
                        break;
                    case 3:
                        Bishop();
                        break;
                    case 4:
                        Rook();
                        Bishop();
                        break;
                    case 5:
                        int[,] list5 = { { cx - 1, cy - 1 }, { cx, cy - 1 }, { cx + 1, cy - 1 }, { cx + 1, cy }, { cx + 1, cy + 1 }, { cx, cy + 1 }, { cx - 1, cy + 1 }, { cx - 1, cy } };
                        for (int a = 0; a < 8; a++) {
                            if (list5[a, 0] < 8 && list5[a, 0] > -1 && list5[a, 1] < 8 && list5[a, 1] > -1 && team[list5[a, 0], list5[a, 1]] != "White") {
                                Color_change(FloorList[list5[a, 0]].data[list5[a, 1]], "PlayerMove");
                                move[list5[a, 0], list5[a, 1]] = true;
                            }
                        }
                        break;
                    case 6:
                        if (cy < 7) {
                            if (cx > 0 && team[cx - 1, cy + 1] == "Black") {
                                Color_change(FloorList[cx - 1].data[cy + 1], "PlayerMove");
                                move[cx - 1, cy + 1] = true;
                            }
                            if (cx < 7 && team[cx + 1, cy + 1] == "Black") {
                                Color_change(FloorList[cx + 1].data[cy + 1], "PlayerMove");
                                move[cx + 1, cy + 1] = true;
                            }
                            if (cy < 7 && code[cx, cy+1] == 0) {
                                if (cy == 1 && code[cx, cy+2] == 0) {
                                    Color_change(FloorList[cx].data[cy + 2], "PlayerMove");
                                    move[cx, cy + 2] = true;
                                }
                                Color_change(FloorList[cx].data[cy + 1], "PlayerMove");
                                move[cx, cy + 1] = true;
                            }
                        }
                        break;
                }
            }
            else if(Selected&&move[x,y]==true) {
                Selected = false;
                Color_change(PieceList[cx].data[cy], team[cx, cy]);
                Board_reset();
                if (PieceList[x].data[y] != null) {
                    PieceList[x].data[y].gameObject.transform.position = new Vector3(100, 100, 100);
                }
                PieceList[cx].data[cy].gameObject.transform.position = new Vector3((float)3.5 + x * (float)1.2, (float)0.1, (float)3.5 + y * (float)1.2);
                PieceList[x].data[y] = PieceList[cx].data[cy];
                PieceList[cx].data[cy] = null;
                if (code[x, y] == 5) {
                    //플레이어의 승리로 게임종료
                    StartCoroutine(GameEnd("플레이어의 승리\n기계는 때려야 말을듣지"));
                }
                code[x, y] = code[cx, cy];
                if (code[x, y] == 6 && y == 7) {
                    code[x, y] = 4;
                }
                code[cx, cy] = 0;
                team[x, y] = team[cx, cy];
                team[cx, cy] = null;
                //여기서 인공지능 실행
                if (gameend)
                {
                    AI();
                }
            }
        }
        else if ((Input.GetKeyDown (KeyCode.Escape)|| buttonFlag[5])&&Selected) {
            Selected = false;
            Color_change(PieceList[cx].data[cy], team[cx, cy]);
            Board_reset();
        }
        buttonFlag = new bool[6]{false,false,false,false,false,false};
	}

    private void Change_curser() {
        if (move[nx, ny]) {
            Color_change(FloorList[nx].data[ny], "PlayerMove");
        }
        else {
            Color_change(FloorList[nx].data[ny], mat);
        }
        nx = x;
        ny = y;
        Color_change(FloorList[nx].data[ny], "Select");
        if (mat == "Black") {
            mat = "White";
        }
        else {
            mat = "Black";
        }
    }
    
    private void Color_change(GameObject item, string mat) {
        item.GetComponent<MeshRenderer>().material = Resources.Load(mat) as Material;
    }
    
    private void Board_reset() {
        for(int a = 0; a < 8; a++) {
            for(int b = 0; b < 8; b++) {
                move[a, b] = false;
                if ((a + b) % 2 == 0) {
                    Color_change(FloorList[a].data[b], "Black");
                }
                else
                {
                    Color_change(FloorList[a].data[b], "White");
                }
            }
        }
        Color_change(FloorList[x].data[y], "Select");
    }
    
    private void Rook() {
        bool up = true, down = true, left = true, right = true;
        for (int a = 1; a < 8; a++) {
            if (up && cy+a < 8) {
                if (team[cx, cy + a] == "White") {
                    up = false;
                }
                else {
                    Color_change(FloorList[cx].data[cy + a], "PlayerMove");
                    move[cx, cy + a] = true;
                }
                if (team[cx, cy + a] == "Black") {
                    up = false;
                }
            }
           if (down && cy-a > -1) {
                if (team[cx, cy - a] == "White") {
                    down = false;
                }
                else {
                    Color_change(FloorList[cx].data[cy - a], "PlayerMove");
                    move[cx, cy - a] = true;
                }
                if (team[cx, cy - a] == "Black") {
                    down = false;
                }
            }
            if(left && cx + a < 8) {
                if (team[cx + a, cy] == "White") {
                    left = false;
                }
                else {
                    Color_change(FloorList[cx + a].data[cy], "PlayerMove");
                    move[cx + a, cy] = true;
                }
                if (team[cx + a, cy] == "Black") {
                    left = false;
                }
            }
            if (right && cx-a > -1) {
                if (team[cx - a, cy] == "White") {
                    right = false;
                }
                else {
                    Color_change(FloorList[cx - a].data[cy], "PlayerMove");
                    move[cx - a, cy] = true;
                }
                if (team[cx - a, cy] == "Black") {
                    right = false;
                }
            }
        }
    }
    private void Bishop() {
        bool RU = true, RD = true, LD = true, LU = true;
        for(int a = 1; a < 8; a++) {
            if (RU && cx + a < 8 && cy + a < 8) {
                if (team[cx + a, cy + a] == "White") {
                    RU = false;
                }
                else {
                    Color_change(FloorList[cx + a].data[cy + a], "PlayerMove");
                    move[cx + a, cy + a] = true;
                }
                if (team[cx + a, cy + a] == "Black") {
                    RU = false;
                }
            }
            if (RD && cx + a < 8 && cy - a > -1) {
                if (team[cx + a, cy - a] == "White") {
                    RD = false;
                }
                else {
                    Color_change(FloorList[cx + a].data[cy - a], "PlayerMove");
                    move[cx + a, cy - a] = true;
                }
                if (team[cx + a, cy - a] == "Black") {
                    RD = false;
                }
            }
            if (LD && cx - a > -1 && cy - a > -1) {
                if (team[cx - a, cy - a] == "White") {
                    LD = false;
                }
                else {
                    Color_change(FloorList[cx - a].data[cy - a], "PlayerMove");
                    move[cx - a, cy - a] = true;
                }
                if (team[cx - a, cy - a] == "Black") {
                    LD = false;
                }
            }
            if (LU && cx - a > -1 && cy + a < 8) {
                if (team[cx - a, cy + a] == "White") {
                    LU = false;
                }
                else {
                    Color_change(FloorList[cx - a].data[cy + a], "PlayerMove");
                    move[cx - a, cy + a] = true;
                }
                if (team[cx - a, cy + a] == "Black") {
                    LU = false;
                }
            }
        }
    }

}
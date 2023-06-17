using System.Collections;
using System.Collections.Generic;

public class AIController
{
    //1 룩, 2 나이트, 3 비숍, 4 퀸, 5 킹, 6 폰
    //흑팀 "Black", 백팀 "White"
    int[,] code;
    string[,] team;
    private bool[,] danger = new bool[8, 8];
    int[] move = new int[5];

    //흰말 위험지역 체크
    bool PieceCheck(int x,int y) {
        if (team[x, y] == "Black" && code[x, y] == 5) {
            return true;
        }
        if (team[x, y] != null) {
            return false;
        }
        return true;
    }
    void WhiteRook(int x, int y) {
        bool left = true, right = true, top = true, bottom = true;
        for(int a = 1; a < 8; a++) {
            if (x + a <= 7 && right) {
                danger[x + a, y] = true;
                right = PieceCheck(x + a, y);
            }
            if (x - a >= 0 && left) {
                danger[x - a, y] = true;
                left = PieceCheck(x - a, y);
            }
            if (y + a <= 7 && top) {
                danger[x, y + a] = true;
                top = PieceCheck(x, y + a);
            }
            if (y - a >= 0 && bottom) {
                danger[x, y - a] = true;
                bottom = PieceCheck(x, y);
            }
        }
    }
    void WhiteBishop(int x, int y) {
        bool left = true, right = true, top = true, bottom = true;
        for (int a = 1; a < 8; a++) {
            if (x + a <=7 && y + a <= 7 && right) {
                danger[x + a, y + a] = true;
                right = PieceCheck(x + a, y + a);
            }
            if (x + a <= 7 && y - a >= 0 && left) {
                danger[x + a, y - a] = true;
                left = PieceCheck(x + a, y - a);
            }
            if (x - a >= 0 && y - a >= 0 && top) {
                danger[x - a, y - a] = true;
                top = PieceCheck(x - a, y - a);
            }
            if (x - a >= 0 && y + a <= 7 && bottom) {
                danger[x - a, y + a] = true;
                bottom = PieceCheck(x - a, y + a);
            }
        }
    }
    void WhiteNormal(int[,] templist) {
        for (int a = 0; a < templist.Length/2; a++) {
            if (templist[a, 0] >= 0 && templist[a, 0] <= 7 && templist[a, 1] >= 0 && templist[a, 1] <= 7) {
                danger[templist[a, 0], templist[a, 1]] = true;
            }
        }
    }
    
    //검은말 이동가능 체크
    int BlackPoint(int piece) {
        return piece switch
        {
            1 => 50,
            2 => 30,
            3 => 30,
            4 => 90,
            5 => 1000,
            6 => 10,
            _ => 0,
        };
    }
    bool BlackCheck(int nowx,int nowy,int gox,int goy) {
        int score = 0;
        //현재 위치의 위험성 체크
        if (danger[nowx, nowy]) {
            score += BlackPoint(code[nowx, nowy]);
        }
        //가려는 위치의 이득 체크
        if (team[gox, goy]=="White") {
            score += BlackPoint(code[gox, goy]);
        }
        //가려는 위치의 팀킬 체크
        else if (team[gox, goy]=="Black") {
            score -= 10000;
        }
        //가려는 위치의 위험성 체크
        if (danger[gox, goy]) {
            score -= BlackPoint(code[nowx, nowy]);
        }
        if (code[nowx, nowy] == 6&&nowy==6) {
            score += 5;
        }
        if (move[4] < score) {
            move = new int[] { nowx, nowy, gox, goy, score };
            return true;
        }
        return false;
    }
    void BlackRook(int x, int y) {
        bool left = true, right = true, top = true, bottom = true;
        for (int a = 1; a < 8; a++) {
            if (x + a <= 7 && right) {
                BlackCheck(x, y, x + a, y);
                right = PieceCheck(x + a, y);
            }
            if (x - a >= 0 && left) {
                BlackCheck(x, y, x - a, y);
                left = PieceCheck(x - a, y);
            }
            if (y + a <= 7 && top) {
                BlackCheck(x, y, x, y + a);
                top = PieceCheck(x, y + a);
            }
            if (y - a >= 0 && bottom) {
                BlackCheck(x, y, x, y - a);
                bottom = PieceCheck(x, y - a);
            }
        }
    }
    void BlackBishop(int x, int y) {
        bool left = true, right = true, top = true, bottom = true;
        for (int a = 1; a < 8; a++) {
            if (x + a <= 7 && y + a <= 7 && right) {
                BlackCheck(x, y, x + a, y + a);
                right = PieceCheck(x + a, y + a);
            }
            if (x + a <= 7 && y - a >= 0 && left) {
                BlackCheck(x, y, x + a, y - a);
                left = PieceCheck(x + a, y - a);
            }
            if (x - a >= 0 && y - a >= 0 && top) {
                BlackCheck(x, y, x - a, y - a);
                top = PieceCheck(x - a, y - a);
            }
            if (x - a >= 0 && y + a <= 7 && bottom) {
                BlackCheck(x, y, x - a, y + a);
                bottom = PieceCheck(x - a, y + a);
            }
        }
    }
    void BlackNormal(int[,] templist,int x,int y) {
        for (int a = 0; a < templist.Length / 2; a++) {
            if (templist[a, 0] >= 0 && templist[a, 0] <= 7 && templist[a, 1] >= 0 && templist[a, 1] <= 7&&team[templist[a,0],templist[a,1]]!="Black") {
                BlackCheck(x, y, templist[a, 0], templist[a, 1]);
            }
        }
    }
    
    public int[] BlackControl(int[,] tempcode,string[,] tempteam) {
        danger = new bool[8, 8];
        code = tempcode;
        team = tempteam;
        move = new int[] { 0, 0, 0, 0, -10000 };
        //흰말의 이동범위 
        for(int x = 0; x < 8; x++) {
            for(int y = 0; y < 8; y++) {
                if (team[x, y] == "White") {
                    switch (code[x, y]) {
                        case 1:
                            WhiteRook(x,y);
                            break;
                        case 2:
                            int[,] temp2 = { {x-2,y+1}, {x-1,y+2}, {x+1,y+2}, {x+2,y+1}, {x+2,y-1}, {x+1,y-2}, {x-1,y-2}, {x-2,y-1} };
                            WhiteNormal(temp2);
                            break;
                        case 3:
                            WhiteBishop(x,y);
                            break;
                        case 4:
                            WhiteRook(x,y);
                            WhiteBishop(x,y);
                            break;
                        case 5:
                            int[,] temp5 = { {x-1,y+1}, {x,y+1}, {x+1,y+1}, {x+1,y}, {x+1,y-1}, {x,y-1}, {x-1,y-1}, {x-1,y} };
                            WhiteNormal(temp5);
                            break;
                        case 6:
                            int[,] temp6 = { {x-1,y+1}, {x+1,y+1} };
                            WhiteNormal(temp6);
                            break;
                    }
                }
            }
        }
        //검말 이동
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                if (team[x, y] == "Black") {
                    switch (code[x, y]) {
                        case 1:
                            BlackRook(x, y);
                            break;
                        case 2:
                            int[,] temp2 = { { x - 2, y + 1 }, { x - 1, y + 2 }, { x + 1, y + 2 }, { x + 2, y + 1 }, { x + 2, y - 1 }, { x + 1, y - 2 }, { x - 1, y - 2 }, { x - 2, y - 1 } };
                            BlackNormal(temp2, x, y);
                            break;
                        case 3:
                            BlackBishop(x, y);
                            break;
                        case 4:
                            BlackRook(x, y);
                            BlackBishop(x, y);
                            break;
                        case 5:
                            int[,] temp5 = { { x - 1, y + 1 }, { x, y + 1 }, { x + 1, y + 1 }, { x + 1, y }, { x + 1, y - 1 }, { x, y - 1 }, { x - 1, y - 1 }, { x - 1, y } };
                            BlackNormal(temp5, x, y);
                            break;
                        case 6:
                            if (y >= 1) {//1칸 전진
                                if (x > 0 && team[x - 1, y - 1] == "White") {
                                    BlackCheck(x, y, x - 1, y - 1);
                                }
                                if (x < 7 && team[x + 1, y - 1] == "White") {
                                    BlackCheck(x, y, x + 1, y - 1);
                                }
                                if (move[4] < 8 - y && team[x, y - 1] == null) {
                                    if (BlackCheck(x, y, x, y - 1)) {
                                        move[4] = 8 - y;
                                    }
                                }
                            }
                            if (y == 6 && team[x, 4] == null && team[x, 5] == null) {//2칸 전진
                                if (move[4] < 5) {
                                    if (BlackCheck(x, y, x, y - 2)) {
                                        move[4] = 5;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
        return move;
    }
}
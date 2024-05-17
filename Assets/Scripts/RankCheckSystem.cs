using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class RankCheckSystem
{
    private static int[] diceNumberCount = new int[6];
    public static int[] ranksList = new int[9];
    private static RanksFlag ranksCheckList = 0; //랭크 체크 후 9개 족보 활성화 여부 저장

    public static RanksFlag RankCheck(int[] list)
    {
        ClearList();
        Array.Copy(list, diceNumberCount, list.Length);
        RankCheck5();
        //switch (GameMgr.Instance.currentDiceCount)
        //{
        //    case GameMgr.DiceCount.three:
        //        RankCheck3();
        //        break;
        //    case GameMgr.DiceCount.four:
        //        RankCheck4();
        //        break;
        //    case GameMgr.DiceCount.five:
        //        RankCheck5();
        //        break;
        //}

        return ranksCheckList;
    }

    private static void RankCheck3()
    {
        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            if (diceNumberCount[i] >= 2)
            {
                ranksCheckList |= RanksFlag.OnePair;
            }
        } // 원페어 체크


        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            if (diceNumberCount[i] >= 3)
            {
                ranksCheckList |= RanksFlag.Triple;
            }
        } // 트리플 체크

        for (int i = 0; i < diceNumberCount.Length - 2; i++)
        {
            if (diceNumberCount[i] >= 1 && diceNumberCount[i + 1] >= 1 && diceNumberCount[i + 2] >= 1)
            {
                ranksCheckList |= RanksFlag.Straight3;

            }
        } // 스트레이트 3 체크

    }


    private static void RankCheck4()
    {
        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            if (diceNumberCount[i] >= 2)
            {
                for (int j = 0; j < diceNumberCount.Length; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    if (diceNumberCount[j] >= 2)
                    {
                        ranksCheckList |= RanksFlag.TwoPair;
                    }
                }
                ranksCheckList |= RanksFlag.OnePair;
            }// 원페어, 투페어 체크
        }

        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            if (diceNumberCount[i] >= 3)
            {
                ranksCheckList |= RanksFlag.Triple;
            }
        } // 트리플 체크

        for (int i = 0; i < diceNumberCount.Length - 2; i++)
        {
            if (diceNumberCount[i] >= 1 && diceNumberCount[i + 1] >= 1 && diceNumberCount[i + 2] >= 1)
            {
                ranksCheckList |= RanksFlag.Straight3;

            }
        } // 스트레이트 3 체크


        for (int i = 0; i < diceNumberCount.Length - 3; i++)
        {
            if (diceNumberCount[i] >= 1 && diceNumberCount[i + 1] >= 1 && diceNumberCount[i + 2] >= 1
                && diceNumberCount[i + 3] >= 1)
            {
                ranksCheckList |= RanksFlag.Straight4;
            }
        } // 스트레이트 4 체크

        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            if (diceNumberCount[i] >= 4)
            {
                ranksCheckList |= RanksFlag.KindOf4;
            }
        } // 카인드 4 체크

    }


    private static void RankCheck5()
    {
        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            if (diceNumberCount[i] >= 2)
            {
                for (int j = 0; j < diceNumberCount.Length; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    if (diceNumberCount[j] >= 2 && ranksList[(int)Ranks.TwoPair] != 0)
                    {
                        ranksCheckList |= RanksFlag.TwoPair;
                    }
                }
                ranksCheckList |= RanksFlag.OnePair;

            }
        } // 원페어, 투페어 체크


        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            if (diceNumberCount[i] >= 3)
            {
                for (int j = 0; j < diceNumberCount.Length; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    if (diceNumberCount[j] >= 2 && ranksList[(int)Ranks.FullHouse] != 0)
                    {
                        ranksCheckList |= RanksFlag.FullHouse;
                    }
                }
                if (ranksList[(int)Ranks.Triple] != 0)
                {
                    ranksCheckList |= RanksFlag.Triple;
                }
            }
        } // 트리플, 풀하우스 체크


        for (int i = 0; i < diceNumberCount.Length - 2; i++)
        {
            if (diceNumberCount[i] >= 1 && diceNumberCount[i + 1] >= 1 && diceNumberCount[i + 2] >= 1)
            {
                if (ranksList[(int)Ranks.Straight3] != 0)
                {
                    ranksCheckList |= RanksFlag.Straight3;
                }
            }
        } // 스트레이트 3 체크


        for (int i = 0; i < diceNumberCount.Length - 3; i++)
        {
            if (diceNumberCount[i] >= 1 && diceNumberCount[i + 1] >= 1 && diceNumberCount[i + 2] >= 1
                && diceNumberCount[i + 3] >= 1)
            {
                if (ranksList[(int)Ranks.Straight4] != 0)
                {
                    ranksCheckList |= RanksFlag.Straight4;
                }
            }
        } // 스트레이트 4 체크

        for (int i = 0; i < diceNumberCount.Length - 4; i++)
        {
            if (diceNumberCount[i] >= 1 && diceNumberCount[i + 1] >= 1 && diceNumberCount[i + 2] >= 1
                && diceNumberCount[i + 3] >= 1 && diceNumberCount[i + 4] >= 1)
            {
                if (ranksList[(int)Ranks.Straight5] != 0)
                {
                    ranksCheckList |= RanksFlag.Straight5;
                }
            }
        } // 스트레이트 5 체크


        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            if (diceNumberCount[i] >= 4)
            {
                if (ranksList[(int)Ranks.KindOf4] != 0)
                {
                    ranksCheckList |= RanksFlag.KindOf4;
                }
            }
        } // 카인드 4 체크

        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            if (diceNumberCount[i] == 5)
            {
                if (ranksList[(int)Ranks.KindOf5] != 0)
                {
                    ranksCheckList |= RanksFlag.KindOf5;
                }
            }
        } // 카인드 5 체크


    }

    private static void ClearList()
    {
        for (int i = 0; i < diceNumberCount.Length; i++)
        {
            diceNumberCount[i] = 0;
        }

        ranksCheckList = 0;
    }
}

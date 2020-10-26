using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KeySchedule_GUI
{
    class DFS
    {
        const uint ONE = 0b0001;
        const int OPEN = 1, CLOSE = 3;

        int[][] schedule;
        string[] names;
        uint start_key;

        public List<string> answers;

        public DFS()
        {
            /*schedule = new int[][]{
                new int[] { 2, 2, 2, 0, 2, 0, 2, 2, 2, 2 },
                new int[] { 0, 3, 2, 2, 3, 0, 3, 0, 1, 2 },
                new int[] { 2, 0, 2, 0, 2, 2, 2, 2, 0, 1 },
                new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 3, 0 },
                new int[] { 3, 0, 3, 0, 0, 0, 0, 3, 0, 3 },
                new int[] { 0, 0, 3, 0, 0, 3, 0, 0, 0, 3 },
                new int[] { 0, 3, 0, 0, 0, 0, 0, 0, 3, 0 },
                new int[] { 3, 0, 0, 3, 0, 0, 0, 3, 0, 0 },
                new int[] { 0, 0, 0, 3, 0, 0, 3, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 3, 3, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 3, 0, 0, 0, 0, 0, 0 },
                new int[] { 1, 1, 0, 1, 1, 2, 0, 1, 0, 0 },
                new int[] { 1, 0, 1, 0, 0, 1, 1, 1, 0, 1 },
                new int[] { 0, 1, 0, 0, 1, 1, 1, 0, 1, 0 },
                new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };*/
            /*schedule = new int[][]{
                new int[] { 2, 2, 2, 0, 2},
                new int[] { 0, 3, 2, 2, 3},
                new int[] { 2, 0, 2, 0, 2 },
                new int[] { 0, 0, 0, 1, 0 },
                new int[] { 3, 0, 3, 0, 0 },
                new int[] { 0, 0, 3, 0, 0 },
                new int[] { 0, 3, 0, 0, 0 },
                new int[] { 3, 0, 0, 3, 0 },
                new int[] { 0, 0, 0, 3, 0 },
                new int[] { 0, 0, 0, 0, 3},
                new int[] { 0, 0, 0, 3, 0},
                new int[] { 1, 1, 0, 1, 1 },
                new int[] { 1, 0, 1, 0, 0},
                new int[] { 0, 1, 0, 0, 1 },
                new int[] { 0, 0, 0, 0, 0 }
            };*/
        }

        public DFS(List<int[]> schedule,List<string> names,uint start_key)
        {
            this.schedule = schedule.ToArray();
            this.names = names.ToArray();
            this.start_key = start_key;

            answers = new List<string>();
        }

        public void Search(int box)
        {
            string answer = "";

            Open(start_key, box, 0, answer);
        }

        private void Open(uint key, int box, int day, string answer)
        {
            //シフト表の外
            if (schedule[0].Length <= day)
            {
                //解を格納
                answers.Add(answer);

                return;
            }

            //解が1000個を超過しているなら問答無用で終了
            if (answers.Count > 100)
            {
                return;
            }

            //開店可能
            if (CanOpenClose(key, OPEN, day))
            {
                //鍵情報を保存
                answer += Convert.ToString(key, 2).PadLeft(schedule.Length,'0') + Environment.NewLine;

                //業務開始
                DepositKey(key, box, day, OPEN, answer);
            }
        }

        private void DepositKey(uint key, int box, int day, int time, string answer)
        {
            //全員についてチェック
            for (int p = 0; p < schedule.Length; p++)
            {
                //勤務時間
                if (schedule[p][day] == time)
                {
                    //鍵を所持
                    if ((key >> p & ONE) != 0)
                    {
                        //鍵を手放す
                        key = key ^ (ONE << p);
                        //鍵を預ける
                        box++;
                    }
                }
            }

            //鍵回収フェイズへ
            GetKey(key, box, day, time, 0, answer);
        }

        private void GetKey(uint key, int box, int day, int time, int p_start, string answer)
        {
            //行動を未決定の従業員についてチェック
            for (int p = p_start; p < schedule.Length; p++)
            {
                //勤務時間
                if (schedule[p][day] == time)
                {
                    //預けられている鍵がある
                    if (box >= 1)
                    {
                        //鍵を取る
                        GetKey(key | (ONE << p), box - 1, day, time, p + 1, answer);
                    }
                }
            }

            //閉店時間
            if (time >= CLOSE)
            {
                //閉店作業
                Close(key, box, day, answer);
            }
            else
            {
                //時間を進める
                DepositKey(key, box, day, time + 1, answer);
            }
        }

        private void Close(uint key, int box, int day, string answer)
        {
            //閉店可能
            if (CanOpenClose(key, CLOSE, day))
            {
                //鍵情報を保存
                answer += Convert.ToString(key, 2).PadLeft(schedule.Length, '0') + Environment.NewLine;

                Open(key, box, day + 1, answer);
            }

            return;
        }

        private bool CanOpenClose(uint key, int time, int day)
        {
            //各従業員についてチェック
            for (int p = 0; p < schedule.Length; p++)
            {
                //鍵を持っている
                if ((key >> p & ONE) != 0)
                {
                    //指定の動作が可能
                    if (schedule[p][day] == time)
                    {
                        return true;
                    }
                }
            }

            //開け閉め不可
            return false;
        }

        static public string AnswerToCsv(string answer)
        {
            string csv="";

            //分解後のデータ
            List<char[]> list=new List<char[]>();

            //行分解の準備
            StringReader stringReader = new StringReader(answer);
            //行
            string line;

            //分解して格納
            while((line = stringReader.ReadLine()) != null)
            {
                list.Add(line.ToCharArray().Reverse().ToArray());
            }

            for (int p = 0; p < list[0].Length; p++)
            {
                csv += Environment.NewLine;

                for (int t = 0; t < list.Count; t++)
                {
                    csv += list[t][p]+",";
                }
            }

            csv.Remove(csv.Length - 1);

            return csv;
        }
    }
}

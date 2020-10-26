using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace KeySchedule_GUI
{
    public partial class Form1 : Form
    {
        DFS dfs;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ダイアログ表示
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //決定ボタンを押した
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //ファイルを開く
                    Stream stream = openFileDialog.OpenFile();

                    //読み込む
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        //一時格納用
                        List<int[]> list = new List<int[]>();
                        List<string> name = new List<string>();
                        uint key = 0;

                        try
                        {
                            //行数
                            int i = 0;
                            //1行読み込み
                            string line;
                            //1行目を無視
                            reader.ReadLine();

                            while ((line = reader.ReadLine()) != null)
                            {
                                //分割
                                List<string> str =line.Split(',').ToList();
                                
                                //名前を保存
                                name.Add(str[0]);
                                //名前部分を削除
                                str.RemoveAt(0);

                                //鍵の有無
                                uint flag = 0;
                                //鍵の有無を値に変換
                                uint.TryParse(str[0],out flag);
                                //鍵の有無を保存
                                key |= flag << i;
                                //鍵部分を削除
                                str.RemoveAt(0);

                                //シフト表を保存
                                list.Add(str.Select(s => int.Parse(s)).ToArray());


                                //行数を加算
                                i++;
                            }

                            //シフト表を元にDFSをインスタンス化
                            dfs = new DFS(list,name,key);

                            //完了通知
                            label1.Text = "Load Complete";
                        }
                        catch (FormatException exception)
                        {
                            //エラー通知
                            label1.Text = "Error";
                        }

                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "Start";

            dfs.Search(decimal.ToInt32(numericUpDown1.Value));

            if (dfs.answers.Count > 0)
            {
                textBox1.Text = DFS.AnswerToCsv(dfs.answers.Last());

                label1.Text = "End";
            }
            else
            {
                label1.Text = "No solution";
            }
        }
    }
}

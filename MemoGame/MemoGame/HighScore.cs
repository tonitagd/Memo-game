using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MemoGame
{
    class HighScore
    {
        private List<ScoreLog> highScoreList = new List<ScoreLog>();

        private int Compare(ScoreLog a, ScoreLog b)
        {
            return a.Score.CompareTo(b.Score);
        }

        private int CompareDesc(ScoreLog a, ScoreLog b)
        {
            return b.Score.CompareTo(a.Score);
        }

        public HighScore()
        {
            Stream stream = File.Open(@"..\..\score\HighScore.bin", FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();

            List<string> listScores = new List<string>();
            using (stream)
            {
                if (stream.Length > 0)
                {
                    int EntitiesNumber = (int)bformatter.Deserialize(stream);
                    for (int i = 1; i <= EntitiesNumber; i++)
                    {
                        ScoreLog log = (ScoreLog)bformatter.Deserialize(stream);
                        highScoreList.Add(log);
                    }
                }
            }
        }

        public int Lowest
        {
            get
            {
                if (highScoreList.Count > 4)
                {
                    return highScoreList.Last().Score;
                }
                else
                    return 0;
            }
        }

        public void AddScore(int score, string name)
        {

            highScoreList.Add(new ScoreLog(score, name, DateTime.Now.ToString("dd/MM/yyyy")));
            highScoreList.Sort(CompareDesc);

            if (highScoreList.Count > 5)
            {
                highScoreList.RemoveAt(highScoreList.Count - 1);
            }
        }

        public void SaveScores()
        {
            Stream stream = File.Open(@"..\..\score\HighScore.bin", FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();
            using (stream)
            {
                highScoreList.Sort(CompareDesc);
                //add number of objects
                bformatter.Serialize(stream, highScoreList.Count);
                foreach (var item in highScoreList)
                {
                    //add log score
                    bformatter.Serialize(stream, item);
                }
            }
        }

        public static List<string> LoadScores()
        {
            Stream stream = File.Open(@"..\..\score\HighScore.bin", FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();

            List<string> listScores = new List<string>();
            using (stream)
            {
                if (stream.Length > 0)
                {
                    int EntitiesNumber = (int)bformatter.Deserialize(stream);
                    for (int i = 1; i <= EntitiesNumber; i++)
                    {
                        ScoreLog log = (ScoreLog)bformatter.Deserialize(stream);
                        string str = i.ToString() + "." + log.ToString();
                        listScores.Add(str);
                    }
                }
            }

            return listScores;
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace MemoGame
{
    [Serializable()]   
    class ScoreLog : ISerializable
    {
        public ScoreLog(int score, string name, string date)
        {
            Score = score;
            Name = name;
            Date = date;
        }

        //Deserialization constructor.
        public ScoreLog(SerializationInfo info, StreamingContext ctxt)
        {
            //Get the values from info and assign them to the appropriate properties
            Score = (int)info.GetValue("Score", typeof(int));
            Name = (String)info.GetValue("Name", typeof(string));
            Date = (String)info.GetValue("Date", typeof(string));
        }

        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            //You can use any custom name for your name-value pair. But make sure you read the values with the same name.
            info.AddValue("Score", Score);
            info.AddValue("Name", Name);
            info.AddValue("Date", Date);
        }

        public override string ToString()
        {
            return string.Format("{0}  {1}  {2}", Name.PadRight(13), Score.ToString().PadRight(6), Date);
        }

        public int Score { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewmanQuz.QuizData
{
    public class DataQuiz
    {
        public string QuestionID { get; set; }
        public string Question { get; set; }
        public string AnswerKey { get; set; }

        public int numberOfCorrect { get; set; }

        public int numberOfWrong { get; set; }

        public string[] Answers { get; set; }
    }
}



using System;
using System.Collections.Generic;
using System.Text;

namespace Newman.Data
{
    class QuizSettings
    {
        public static int setNumQuestions { get; set; } 

        public static string SetPlayerName { get; set; }

        public static string GlobalLogPath { get; set; } = NewmanQuz.Helpers.FileProcessor.tempDirectory;

        public static int SetTimeLimit { get; set; }

        public static string LogName = "QuizLog.txt";

    }


}

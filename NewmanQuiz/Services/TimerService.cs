using Newman.Data;
using NewmanQuz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NewmanQuiz.Services
{
    class TimerService
    {
        public static bool IsQuizStarted;
        public static void StartTimer()
        {

            if (QuizSettings.SetTimeLimit != 0)
            {
                int a = QuizSettings.SetTimeLimit;

                do
                {
                    if (!IsQuizStarted)
                    {
                        IsQuizStarted = true;
                        Program.StartQuiz();



                    }

                    if (IsQuizStarted == true)
                    {
                        System.Threading.Thread.Sleep(1000);

                        a--;
                    }
                   

                    if(a == 0)
                    {
                        Console.Clear();
                        Console.Write("TIMES UP!");
                        Program.stopWatch.Stop();
                        System.Threading.Thread.Sleep(2500);

                        // Get the elapsed time as a TimeSpan value.
                        TimeSpan ts = Program.stopWatch.Elapsed;

                        // Format and display the TimeSpan value.

                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
                        string logText = "PLAYER: " + QuizSettings.SetPlayerName + "\nNumber Answered Correct: " + Program.scoreKeeper.numberOfCorrect +
                            " Number of Incorrect: " + Program.scoreKeeper.numberOfWrong + "\nLength of Time: " + elapsedTime + " \n" + Environment.NewLine;
                        string logPath = QuizSettings.GlobalLogPath + QuizSettings.LogName;

                        File.AppendAllText(logPath, logText);

                        Console.WriteLine("Game Over\n");
                        Console.WriteLine(logText);
                        System.Threading.Thread.Sleep(3500);
                        Program.ResetData();
                        Console.Clear();
                        Program.ShowMenu();
                    }

                } while (a > 0);
            }

        }

    }
}

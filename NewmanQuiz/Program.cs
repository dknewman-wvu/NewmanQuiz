using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newman.Data;
using NewmanQuiz.Services;
using NewmanQuz.Helpers;
using NewmanQuz.QuizData;
using NewmanQuz.Services;

namespace NewmanQuz
{
    public class Program
    {
        public static List<string> _quiz;
        public static string getAnswerKey;
        public static string answserChosen;
        public static bool isQuizStarted;
        public static string quizFilePath;
        public static string answerChosen;
        public static string sInput;
        public static int questionCount = 1;
        public static int scoreNumOfRight = 0;
        public static int scoreNumOfWrong = 0;
        public static DataQuiz scoreKeeper = new DataQuiz();
        public static Stopwatch stopWatch = new Stopwatch();





        static void Main(string[] args)
        {
            ShowMenu();

        }

        public static void ShowMenu()
        {
            Console.WriteLine("CSS 533 Quiz!");
            Console.WriteLine();
            Console.WriteLine("STUDENT: David Newman");
            Console.WriteLine();
            Console.WriteLine("1. Set Question File Location");
            Console.WriteLine("2. Set Log File Location");
            Console.WriteLine("3. Set the number of questions");
            Console.WriteLine("4. Set the Timer");
            Console.WriteLine("5. Load Scores");
            Console.WriteLine("6. Start Quiz");
            Console.WriteLine("7. QUIT");

            GenerateQuiz();


        }

        private static void GenerateQuiz()
        {
            MainMenu mmChoice = MainMenu.UNASSIGNED;

            if (isQuizStarted != false)
            {
                sInput = "6";

            }
            else
            {
                sInput = Console.ReadLine();
            }

            if (Enum.TryParse(sInput, out mmChoice))
            {
                switch (mmChoice)
                {
                    case MainMenu.LOADQUIZ:
                        Console.Clear();
                        Console.WriteLine("Set the path to the question file");
                        quizFilePath = Console.ReadLine();
                        FileProcessor.ReadFile();
                        Console.Clear();
                        ShowMenu();
                        break;

                    case MainMenu.SETNUMQUESTIONS:
                        SetNumQuestion();
                        break;

                    case MainMenu.STARTQUIZ:
                        stopWatch.Start();
                        if (QuizSettings.setNumQuestions == 0)
                        {
                            SetNumQuestion();
                        }

                        if (QuizSettings.SetPlayerName == null)
                        {
                            SetPlayerName();
                        }

                        StartQuiz();

                        break;

                    case MainMenu.QUIT:
                        Console.WriteLine("Goodbye!");
                        Environment.Exit(0);

                        break;

                    case MainMenu.LOADSCORES:
                        if (File.Exists(QuizSettings.GlobalLogPath + QuizSettings.LogName))
                        {
                            ReadLogFile();

                        }
                        else
                        {
                            Console.WriteLine("There is no log file. Play a few rounds to get on generated");
                            System.Threading.Thread.Sleep(2000);
                            ShowMenu();


                        }

                        break;

                    case MainMenu.SETTIMER:
                        var valid = false;

                        while (!valid)
                        {
                            Console.WriteLine("Set the number of seconds for the quiz.\nTo have unlimited time put in 0 \nRemember 1 minute = 60 seconds.");
                            var val = Console.ReadLine();
                            valid = !string.IsNullOrWhiteSpace(val) &&
                                val.All(c => c >= '0' && c <= '9');
                            if (valid == true)
                            {
                                QuizSettings.SetTimeLimit = Int32.Parse(val);
                                Console.WriteLine("The timer is set for: " + val);
                                System.Threading.Thread.Sleep(2500);
                                Console.Clear();
                                ShowMenu();
                            }
                            if (!valid)
                                Console.WriteLine("Please enter a valid a number!");
                        }


                        break;

                    case MainMenu.SETLOGLOCATION:
                        Console.Clear();
                        Console.WriteLine("Set the path to the log file");
                        QuizSettings.GlobalLogPath = Console.ReadLine();
                        SetLogPath();

                        break;

                    case MainMenu.UNASSIGNED:
                        break;
                }
            }
        }

        public static void StartQuiz()
        {
            if (questionCount <= QuizSettings.setNumQuestions)
            {
                if (!TimerService.IsQuizStarted)
                {
                    Task.Run(() => TimerService.StartTimer());

                }
                GenerateQuestions();
                GenerateAnswers();
                answerChosen = Console.ReadLine();
                if (getAnswerKey == answerChosen)
                {
                    Console.WriteLine("CORRECT!");
                    scoreNumOfRight = scoreNumOfRight + 1;
                    scoreKeeper.numberOfCorrect = scoreNumOfRight;
                    questionCount = questionCount + 1;
                    System.Threading.Thread.Sleep(2000);
                    Console.Clear();
                    GenerateQuiz();
                }
                else
                {
                    if (answerChosen.ToUpper() == "EXIT")
                    {
                        isQuizStarted = false;
                        Console.Clear();
                        ShowMenu();
                    }
                    else
                    {
                        Console.WriteLine("SORRY THAT IS INCORRECT!");
                        scoreNumOfWrong = scoreNumOfWrong + 1;
                        scoreKeeper.numberOfWrong = scoreNumOfWrong;
                        questionCount = questionCount + 1;
                        System.Threading.Thread.Sleep(2000);
                        Console.Clear();
                        GenerateQuiz();
                    }

                }

            }
            else
            {
                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.

                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                string logText = "PLAYER: " + QuizSettings.SetPlayerName + "\nNumber Answered Correct: " + scoreKeeper.numberOfCorrect +
                    " Number of Incorrect: " + scoreKeeper.numberOfWrong + "\nLength of Time: " + elapsedTime + " \n" + Environment.NewLine;
                string logPath = QuizSettings.GlobalLogPath + QuizSettings.LogName;

                File.AppendAllText(logPath, logText);

                Console.WriteLine("Game Over\n");
                Console.WriteLine(logText);
                System.Threading.Thread.Sleep(3500);
                ResetData();
                Console.Clear();
                ShowMenu();

            }
        }

        private static void SetLogPath()
        {
            if (QuizSettings.GlobalLogPath != null && QuizSettings.GlobalLogPath != string.Empty)
            {
                Console.WriteLine("The log path has been set to " + QuizSettings.GlobalLogPath);
                System.Threading.Thread.Sleep(2000);
                ShowMenu();

            }
            else
            {
                Console.WriteLine("The path cannot be blank, please enter a valid location!");
                System.Threading.Thread.Sleep(2000);
                Console.Clear();
                ShowMenu();

            }
        }

        private static void ReadLogFile()
        {

            string readText = File.ReadAllText(QuizSettings.GlobalLogPath + QuizSettings.LogName);
            Console.WriteLine(readText);
            System.Threading.Thread.Sleep(4000);
            Console.Clear();
            ShowMenu();

        }

        public static void ResetData()
        {
            isQuizStarted = false;
            questionCount = 1;
            scoreNumOfRight = 0;
            scoreNumOfWrong = 0;
            scoreKeeper.numberOfCorrect = 0;
            scoreKeeper.numberOfWrong = 0;
            stopWatch.Reset();
        }

        private static void SetNumQuestion()
        {
            Console.Clear();
            Console.WriteLine("SET THE NUMBER OF QUESTIONS TO ASK: ");
            string numberOfQuestions = Console.ReadLine();
            QuizSettings.setNumQuestions = Int32.Parse(numberOfQuestions);
            if (QuizSettings.setNumQuestions > 0)
            {
                Console.WriteLine("YOU WILL BE ASKED " + QuizSettings.setNumQuestions + " QUESTIONS");
                System.Threading.Thread.Sleep(2000);
                Console.Clear();
                ShowMenu();
            }
            else
            {
                Console.WriteLine("Please Select a number greater than 0!");
                System.Threading.Thread.Sleep(1500);
                Console.Clear();
                SetNumQuestion();
            }

        }

        private static void SetPlayerName()
        {
            Console.Clear();
            Console.WriteLine("PLESE SET THE PLAYER NAME: ");
            QuizSettings.SetPlayerName = Console.ReadLine();

            if (QuizSettings.SetPlayerName != null && QuizSettings.SetPlayerName != string.Empty)
            {
                Console.WriteLine("Thank You! The new player is: " + QuizSettings.SetPlayerName);
                System.Threading.Thread.Sleep(2000);
                Console.Clear();
                StartQuiz();

            }
            else
            {
                Console.WriteLine("Please enter a valid name");
                System.Threading.Thread.Sleep(2000);
                Console.Clear();
                SetPlayerName();

            }

        }

        private enum MainMenu
        {
            LOADQUIZ = 1,
            SETLOGLOCATION,
            SETNUMQUESTIONS,
            SETTIMER,
            LOADSCORES,
            STARTQUIZ,
            QUIT,
            UNASSIGNED
        }

        public static void GenerateQuestions()
        {
            Console.Clear();
            Console.WriteLine("Question");
            Console.WriteLine(" ");
            QuizService.SetQuizQuestions();
            string newQuestion = QuizService.question.Trim();
            Console.Write(Regex.Replace(newQuestion, "^[0-9]+", string.Empty) + "\n");
            isQuizStarted = true;
            Console.WriteLine(" ");
            Console.WriteLine(" ");


        }
        public static void GenerateAnswers()
        {


            int num = 1;

            foreach (string answer in QuizService.answer.Skip(1))
            {
                Console.WriteLine(" " + num + ": " + answer);
                num = num + 1;
            }

            Console.WriteLine("\n");
            Console.WriteLine("Please select your answer or type EXIT to quit.");

            getAnswerKey = QuizService.answerKey.ToString();
            Debug.WriteLine("ANSWER KEY: " + getAnswerKey);
            var answerPick = new object();

        }


    }
}

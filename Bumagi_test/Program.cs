using Bumagi_test.Helpers;
using Bumagi_test.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bumagi_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string command = string.Empty;
            var questionareHelper = QuestionareHelper.getInstance();
            Stack<Questionnaire> filledQstnrs = new Stack<Questionnaire>();

            do
            {
                Console.Clear();
                Console.WriteLine("Выберите действие: ");

                command = Console.ReadLine();
                var inputArg = command.Split(" ");

                // Некоторый команды содержат аргументы
                if (inputArg.Length > 1)
                {
                    command = inputArg[0];
                }

                switch (command)
                {
                    case "-new_profile":
                        filledQstnrs.Push(questionareHelper.StartFilling());
                        break;

                    case "-statistics":
                        StatisticsHelper.GetDefaultStatistics(filledQstnrs);
                        break;

                    case "-save":
                        questionareHelper.SaveQuestionare(filledQstnrs.Peek());
                        break;

                    case "-find":
                        if (inputArg.Length == 2)
                            questionareHelper.FindQuestionareFile(inputArg[1]);
                        break;

                    case "-delete":
                        if (inputArg.Length == 2)
                            questionareHelper.DeleteQuestionareFile(inputArg[1]);
                        break;

                    case "-list":
                        questionareHelper.ListQuestionareFiles();
                        break;

                    case "-list_today":
                        questionareHelper.ListQuestionareFiles(true);
                        break;

                    case "-zip":
                        if (inputArg.Length == 3)
                            questionareHelper.ArchiveFile(inputArg[1], inputArg[2]);
                        break;

                    case "-help":
                        ShowHelp();
                        break;

                    default:
                        break;
                }
            } while (command != "-exit");
        }

        public static void ShowHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("-new_profile - Заполнить новую анкету")
                .AppendLine("-statistics - Показать статистику всех заполненных анкет")
                .AppendLine("-save - Сохранить заполненную анкету")
                .AppendLine("-find <Имя файла анкеты> - Найти анкету и показать данные анкеты в консоль")
                .AppendLine("-delete <Имя файла анкеты> - Удалить указанную анкету")
                .AppendLine("-list - Показать список названий файлов всех сохранённых анкет")
                .AppendLine("-list_today - Показать список названий файлов всех сохранённых анкет, созданных сегодня")
                .AppendLine("-zip <Имя файла анкеты> <Путь для сохранения архива> - Запаковать указанную анкету в архив и сохранить архив по указанному пути")
                .AppendLine("-help - Показать список доступных команд с описанием")
                .AppendLine("-exit - Выйти из приложения");

            Console.Clear();
            Console.WriteLine(sb.ToString());
            ConsoleHelper.PressToContinue();


        }
    }
}

using Bumagi_test.Helpers;
using Bumagi_test.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Bumagi_test
{
    internal class QuestionareHelper
    {
        private Questionnaire[] questionnaires;
        private static QuestionareHelper instance;

        /// <summary>
        /// Конструктор, вызывается единожды
        /// </summary>
        /// <param name="pathToXml"></param>
        private QuestionareHelper(string pathToXml = "")
        {
            // Директория в которой распологаются шаблоны анкет
            if (string.IsNullOrEmpty(pathToXml))
                pathToXml = ConfigurationManager.AppSettings.Get("QuestionariesPath");

            var directoryInfo = new DirectoryInfo(pathToXml);

            if (!directoryInfo.Exists)
                return;

            // Имена всех шаблонов
            var fileNames = directoryInfo.GetFiles();

            XmlSerializer ser = new XmlSerializer(typeof(Questionnaire));
            List<Questionnaire> qstList = new List<Questionnaire>();

            // Подргружаем все шаблоны 
            foreach (var fileInfo in fileNames)
            {
                var fileExt = Path.GetExtension(fileInfo.Name);

                // Подгружаем только xml, и только то что анкета
                if (fileExt == ".xml")
                    try
                    {
                        using (FileStream file = new FileStream(fileInfo.FullName, FileMode.Open))
                        {
                            qstList.Add((Questionnaire)ser.Deserialize(file));
                        }
                    }
                    catch { }
            }

            questionnaires = qstList.ToArray();
        }

        /// <summary>
        /// Единственный способ проинициализировать экземпляр класса
        /// </summary>
        /// <returns></returns>
        public static QuestionareHelper getInstance()
        {
            if (instance == null)
                instance = new QuestionareHelper();
            return instance;
        }

        #region Создание новой анкеты
        /// <summary>
        /// Подготовка к заполнению анкеты
        /// Проверки на существование и выбор анкеты
        /// </summary>
        public Questionnaire StartFilling()
        {

            // Если в директории не было ни одного шаблона анкеты, то и заполнять нечего
            if ((questionnaires?.Length ?? 0) == 0)
            {
                Console.WriteLine("Нет анкет");
                Console.ReadKey();
                return null;
            }

            // Для того чтобы не пачкать ответами шаблоны создается дубликат анкеты, и уже этот объект будет в дальнейшем в работе
            Questionnaire qstnrForFilling;

            if (questionnaires.Length == 1)
            {
                qstnrForFilling = (Questionnaire)questionnaires[0].Clone();
            }
            else
            {
                // Если существует несколько анкет можно выбрать какую именно заполнять
                Console.Clear();
                Console.WriteLine("Выберите анкету для заполнения");

                for (int counter = 0; counter < questionnaires.Length; counter++)
                {
                    Console.WriteLine($"\t{counter}: " + questionnaires[counter].Name);
                }

                string userInput;
                int num = 1;

                do
                {
                    Console.Write("Номер анкеты: ");
                    userInput = Console.ReadLine();
                } while (!(!string.IsNullOrEmpty(userInput) && Int32.TryParse(userInput, out num) && num <= questionnaires.Length));


                qstnrForFilling = (Questionnaire)questionnaires[num].Clone();
            }

            // После выбора анкеты, ее вопросы передаются для заполнения
            FillQuestionaire(qstnrForFilling.Questions);

            // Как все вопросы заполнены, сохраняем дату
            qstnrForFilling.FillDate = DateTime.Now;

            return qstnrForFilling;
        }

        /// <summary>
        /// Проверка анкеты на наличие вопросов и передача вопросов для заполнения
        /// </summary>
        /// <param name="questions"></param>
        private void FillQuestionaire(QuestionnaireQuestion[] questions)
        {
            if ((questions?.Length ?? 0) == 0)
            {
                Console.WriteLine("В анкете нет вопросов");
                Console.ReadKey();
                return;
            }

            int qNum = 0; // Номер вопроса, изначально первый вопрос (0)
            int gotoNum = -1; // Если пользваотель захотел перейти кк какому либо вопросу значение сохраняется сюда
            int tmpInt = 0; // времянка, для свапа значений
            bool nextQuestion = false; // признак переходить ли к следующему вопросу, false  в случае если пользователь ввел какую либо команду ( кроме help )
            string userInput = string.Empty; // пользовательский ввод

            do
            {
                // Вопрос уходит на заполнение
                (nextQuestion, userInput) = FillQuestions(questions[qNum]);

                // Если пользователь ввел какую либо команду
                if (!nextQuestion)
                {
                    userInput = userInput.Trim();

                    if (userInput.Contains("-goto_question"))
                    {
                        // При переходе на другой вопрос, номер текущего вопроса сохраняется. И после заполнения возвращает на тот же вопрос
                        int.TryParse(userInput.Substring("-goto_question".Length), out gotoNum);
                        if (--gotoNum >= 0 && gotoNum < qNum)
                        {
                            tmpInt = qNum;
                            qNum = gotoNum;
                            gotoNum = tmpInt;

                        }
                        else
                            Console.WriteLine("Недопустимый номер вопроса");
                    }

                    if (userInput.Contains("-goto_prev_question"))
                        qNum--;

                    if (userInput.Contains("-restart_profile"))
                        qNum = 0;
                }
                else
                {
                    questions[qNum].Answer = userInput;

                    // Если прыгал на другой вопрос, то после ввода ответа на него возвращаем на вопрос до прыжка
                    if (gotoNum != -1)
                    {
                        qNum = gotoNum;
                        gotoNum = -1;
                    }
                    else
                        qNum++;
                }

            } while (qNum < questions.Length);

            return;
        }

        /// <summary>
        /// </summary>
        /// <param name="question"></param>
        /// <returns>
        /// bool - перейти ли к следующему вопросу, или нужно выполнить иное действие
        /// string - иное действие
        /// </returns>
        private (bool, string) FillQuestions(QuestionnaireQuestion question)
        {
            Console.Clear();
            Console.WriteLine(question.Text);

            var userInput = string.Empty;
            bool correctAnswer = false;
            string ErrMsg = string.Empty;

            do
            {
                userInput = Console.ReadLine();
                if (userInput.Contains("-help"))
                {
                    ShowFillingHelp();
                    continue;
                }

                // Доступные пользователю команды
                Regex regex = new Regex("^(-goto_question|-goto_prev_question|-restart_profile)");

                if (regex.IsMatch(userInput.Trim()))
                    return (false, userInput);

                (correctAnswer, ErrMsg) = isCorrectAnswer(userInput, question.Mask, question.CheckType);

                if (!string.IsNullOrEmpty(ErrMsg))
                    Console.WriteLine(ErrMsg);

            }
            while (!correctAnswer);

            return (true, userInput);
        }

        /// <summary>
        /// Валидации ответа
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="Mask"></param>
        /// <param name="CheckType"></param>
        /// <returns></returns>
        private (bool, string) isCorrectAnswer(string userInput, string Mask, string CheckType)
        {

            if (Mask != null)
            {
                Regex regex = new Regex(Mask);
                if (!regex.IsMatch(userInput))
                    return (false, "Некорректный формат");
            }

            if (CheckType != null)
            {
                switch (CheckType)
                {
                    case "DateTime":
                        {
                            if (!DateTime.TryParse(userInput, out DateTime result))
                                return (false, "Недопустимое значение");
                            break;
                        }

                        // Можно дописать еще проверки для других типов, но в рамках задачи кроме даты ничего не приходится валидировать
                }
            }

            return (true, string.Empty);
        }

        private void ShowFillingHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("-goto_question <Номер вопроса> - Вернуться к указанному вопросу")
                .AppendLine("-goto_prev_question - Вернуться к предыдущему вопросу")
                .AppendLine("-restart_profile - Заполнить анкету заново ");

            Console.WriteLine(sb.ToString());
        }
        #endregion

        #region Сохранение анкеты
        /// <summary>
        /// Сохранение анкеты
        /// Так как текст сохраненной анкеты шаблонизирован. И может иметь произвольный формат, то необходимо заменить возможные теги реальными значениями
        /// </summary>
        /// <param name="questionnaire"></param>
        public void SaveQuestionare(Questionnaire questionnaire)
        {
            var text = questionnaire.TextTemplate;
            var fileName = questionnaire.SaveFileName;
            Dictionary<string, string> answers = new Dictionary<string, string>();

            foreach (var question in questionnaire.Questions)
                answers.Add(question.Code, question.Answer);

            answers.Add("fill_date", questionnaire.FillDate.ToString("dd.MM.yyyy"));

            foreach (var answer in answers)
            {
                text = text.Replace("{" + answer.Key + "}", answer.Value);
                fileName = fileName.Replace("{" + answer.Key + "}", answer.Value);
            }

            FileHelper.SaveFile(ConfigurationManager.AppSettings.Get("SavePath"), fileName, text);

        }

        #endregion

        #region Работа с файлами анкет

        /// <summary>
        /// Найти анкету и вывести в консоль
        /// </summary>
        /// <param name="fileName"></param>
        public void FindQuestionareFile(string fileName)
        {
            var filePath = ConfigurationManager.AppSettings.Get("SavePath") + "\\" + fileName;
            FileHelper.FileToConsole(filePath);
        }

        /// <summary>
        /// Удалить файл
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteQuestionareFile(string fileName)
        {
            var filePath = ConfigurationManager.AppSettings.Get("SavePath") + "\\" + fileName;
            FileHelper.DeleteFile(filePath);
        }

        /// <summary>
        /// Список файлов
        /// </summary>
        /// <param name="today"></param>
        public void ListQuestionareFiles(bool today = false)
        {
            FileHelper.FilesList(ConfigurationManager.AppSettings.Get("SavePath"), today);
        }

        public void ArchiveFile(string fileName, string targetWay)
        {
            var filePath = ConfigurationManager.AppSettings.Get("SavePath") + "\\" + fileName;
            FileHelper.ArchiveFile(filePath, targetWay);
        }
        #endregion
    }
}

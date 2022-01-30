using Bumagi_test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bumagi_test.Helpers
{
    internal class StatisticsHelper
    {
        /// <summary>
        /// Сбор статистики, только для дефолтных анкет
        /// Не придумал как адекватно реализовать шаблонизированный сбор статистики
        /// </summary>
        /// <param name="questionnaires"></param>
        public static void GetDefaultStatistics(IEnumerable<Questionnaire> questionnaires)
        {
            var defaultQuestionaries = from questionnaire in questionnaires
                                       where questionnaire.Code == "DefaultQuestionaire"
                                       select questionnaire;

            Dictionary<string, int> langDict = new Dictionary<string, int>();
            int totalAge = 0;            
            int maxProgExpYears = 0;
            DateTime birthDate;
            DateTime zeroTime = new DateTime(1, 1, 1);
            string langName = string.Empty;
            string fioMax = string.Empty;

            foreach (var questionnaire in defaultQuestionaries)
            {
                // Заполненные вопросы в словарь для более удобного посика позднее
                var qstnDict = questionnaire.Questions.ToDictionary(x => x.Code, x => x.Answer);
                DateTime.TryParse(qstnDict["birthdate"], out birthDate);
                totalAge += (zeroTime + (DateTime.Now - birthDate)).Year;

                langName = qstnDict["prog_language"];

                // Поиск самого популярного языка программирования
                if (langDict.ContainsKey(langName))
                    langDict[langName] = langDict[langName] + 1;
                else
                    langDict.Add(langName, 1);

                // Самый опытный программист
                if ( int.Parse(qstnDict["prog_experience"]) > maxProgExpYears)
                {
                    maxProgExpYears = int.Parse(qstnDict["prog_experience"]);
                    fioMax = qstnDict["fio"];
                }

            }

            var mostPopularLanguage = langDict.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            var middleAge = (int)Math.Round((double)totalAge / defaultQuestionaries.Count());

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Средний возраст всех опрошенных: {middleAge} {NumAge(middleAge)}")
                .AppendLine($"Самый популярный язык программирования: {mostPopularLanguage}")
                .AppendLine($"Самый опытный программист: {fioMax}");

            Console.WriteLine(sb.ToString());
            ConsoleHelper.PressToContinue();
        }

        private static string NumAge(int num)
        {
            int last = num % 10;
            if (last == 0) 
                return "лет";
            else if (last == 1) 
                return "год";
            else 
                return "года";
        }
    }
}

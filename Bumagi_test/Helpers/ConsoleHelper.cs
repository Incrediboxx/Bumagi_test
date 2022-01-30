using System;

namespace Bumagi_test.Helpers
{
    /// <summary>
    /// Часто выполняемые штуки с консолью
    /// </summary>
    internal class ConsoleHelper
    {
        public static void PressToContinue()
        {
            Console.Write("Нажмите любую клавишу чтобы продолжить... ");
            Console.ReadKey();
        }
    }
}

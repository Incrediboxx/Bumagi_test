using System;
using System.IO;
using System.IO.Compression;

namespace Bumagi_test.Helpers
{
    internal class FileHelper
    {
        /// <summary>
        /// Сохранение файла
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filename"></param>
        /// <param name="text"></param>
        public static void SaveFile(string dir, string filename, string text)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var filepath = dir + "\\" + filename;

            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.Write(text);
            }
        }

        /// <summary>
        /// Список файлов в консоль
        /// </summary>
        /// <param name="filePath"></param>
        public static void FileToConsole(string filePath)
        {
            if (File.Exists(filePath))
            {
                Console.Clear();
                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        Console.WriteLine(sr.ReadLine());
                    }
                }

                ConsoleHelper.PressToContinue();
            }
            else
                NotFound("Файл с таким именем отсутствует");
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            else
                NotFound("Файл с таким именем отсутствует");
        }

        /// <summary>
        /// Список файлов
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="today"></param>
        public static void FilesList(string dirPath, bool today = false)
        {
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
                Console.Clear();
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    if ((today && file.LastWriteTime.Date == DateTime.Now.Date) ||
                        !today)
                        Console.WriteLine(file.Name);
                }
                ConsoleHelper.PressToContinue();
            }
            else
                NotFound("Директория отсутствует");
        }

        /// <summary>
        /// Архивация файла
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="targetWay"></param>
        public static void ArchiveFile(string filePath, string targetWay)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    // поток для чтения исходного файла
                    using (FileStream sourceStream = new FileStream(filePath, FileMode.Open))
                    {
                        // поток для записи сжатого файла
                        using (FileStream targetStream = File.Create(targetWay))
                        {
                            // поток архивации
                            using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                            {
                                sourceStream.CopyTo(compressionStream);
                            }
                        }
                    }
                }
                catch { }
            }
            else
                NotFound("Файл с таким именем отсутствует");
        }

        private static void NotFound(string text)
        {
            Console.WriteLine(text);
            ConsoleHelper.PressToContinue();
        }

    }
}

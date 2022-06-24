using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LitsReinforcementLearning
{
    public static class Path 
    {
        public static string directory = "C:\\Users\\jleis\\Documents\\Visual Studio 2019\\Projects\\LitsGitRL\\LitsConsole";
    }

    public static class Log
    {
        static string logPath = $"{Path.directory}\\Logs";
        static string logFile = $"{logPath}\\Logs.txt";
        static string archiveFile 
        { 
            get 
            {
                DateTime now = DateTime.Now;
                return $"{logPath}\\Archives\\Log {now.Day}-{now.Month}-{now.Year}T{now.Hour}-{now.Minute}-{now.Second}.txt"; 
            }
        }
        static string errorFile 
        {
            get 
            {
                DateTime now = DateTime.Now;
                return $"{logPath}\\Errors\\Log {now.Day}-{now.Month}-{now.Year}T{now.Hour}-{now.Minute}-{now.Second}.txt";
            }
        }

        private static bool rotated = false;
        public static void Rotate() 
        {
            Rotate(archiveFile);
            rotated = true;
        }
        public static void RotateError() 
        {
            Rotate(errorFile);
        }
        private static void Rotate(string destPath) 
        {
            string logContents = Read();
            Write(logContents, destPath);
            Clear();
        }
        public static void Clear() 
        {
            File.WriteAllText(logFile, "");
        }
        public static void Clear(string filePath) 
        {
            File.WriteAllText(filePath, "");
        }

        private static string Read() 
        {
            if (File.Exists(logFile))
                return File.ReadAllText(logFile);
            else
                return null;
        }
        private static string Read(string path) 
        {
            if (File.Exists(path))
                return File.ReadAllText(path);
            else
                throw new FileNotFoundException();
        }
        public static void Write(string contents)
        {
            if (!rotated)
                Rotate();
            File.AppendAllText(logFile, contents + '\n');
        }
        public static void Write(string[] contents)
        {
            if (!rotated)
                Rotate();
            File.AppendAllLines(logFile, contents);
        }
        private static void Write(string contents, string path)
        {
            if (path == logPath)
                Write(contents);
            else
                File.AppendAllText(path, contents);
        }
        private static void Write(string[] contents, string path)
        {
            if (path == logPath)
                Write(contents);
            else
                File.AppendAllLines(path, contents);
        }
    }
}

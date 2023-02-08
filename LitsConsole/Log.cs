using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace LitsReinforcementLearning
{
    public static class Debug
    {
        public static bool IsDebug { get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Name == "LitsConsoleDebug"; } }
    }
    public class DebugAttribute : Attribute
    {

    }

    public static class Path 
    {
        public static string directory
        {
            get
            {
                string dir = Directory.GetCurrentDirectory();
                while (new DirectoryInfo(dir).Name != "LitsReinforcementLearning") // Gets all the way to the root (LitsGitRL) of the path
                    dir = GetParent(dir);

                return $"{dir}{Slash}LitsConsole";
            }
        }
        private static string GetParent(string path) 
        {
            return Directory.GetParent(path).ToString();
        }
        public static char Slash 
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return '\\';
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    return '/';
                else
                {
                    Log.Write($"Unfamiliar platform {RuntimeInformation.OSDescription}");
                    Log.RotateError();
                    return '/';
                }
            }
        }
    }

    

    public static class Log
    {
        static string logPath = $"{Path.directory}{Path.Slash}Logs";
        static string logFile = $"{logPath}{Path.Slash}Logs.txt";
        static string archiveFile 
        { 
            get 
            {
                DateTime now = DateTime.Now;
                return $"{logPath}{Path.Slash}Archives{Path.Slash}Log {now.Day}-{now.Month}-{now.Year}T{now.Hour}-{now.Minute}-{now.Second}.txt"; 
            }
        }
        static string errorFile 
        {
            get 
            {
                DateTime now = DateTime.Now;
                return $"{logPath}{Path.Slash}Errors{Path.Slash}Log {now.Day}-{now.Month}-{now.Year}T{now.Hour}-{now.Minute}-{now.Second}.txt";
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
            File.AppendAllText(logFile, contents + '\n');
        }
        public static void Write(string[] contents)
        {
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

    [AttributeUsage(AttributeTargets.Method)]
    public class LogAttribute : Attribute
    {
        public static void StartTimer() { }
        public static void StopTimer() { }
    }
}

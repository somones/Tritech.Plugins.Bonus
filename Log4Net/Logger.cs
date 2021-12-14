using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Tritech.Plugins.Test.Log4Net
{
     public static class Logger
     {
          private static ILog _log;

          #region Public methods

          public static void LogError(
           string message, LogType type = LogType.Error,
           [CallerFilePath] string path = @"C:\Unknown.cs", [CallerLineNumber] int line = 0, [CallerMemberName] string method = ""
           )
          {
               Log(message, LogType.Debug, path, line, method);
          }

          public static void Log(
              string message, LogType type = LogType.Debug,
              [CallerFilePath] string path = @"C:\Unknown.cs", [CallerLineNumber] int line = 0, [CallerMemberName] string method = ""
              )
          {
               // Add the caller file information
               message = $"{PrefixLog(path, line, method)} {message}";

               ProcessLog(message, type);
          }

          public static void LogIn(
              string message = "",
              [CallerFilePath] string path = @"C:\Unknown.cs", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
          {
               var prefix = PrefixLog(path, line, method);

               ProcessLog($"{prefix} >>>>>>>>>> IN >>>>>>>>>>", LogType.Debug);

               // Extra message to add?
               if (!string.IsNullOrWhiteSpace(message))
                    ProcessLog($"{prefix} {message}", LogType.Debug);
          }

          public static void LogOut(
              string message = "",
              [CallerFilePath] string path = @"C:\Unknown.cs", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
          {
               var prefix = PrefixLog(path, line, method);

               // Extra message to add?
               if (!string.IsNullOrWhiteSpace(message))
                    ProcessLog($"{prefix} {message}", LogType.Debug);

               ProcessLog($"{prefix} <<<<<<<<<< OUT <<<<<<<<<<", LogType.Debug);
          }

          public static void LogException(
              Exception exception, LogType type = LogType.Error,
              [CallerFilePath] string path = @"C:\Unknown.cs", [CallerLineNumber] int line = 0, [CallerMemberName] string method = "")
          {
               var prefix = PrefixLog(path, line, method);

               ProcessLog($"{prefix} [EXCEPTION] - Message: {exception.Message}", type);

               // Log every inner exception too
               while (exception.InnerException != null)
               {
                    exception = exception.InnerException;

                    ProcessLog($"{prefix} [INNER EXCEPTION] - Message: {exception.Message}", type);
               }
          }

          #endregion

          #region Private methods

          private static void ProcessLog(string message, LogType type)
          {
               EnsureLogger();

               switch (type)
               {
                    case LogType.Fatal:
                         _log.Fatal(message);
                         break;
                    case LogType.Error:
                         _log.Error(message);
                         break;
                    case LogType.Warn:
                         _log.Warn(message);
                         break;
                    case LogType.Info:
                         _log.Info(message);
                         break;
                    case LogType.Debug:
                    default:
                         _log.Debug(message);
                         break;
               }
          }

          private static string PrefixLog(string path, int line, string method)
          {
               var file = new FileInfo(path);

               return $"{file.Name}:{line} ({method})";
          }

          private static void EnsureLogger()
          {
               if (_log != null) return;

               //var assembly = Assembly.GetEntryAssembly();
               var assembly = new StackTrace().GetFrames().First().GetMethod().Module.Assembly;
               //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
               var logRepository = LogManager.GetRepository(assembly);
               var configFile = GetConfigFile();

               // Configure Log4Net
               XmlConfigurator.Configure(logRepository, configFile);
               _log = LogManager.GetLogger(assembly, assembly.ManifestModule.Name.Replace(".dll", "").Replace(".", " "));
          }

          private static FileInfo GetConfigFile()
          {
               FileInfo configFile = null;

               // Search config file
               var configFileNames = new[] { "Config/log4net.config", "log4net.config" };

               foreach (var configFileName in configFileNames)
               {
                    configFile = new FileInfo(configFileName);

                    if (configFile.Exists) break;
               }

               if (configFile == null || !configFile.Exists) throw new NullReferenceException("Log4net config file not found.");

               return configFile;
          }

          #endregion
     }

     public enum LogType
     {
          Debug,
          Info,
          Warn,
          Error,
          Fatal
     }
}

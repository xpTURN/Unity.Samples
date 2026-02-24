using VContainer;
using VContainer.Unity;
using VitalRouter;
using VitalRouter.VContainer;
using Utf8StringInterpolation;
using ZLogger;
using ZLogger.Unity;
using ZLogger.Providers;
using Microsoft.Extensions.Logging;

using xpTURN.Text;

public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // Don't destroy this game object when the scene is unloaded
        DontDestroyOnLoad(gameObject);

        // Exception Logging
        builder.RegisterEntryPointExceptionHandler(ex =>
        {
            UnityEngine.Debug.LogError(ex);
        });

        // Register VitalRouter
        builder.RegisterInstance(Router.Default).As<ICommandPublisher>().As<ICommandSubscribable>();
        builder.RegisterVitalRouter(routing =>
        {
            /// Note: For MonoBehaviour including routing.Map registration, do not register here if it's not a singleton.
            /// It should be registered with MapTo in the MonoBehaviour's Start method, etc.
        });

        // Logger factory setup
        var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddZLoggerUnityDebug(); // log to UnityDebug

            // output to  yyyy-MM-dd_*.log, roll by 1MB or changed date
            logging.AddZLoggerRollingFile(options =>
            {
                options.FilePathSelector = (dt, index) => XString.Format($"Logs/ZLogger/Client_{dt:yyyy-MM-dd-HH-mm-ss}_{index:000}.log");
                options.RollingInterval = RollingInterval.Day;
                options.RollingSizeKB = 1024 * 1024;
                options.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter($"{0}|{1:short}|", (in MessageTemplate template, in LogInfo info) => template.Format(info.Timestamp, info.LogLevel));
                    formatter.SetSuffixFormatter($" ({0})", (in MessageTemplate template, in LogInfo info) => template.Format(info.Category));
                    formatter.SetExceptionFormatter((writer, ex) => Utf8String.Format(writer, $"{ex.Message}"));
                });
            });
        });

        // Register logger instance
        var logger = loggerFactory.CreateLogger("Client");
        builder.RegisterInstance(logger);
        logger.XLogInformation($"Start ZLogger - Logging!");
    }
}
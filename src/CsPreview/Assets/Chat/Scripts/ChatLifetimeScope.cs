using VContainer;
using VContainer.Unity;
using VitalRouter;
using VitalRouter.VContainer;
using Microsoft.Extensions.Logging;
using ZLogger;
using ZLogger.Unity;
using ZLogger.Providers;
using Utf8StringInterpolation;

public class ChatLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // VitalRouter routing (MapTo is called from MonoBehaviour)
        builder.RegisterVitalRouter(routing =>
        {
            /// Note: For MonoBehaviour including routing.Map registration, do not register here if it's not a singleton.
            /// It should be registered with MapTo in the MonoBehaviour's Start method, etc.
        });

        // Chat-scoped services
        builder.Register<MockNetworkService>(Lifetime.Scoped).As<INetworkService>();
        builder.Register<ChatModel>(Lifetime.Scoped);
        builder.Register<ChatViewModel>(Lifetime.Scoped);
    }
}

using Autofac;
using Autofac.Features.AttributeFilters;
using Dapplo.Addons;
using Dapplo.CaliburnMicro;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.Exchange.ClientExample.Services;
using Dapplo.Exchange.ClientExample.UseCases.ContextMenu.ViewModels;

namespace Dapplo.Exchange.ClientExample
{
    /// <summary>
    /// Configure the builder for Exchange
    /// </summary>
    public class EchangeAddonModule : AddonModule
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(context => new ExchangeServiceContainer().Initialize())
                .As<ExchangeServiceContainer>()
                .SingleInstance();

            builder
                .RegisterType<ExchangeInstance>()
                .As<IService>()
                .SingleInstance();

            builder
                .RegisterType<NewEmailHandler>()
                .As<IUiStartup>()
                .SingleInstance();

            builder
                .RegisterType<ExchangeTrayIconViewModel>()
                .As<ITrayIconViewModel>()
                .WithAttributeFiltering()
                .SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IMenuItem>()
                .As<IMenuItem>()
                .SingleInstance();

            base.Load(builder);
        }
    }
}

using Autofac;
using Autofac.Features.AttributeFilters;
using Dapplo.Addons;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.CaliburnMicro.Metro.Configuration;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.Config.Ini;
using Dapplo.Config.Language;
using Dapplo.Exchange.ClientExample.Models;
using Dapplo.Exchange.ClientExample.Models.Impl;
using Dapplo.Exchange.ClientExample.Services;
using Dapplo.Exchange.ClientExample.UseCases.ContextMenu.ViewModels;

namespace Dapplo.Exchange.ClientExample
{
    /// <summary>
    /// Configure the builder for Exchange
    /// </summary>
    public class ExchangeAddonModule : AddonModule
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<ExchangeConfig>()
                .As<IIniSection>()
                .As<IExchangeConfig>()
                .As<IMetroUiConfiguration>()
                .As<IUiConfiguration>()
                .SingleInstance();

            builder
                .RegisterType<ContextMenuTranslations>()
                .As<IContextMenuTranslations>()
                .As<ILanguage>()
                .SingleInstance();

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
                .As<IService>()
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

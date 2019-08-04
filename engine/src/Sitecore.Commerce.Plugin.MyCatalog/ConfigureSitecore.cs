namespace Sitecore.Commerce.Plugin.MyCatalog
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<ISearchPipeline>(c =>
                {
                    c.Replace<Sitecore.Commerce.Plugin.Catalog.ProcessDocumentSearchResultBlock,
                    Sitecore.Commerce.Plugin.MyCatalog.Pipelines.Blocks.ProcessDocumentSearchResultBlock>();
                })
             );

            services.RegisterAllCommands(assembly);
        }
    }
}

using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Plugin.Catalog;

namespace Sitecore.Commerce.Plugin.MyCatalog.Pipelines.Blocks
{
    [PipelineDisplayName("Catalog.block.ProcessDocumentSearchResult")]
    public class ProcessDocumentSearchResultBlock : PipelineBlock<Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.EntityViews.EntityView, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Catalog.ProcessDocumentSearchResultBlock" /> class.
        /// </summary>
        /// <param name="findEntity">The find entity.</param>
        public ProcessDocumentSearchResultBlock(IFindEntityPipeline findEntity)
          : base((string)null)
        {
            this._findPipeline = findEntity;
        }

        /// <summary>Runs the specified argument.</summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// an <see cref="T:Sitecore.Commerce.EntityViews.EntityView" /></returns>
        public override async Task<Sitecore.Commerce.EntityViews.EntityView> Run(
          Sitecore.Commerce.EntityViews.EntityView arg,
          CommercePipelineExecutionContext context)
        {
            ProcessDocumentSearchResultBlock searchResultBlock = this;
            // ISSUE: explicit non-virtual call
            Condition.Requires<Sitecore.Commerce.EntityViews.EntityView>(arg).IsNotNull<Sitecore.Commerce.EntityViews.EntityView>(string.Format("{0}: argument can not be null.", (object)searchResultBlock.Name));
            List<Document> source = context.CommerceContext.GetObjects<List<Document>>().FirstOrDefault<List<Document>>();
            if (source == null || !source.Any<Document>() || !arg.HasPolicy<SearchScopePolicy>())
                return arg;
            SearchScopePolicy policy = arg.GetPolicy<SearchScopePolicy>();
            List<string> retrievableProperties = IndexablePolicy.GetPolicyByScope(context.CommerceContext, context.CommerceContext.Environment, policy.Name).Properties.Select(p => new
            {
                p = p,
                pName = p.Key.ToLower()
            }).Where(_param1 => _param1.p.Value.IsRetrievable).Select(_param1 => _param1.pName).ToList<string>();
            if (!policy.ResultDetailsTags.Any<Tag>((Func<Tag, bool>)(t => t.Name.Equals("CatalogTable", StringComparison.OrdinalIgnoreCase))))
                return arg;
            foreach (Document document in source)
            {
                string docId = document["EntityId".ToLowerInvariant()].ToString();
                Sitecore.Commerce.EntityViews.EntityView child = arg.ChildViews.OfType<Sitecore.Commerce.EntityViews.EntityView>().FirstOrDefault<Sitecore.Commerce.EntityViews.EntityView>((Func<Sitecore.Commerce.EntityViews.EntityView, bool>)(c => c.EntityId.Equals(docId, StringComparison.OrdinalIgnoreCase)));
                if (child != null)
                {
                    int? entityVersion = new int?();
                    int result;
                    if (int.TryParse(document["EntityVersion".ToLowerInvariant()].ToString(), out result))
                        entityVersion = new int?(result);
                    CommerceEntity entity = await searchResultBlock._findPipeline.Run(new FindEntityArgument(typeof(CommerceEntity), docId, entityVersion, false), context);
                    
                    if (entity == null)
                    {
                        arg.ChildViews.Remove((Model)child);
                    }
                    else
                    {
                        ViewProperty viewProperty1 = child.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("EntityId", StringComparison.OrdinalIgnoreCase)));
                        if (viewProperty1 != null)
                        {
                            viewProperty1.IsHidden = true;
                            viewProperty1.UiType = string.Empty;
                        }
                        ViewProperty viewProperty2 = child.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("SitecoreId", StringComparison.OrdinalIgnoreCase)));
                        if (viewProperty2 != null)
                            viewProperty2.IsHidden = true;
                        ViewProperty viewProperty3 = child.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("VariantId", StringComparison.OrdinalIgnoreCase)));
                        if (viewProperty3 == null && retrievableProperties.Contains<string>("VariantId", (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase))
                        {
                            ViewProperty viewProperty4 = new ViewProperty();
                            viewProperty4.Name = "variantid";
                            viewProperty4.Value = string.Empty;
                            viewProperty4.RawValue = (object)string.Empty;
                            viewProperty4.OriginalType = typeof(string).FullName;
                            viewProperty4.IsReadOnly = true;
                            viewProperty3 = viewProperty4;
                        }
                        ViewProperty viewProperty5 = child.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("VariantDisplayName", StringComparison.OrdinalIgnoreCase)));
                        if (viewProperty5 == null && retrievableProperties.Contains<string>("VariantDisplayName", (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase))
                        {
                            ViewProperty viewProperty4 = new ViewProperty();
                            viewProperty4.Name = "variantdisplayname";
                            viewProperty4.Value = string.Empty;
                            viewProperty4.RawValue = (object)string.Empty;
                            viewProperty4.OriginalType = typeof(string).FullName;
                            viewProperty4.IsReadOnly = true;
                            viewProperty5 = viewProperty4;
                        }
                        ViewProperty viewProperty6 = child.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("Name", StringComparison.OrdinalIgnoreCase)));
                        if (viewProperty6 != null)
                            viewProperty6.UiType = "EntityLink";
                        ViewProperty viewProperty7 = child.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("DisplayName", StringComparison.OrdinalIgnoreCase)));
                        ViewProperty viewProperty8 = child.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("DateUpdated", StringComparison.OrdinalIgnoreCase)));
                        ViewProperty viewProperty9 = child.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("DateCreated", StringComparison.OrdinalIgnoreCase)));
                        child.Properties.Clear();
                        
                        if (viewProperty7 != null)
                        {
                            StringBuilder catalogNames = new StringBuilder(string.Empty);
                            var sellableItem = entity as SellableItem;
                            if (sellableItem != null
                                && sellableItem.HasComponent<CatalogsComponent>())
                            {
                                var catalogsComponent = sellableItem.GetComponent<CatalogsComponent>();
                                foreach (var catalogComponent in catalogsComponent.ChildComponents.OfType<CatalogComponent>())
                                {
                                    catalogNames.Append(catalogComponent.Name);
                                    catalogNames.Append(",");
                                }

                                if (catalogNames.Length > 0 
                                    && viewProperty7.RawValue != null 
                                    && !string.IsNullOrEmpty(viewProperty7.RawValue.ToString()))
                                {
                                    viewProperty7.RawValue = string.Format("{0}-{1}", catalogNames.ToString().Trim(','), viewProperty7.RawValue);
                                }
                            }
                        }

                        if (viewProperty6 != null)
                            child.Properties.Add(viewProperty6);
                        if (viewProperty7 != null)
                            child.Properties.Add(viewProperty7);
                        if (viewProperty3 != null)
                            child.Properties.Add(viewProperty3);
                        if (viewProperty5 != null)
                            child.Properties.Add(viewProperty5);
                        if (viewProperty9 != null)
                            child.Properties.Add(viewProperty9);
                        if (viewProperty8 != null)
                            child.Properties.Add(viewProperty8);
                        child = (Sitecore.Commerce.EntityViews.EntityView)null;
                    }
                }
            }
            return arg;
        }
    }
}
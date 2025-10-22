﻿using Apps.Webflow.DataSourceHandlers;
using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.CollectionItem;
using Apps.Webflow.DataSourceHandlers.Locale;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Collection;

public class UpdateCollectionItemRequest
{
    [Display("Site ID")]
    [DataSource(typeof(SiteDataSourceHandler))]
    public string? SiteId { get; set; }

    [Display("Collection ID")]
    [DataSource(typeof(UpdateCollectionItemCollectionDataSourceHandler))]
    public string? CollectionId { get; set; }

    [Display("Locale")]
    [DataSource(typeof(UpdateCollectionItemLocaleDataSourceHandler))]
    public string? CmsLocaleId { get; set; }

    [Display("Collection item ID")]
    [DataSource(typeof(UpdateCollectionItemDataSourceHandler))]
    public string? CollectionItemId { get; set; }

    [Display("Publish collection item", Description = "Choose whether to publish your collection item during the update. False by default")]
    public bool? Publish { get; set; }
}

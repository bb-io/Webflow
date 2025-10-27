﻿using Apps.Webflow.DataSourceHandlers.Collection;
using Apps.Webflow.DataSourceHandlers.Content;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Webflow.Models.Request.Content;

public class SearchContentRequest
{
    [Display("Content type")]
    [StaticDataSource(typeof(ContentTypeStaticDataHandler))]
    public IEnumerable<string> ContentTypes { get; set; }

    [Display("Name or title contains")]
    public string? NameContains { get; set; }

    [Display("Last published before")]
    public DateTime? LastPublishedBefore { get; set; }

    [Display("Last published after")]
    public DateTime? LastPublishedAfter { get; set; }

    [Display("Collection ID")]
    [DataSource(typeof(CollectionItemCollectionDataSourceHandler))]
    public string? CollectionId { get; set; }
}

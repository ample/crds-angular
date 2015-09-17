﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.CloudSearchDomain;
using Amazon.CloudSearchDomain.Model;
using crds_angular.Services.Interfaces;
using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Translation.PlatformService;
using Newtonsoft.Json.Linq;

namespace crds_angular.Services
{
    public class SearchService : ISearchService
    {
        private readonly AmazonCloudSearchDomainClient _client;

        public SearchService(IConfigurationWrapper configurationWrapper)
        {
            var endpoint = configurationWrapper.GetEnvironmentVarAsString("AMAZON_SEARCH_ENDPOINT");
            var apiKey = configurationWrapper.GetEnvironmentVarAsString("AMAZON_API_KEY");
            var apiSecret = configurationWrapper.GetEnvironmentVarAsString("AMAZON_API_SECRET");

            AmazonCloudSearchDomainConfig config = new AmazonCloudSearchDomainConfig();

            config.ServiceURL = endpoint;
            _client = new AmazonCloudSearchDomainClient(apiKey, apiSecret, config);
        }

        public JArray GetSearchResults(string searchCriteria)
        {
            SearchRequest request = new SearchRequest();
            request.Query = searchCriteria;

            var searchResult = _client.Search(request);

            JArray resultsArray = new JArray();

            foreach (var hit in searchResult.Hits.Hit)
            {
                JObject resultObject = new JObject();

                foreach (var hitField in hit.Fields)
                {
                    JProperty fieldProperty = new JProperty(hitField.Key, hitField.Value);
                    resultObject.Add(fieldProperty);
                }

                resultsArray.Add(resultObject);
            }

            return resultsArray;
        }
    }
}
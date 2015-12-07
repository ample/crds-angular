﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Timers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using crds_angular.Models.MailChimp;
using crds_angular.Services.Interfaces;
using Crossroads.Utilities.Interfaces;
using Crossroads.Utilities.Serializers;
using log4net;
using MinistryPlatform.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using MPInterfaces = MinistryPlatform.Translation.Services.Interfaces;
using AutoMapper;
using crds_angular.App_Start;

namespace crds_angular.Services
{
    public class BulkEmailSyncService : IBulkEmailSyncService
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(GroupService));

        private readonly MPInterfaces.IBulkEmailRepository _bulkEmailRepository;
        private readonly MPInterfaces.IApiUserService _apiUserService;
        private readonly IConfigurationWrapper _configWrapper;
        private string _token;
        private System.Timers.Timer _refreshTokenTimer;

        public BulkEmailSyncService(
            MPInterfaces.IBulkEmailRepository bulkEmailRepository,
            MPInterfaces.IApiUserService apiUserService,
            IConfigurationWrapper configWrapper)
        {
            _bulkEmailRepository = bulkEmailRepository;
            _apiUserService = apiUserService;
            _configWrapper = configWrapper;

            _token = _apiUserService.GetToken();
            
            ConfigureRefreshTokenTimer();

            //force AutoMapper to register
            AutoMapperConfig.RegisterMappings();
        }

        private void ConfigureRefreshTokenTimer()
        {
            // Hack to get around token expiring every 15 minutes when updating large number of Third_Party_Contact_Id, 
            // so fire event every 10 minutes and get a new token
            _refreshTokenTimer = new System.Timers.Timer(10*60*1000);
            _refreshTokenTimer.AutoReset = true;
            _refreshTokenTimer.Elapsed += RefreshTokenTimerElapsed;
        }

        private void RefreshTokenTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Info("Refreshing token");
            _token = _apiUserService.GetToken();            
        }


        public void RunService()
        {
            try
            {                
                _refreshTokenTimer.Start();

                var publications = _bulkEmailRepository.GetPublications(_token);
                Dictionary<string, string> listResponseIds = new Dictionary<string, string>();

                // this is run first to capture any opt-ins/outs in mailchimp to avoid overwriting
                // them during the bulk sync 
                PullOptsFromThirdParty(publications);

                foreach (var publication in publications)
                {
                    var pageViewIds = _bulkEmailRepository.GetPageViewIds(_token, publication.PublicationId);
                    var subscribers = _bulkEmailRepository.GetSubscribers(_token, publication.PublicationId, pageViewIds);

                    

                    var operationId = SendBatch(publication, subscribers);

                    // add the publication and operation id in prep for polling 
                    if (!String.IsNullOrEmpty(operationId))
                    {
                        listResponseIds.Add(publication.ThirdPartyPublicationId, operationId);
                    }

                    _bulkEmailRepository.UpdateLastSyncDate(_token, publication);
                }

                ProcessSynchronizationResultsWithRetries(listResponseIds);
            }
            finally
            {
                _refreshTokenTimer.Stop();
            }
        }

        public string SendBatch(BulkEmailPublication publication, List<BulkEmailSubscriber> subscribers)
        {
            var client = GetBulkEmailClient();

            var request = new RestRequest("batches", Method.POST);
            request.AddHeader("Content-Type", "application/json");

            var batch = AddSubscribersToBatch(publication, subscribers);

            request.JsonSerializer = new RestsharpJsonNetSerializer();
            request.RequestFormat = DataFormat.Json;
            request.AddBody(batch);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // This will be addressed is US2861: MP/MailChimp Synch Error Handling 
                // TODO: Should these be exceptions?
                _logger.Error(string.Format("Failed sending batch for publication {0} with StatusCode = {1}", publication.PublicationId, response.StatusCode));
                return null;
            }

            var responseValues = DeserializeToDictionary(response.Content);

            // this needs to be returned, because we can't guarantee that the operation won't fail after it begins
            if (responseValues["status"].ToString() == "started" || 
                responseValues["status"].ToString() == "pending" || 
                responseValues["status"].ToString() == "finished")
            {
                return responseValues["id"].ToString();
            }
            else
            {
                // This will be addressed is US2861: MP/MailChimp Synch Error Handling 
                // TODO: Should these be exceptions?
                // TODO: Add logging code here for failure
                _logger.Error(string.Format("Bulk email sync failed for publication {0} Response detail: {1}", publication.PublicationId, response.Content));
                return null;
            }
        }

        private void ProcessSynchronizationResultsWithRetries(Dictionary<string, string> publicationOperationIds)
        {
            const int secondsToSleep = 5;
            const int maxRetries = 60*60/secondsToSleep;
            var attempts = 0;

            do
            {                
                attempts++;
                
                if (attempts > maxRetries)
                {
                    // This will be addressed is US2861: MP/MailChimp Synch Error Handling 
                    // TODO: Should these be exceptions?
                    // Probably an infinite loop so stop processing and log error
                    _logger.Error(string.Format("Failed to LogUpdateStatuses after {0} total retries", attempts));
                    return;
                }

                // pause to allow the operations to complete -- consider switching this to async
                Thread.Sleep(secondsToSleep * 1000);

                publicationOperationIds = ProcessSynchronizationResults(publicationOperationIds);

            } while (publicationOperationIds.Any());
        }

        private Dictionary<string, string> ProcessSynchronizationResults(Dictionary<string, string> publicationOperationIds)
        {
            var client = GetBulkEmailClient();

            for (int index = publicationOperationIds.Count - 1; index >= 0; index--)
            {
                var idPair = publicationOperationIds.ElementAt(index);
                var request = new RestRequest("batches/" + idPair.Value, Method.GET);
                request.AddHeader("Content-Type", "application/json");

                var response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // This will be addressed is US2861: MP/MailChimp Synch Error Handling 
                    _logger.Error(string.Format("StatusCode = {0}", response.StatusCode));
                    continue;
                }

                var responseValues = DeserializeToDictionary(response.Content);

                if (responseValues["status"].ToString() == "started" || responseValues["status"].ToString() == "pending")
                {
                    continue;
                }

                if (responseValues["status"].ToString() == "finished")
                {                                        
                    var total = responseValues["total_operations"];                    
                    var errors = responseValues["errored_operations"];

                    if (errors.ToString() == "0")
                    {
                        _logger.InfoFormat("Processed {0} total records", total); 
                    }
                    else
                    {
                        _logger.ErrorFormat("Processed {0} total records with {1} errors", total, errors); 
                    }

                    _logger.Info(response.Content);
                }
                else
                {
                    // This will be addressed is US2861: MP/MailChimp Synch Error Handling 
                    // TODO: Should these be exceptions?
                    // TODO: Add logging code here for failure
                    _logger.ErrorFormat("Bulk email sync failed for publication {0} Response detail: {1}", idPair.Key, response.Content);
                }

                publicationOperationIds.Remove(idPair.Key);
            }

            return publicationOperationIds;
        }

        private RestClient GetBulkEmailClient()
        {
            var apiUrl = _configWrapper.GetConfigValue("BulkEmailApiUrl");
            var apiKey = _configWrapper.GetEnvironmentVarAsString("BULK_EMAIL_API_KEY");
           
            var client = new RestClient(apiUrl);
            client.Authenticator = new HttpBasicAuthenticator("noname", apiKey);

            return client;
        }

        private SubscriberBatchDTO AddSubscribersToBatch(BulkEmailPublication publication, List<BulkEmailSubscriber> subscribers)
        {
            var batch = new SubscriberBatchDTO();
            batch.Operations = new List<SubscriberOperationDTO>();

            foreach (var subscriber in subscribers)
            {
                if (subscriber.EmailAddress != subscriber.ThirdPartyContactId)
                {
                    if (!string.IsNullOrEmpty(subscriber.ThirdPartyContactId))
                    {                        
                        var unsubscribeOperation = UnsubscribeOldEmailAddress(publication, subscriber);
                        batch.Operations.Add(unsubscribeOperation);
                    }

                    //Assuming success here, update the ID to match the emailaddress
                    //TODO: US2782 - need to also recongnize this is an email change and handle this acordingly
                    subscriber.ThirdPartyContactId = subscriber.EmailAddress;
                    _bulkEmailRepository.UpdateSubscriber(_token, subscriber);
                }

                var operation = GetUpdateOperation(publication, subscriber);
                batch.Operations.Add(operation);
            }
            
            return batch;
        }

        private SubscriberOperationDTO UnsubscribeOldEmailAddress(BulkEmailPublication publication, BulkEmailSubscriber subscriber)
        {
            var updatedSubcriber = subscriber.Clone();
            updatedSubcriber.Subscribed = false;
            var updateOperation = GetUpdateOperation(publication, updatedSubcriber);
            return updateOperation;
        }

        private SubscriberOperationDTO GetUpdateOperation(BulkEmailPublication publication, BulkEmailSubscriber subscriber)
        {
            var mailChimpSubscriber = new SubscriberDTO();
            
            mailChimpSubscriber.Subscribed = subscriber.Subscribed;
            mailChimpSubscriber.EmailAddress = subscriber.ThirdPartyContactId;
            mailChimpSubscriber.MergeFields = subscriber.MergeFields;

            var hashedEmail = CalculateMD5Hash(mailChimpSubscriber.EmailAddress.ToLower());

            var operation = new SubscriberOperationDTO();
            operation.Method = "PUT";
            operation.Path = string.Format("lists/{0}/members/{1}", publication.ThirdPartyPublicationId, hashedEmail);

            // TODO: Do we need to store this somewhere to verify subscriber processed successfully
            operation.OperationId = Guid.NewGuid().ToString();
            operation.Body = JsonConvert.SerializeObject(mailChimpSubscriber);

            return operation;
        }

        // From http://stackoverflow.com/questions/1207731/how-can-i-deserialize-json-to-a-simple-dictionarystring-string-in-asp-net
        private Dictionary<string, object> DeserializeToDictionary(string jo)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jo);
            var values2 = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> d in values)
            {
                // if (d.Value.GetType().FullName.Contains("Newtonsoft.Json.Linq.JObject"))
                if (d.Value is JObject)
                {
                    values2.Add(d.Key, DeserializeToDictionary(d.Value.ToString()));
                }
                else
                {
                    values2.Add(d.Key, d.Value);
                }
            }
            return values2;
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private void PullOptsFromThirdParty(List<BulkEmailPublication> publications)
        {
            var client = GetBulkEmailClient();

            foreach (var publication in publications)
            {
                // query mailchimp to get list activity
                var request = new RestRequest("lists/" + publication.ThirdPartyPublicationId + "/members?since_last_changed=" + publication.LastSuccessfulSync +
                    "&fields=members.id,members.email_address,members.status&activity=status", Method.GET);
                request.AddHeader("Content-Type", "application/json");

                try
                {

                    var response = client.Execute(request);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        // This will be addressed in US2861: MP/MailChimp Synch Error Handling 
                        // TODO: Should these be exceptions?
                        _logger.Error(string.Format("Http failed syncing opts for publication {0} with StatusCode = {1}", publication.PublicationId, response.StatusCode));
                        return;
                    }

                    var responseContent = response.Content;

                    var responseContentJson = JObject.Parse(responseContent);
                    List<BulkEmailSubscriberOptDTO> subscribersDTOs = JsonConvert.DeserializeObject<List<BulkEmailSubscriberOptDTO>>(responseContentJson["members"].ToString());
                    List<BulkEmailSubscriberOpt> subscribers = new List<BulkEmailSubscriberOpt>();

                    foreach (var subscriberDTO in subscribersDTOs)
                    {
                        subscriberDTO.PublicationID = publication.PublicationId;
                        subscribers.Add(Mapper.Map<BulkEmailSubscriberOpt>(subscriberDTO));
                    }

                    _bulkEmailRepository.SetSubscriberSyncs(_token, subscribers);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Opt-in sync code failed for publication {0} Detail: {1}", publication.PublicationId, ex));
                }
            }
        }
    }

}

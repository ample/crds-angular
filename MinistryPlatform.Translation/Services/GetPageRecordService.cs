﻿using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using MinistryPlatform.Translation.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;

namespace MinistryPlatform.Translation.Services
{
    /// <summary>
    /// Service class that fetchs a Record from Ministry Platform using the MP API
    /// </summary>
    ///
    public class GetPageRecordService
    {
        /// <summary>
        /// Get a specific record by pageId and recordId
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="recordId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static JArray GetRecord(int pageId, int recordId, String token)
        {
            var platformServiceClient = new PlatformService.PlatformServiceClient();
            PlatformService.SelectQueryResult result;

            using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
            {
                System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                result = platformServiceClient.GetPageRecord(pageId, recordId, false);
            }
            return MPFormatConversion.MPFormatToJson(result);
        }

        

        /// <summary>
        /// Get a specific record by pageId and recordId
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="recordId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Dictionary<string,object> GetRecordDict(int pageId, int recordId, String token)
        {

            var platformServiceClient = new PlatformService.PlatformServiceClient();
            PlatformService.SelectQueryResult result;

            using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
            {
                System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                result = platformServiceClient.GetPageRecord(pageId, recordId, false);
            }
            Dictionary<string, object> returnVal = MPFormatConversion.MPFormatToDictionary(result);
            return returnVal;
        }

        public static Dictionary<string, object> GetLookupRecord(int pageId, string search, String token, int maxNumberOfRecordsToReturn = 100)
        {
            var platformServiceClient = new PlatformService.PlatformServiceClient();
            PlatformService.SelectQueryResult result;

            using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
            {
                System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                result = platformServiceClient.GetPageLookupRecords(pageId, search, null, maxNumberOfRecordsToReturn);
                //vat tm = platformServiceClient.GetPageLookupRecords()
            }
            Dictionary<string, object> returnVal = MPFormatConversion.MPFormatToDictionary(result);
            return returnVal;

        }

        /// <summary>
        /// Get records based on only the pageId
        /// </summary>
        /// <param name="id">The page Id</param>
        /// <param name="token"> The token of the current user</param>
        /// <returns>/returns>
        public static JArray GetRecords(int id, string token)
        {
            try
            {
                var platformServiceClient = new PlatformService.PlatformServiceClient();
                PlatformService.SelectQueryResult result;

                using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                    result = platformServiceClient.GetPageRecords(id, "", "", 0);
                }
                return MPFormatConversion.MPFormatToJson(result);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return new JArray();
            }
            
        }

        public static JArray GetStates(string token)
        {
            try
            {
                var platformServiceClient = new PlatformService.PlatformServiceClient();
                PlatformService.SelectQueryResult result;

                using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                    result = platformServiceClient.GetPageRecords(Convert.ToInt32(ConfigurationManager.AppSettings["States"]), "", "", 0);
                }
                return MPFormatConversion.MPFormatToJson(result);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return new JArray();
            }
        }

        public static List<Dictionary<string, object>> GetSubPageRecords(int subPageId, int recordId, String token)
        {
            try
            {
                var platformServiceClient = new PlatformService.PlatformServiceClient();
                PlatformService.SelectQueryResult result;

                using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                    result = platformServiceClient.GetSubpageRecords(subPageId, recordId, string.Empty, string.Empty, 0);
                }

                var returnVal = MPFormatConversion.MPFormatToList(result);
                return returnVal;
                //return MPFormatConversion.MPFormatToJson(result);
                //return "";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
        }

        public static Dictionary<string, object> GetRecordsDict(int id, string token)
        {
            try
            {
                var platformServiceClient = new PlatformService.PlatformServiceClient();
                PlatformService.SelectQueryResult result;

                using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                    result = platformServiceClient.GetPageRecords(id, "", "", 0);
                }
                return MPFormatConversion.MPFormatToDictionary(result);
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        public static List<Dictionary<string, object>> GetLookupRecords(int id, string token)
        {
            try
            {
                var platformServiceClient = new PlatformService.PlatformServiceClient();
                PlatformService.SelectQueryResult result;

                using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                    //result = platformServiceClient.GetPageRecords(id, "", "", 0);
                    result = platformServiceClient.GetPageLookupRecords(id, "", "", 0);
                }
                return MPFormatConversion.MPFormatToList(result);
            }
            catch
            {
                return new List<Dictionary<string,object>>();
            }
        }

    }
}
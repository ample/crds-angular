﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;

namespace MinistryPlatform.Translation.Services
{
    public class GetMyRecords
    {
        public static List<MinistryPlatform.Models.Attribute> GetMyAttributes(int recordId, string token)
        {
            var subPageId = Convert.ToInt32(ConfigurationManager.AppSettings["MySkills"]);
            var subPageRecords = Services.GetPageRecordService.GetSubPageRecords(subPageId, recordId, token);
            var attributes = new List<MinistryPlatform.Models.Attribute>();

            foreach (var record in subPageRecords) {
                var attribute = new MinistryPlatform.Models.Attribute
                {
                    Attribute_Name = (string)record["Attribute_Name"],
                    Attribute_Type = (string)record["Attribute_Type"],
                    dp_FileID = (int?)record["dp_FileID"],
                    dp_RecordID = (int)record["dp_RecordID"],
                    dp_RecordName = (string)record["dp_RecordName"],
                    dp_RecordStatus = (int)record["dp_RecordStatus"],
                    dp_Selected = (int)record["dp_Selected"]
                };
                attributes.Add(attribute);
            }
            return attributes;

        }

        public static int CreateAttribute(MinistryPlatform.Models.Attribute attribute, int parentRecordId, string token)
        {
            try
            {
                var subPageId = Convert.ToInt32(ConfigurationManager.AppSettings["MySkills"]);
                var platformServiceClient = new PlatformService.PlatformServiceClient();
                PlatformService.SelectQueryResult result;

                using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);
                    attribute.Start_Date = DateTime.Now;
                    var dictionary = getDictionary(attribute);
                    return platformServiceClient.CreateSubpageRecord(subPageId, parentRecordId, dictionary, false);
                }
                //return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool DeleteAttribute(int recordId, string token)
        {
            // - 12/29 try using page 412 and record id, just like MP 
            try
            {
                var subPageId = Convert.ToInt32(ConfigurationManager.AppSettings["MySkills"]);
                var platformServiceClient = new PlatformService.PlatformServiceClient();
                PlatformService.SelectQueryResult result;

                using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)platformServiceClient.InnerChannel))
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Authorization", "Bearer " + token);

                    

                    var option = new PlatformService.DeleteOption
                    {
                        PageId = 455
                      
                    };
                    var options = new List<PlatformService.DeleteOption>{
                        option
                    };

                    platformServiceClient.DeletePageRecord(412, recordId, options.ToArray());

                    //platformServiceClient.DeleteSubpageRecord(subPageId, recordId, options.ToArray());
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static Dictionary<string, object> getDictionary(Object input)
        {
            var dictionary = input.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(input, null));
            return dictionary;
        }
    }
}

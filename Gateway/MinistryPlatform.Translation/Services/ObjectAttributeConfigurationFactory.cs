using System;
using System.Configuration;
using System.Web.Compilation;
using Crossroads.Utilities.Interfaces;

namespace MinistryPlatform.Translation.Services
{
    public static class ObjectAttributeConfigurationFactory
    {
        public static ObjectAttributeConfiguration ContactAttributeConfiguration()
        {
            return new ObjectAttributeConfiguration()
            {
                SubPage = int.Parse(ConfigurationManager.AppSettings["ContactAttributesSubPage"]),
                SelectedSubPage = int.Parse(ConfigurationManager.AppSettings["SelectedContactAttributes"]),
                TableName = "Contact"
            };
        }
        public static ObjectAttributeConfiguration MyContactAttributeConfiguration()
        {
            return new ObjectAttributeConfiguration()
            {
                SubPage = int.Parse(ConfigurationManager.AppSettings["MyContactAttributesSubPage"]),
                SelectedSubPage = int.Parse(ConfigurationManager.AppSettings["MyContactCurrentAttributesSubPageView"]),
                TableName = "Contact"
            };
        }
    }
}
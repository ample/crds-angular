﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MinistryPlatform.Translation.PlatformService;
using Newtonsoft.Json.Linq;

namespace MinistryPlatform.Translation.Repositories.Interfaces
{
    public interface IMinistryPlatformService
    {
        List<Dictionary<string, object>> GetLookupRecords(String token, int pageId, string search, string sort, int maxNumberOfRecordsToReturn = 100);

        List<Dictionary<string, object>> GetLookupRecords(int pageId, String token);
        
        Dictionary<string, object> GetLookupRecord(int pageId, string search, String token, int maxNumberOfRecordsToReturn = 100);
        
        SelectQueryResult GetRecords(int pageId, String token, String search = "", String sort = "");
        
        JArray GetRecordsArr(int pageId, String token, String search = "", String sort = "");
        
        List<Dictionary<string, object>> GetRecordsDict(int pageId, String token, String search = "", String sort = "");
        List<Dictionary<string, object>> GetRecordsDict(string pageKey, String token, String search = "", String sort = "");
        
        SelectQueryResult GetRecord(int pageId, int recordId, String token, bool quickadd = false);
        
        JArray GetRecordArr(int pageId, int recordId, String token, bool quickadd = false);
        
        Dictionary<string, object> GetRecordDict(int pageId, int recordId, String token, bool quickadd = false);
        
        List<Dictionary<string, object>> GetSubPageRecords(int subPageId, int recordId, String token);
        List<Dictionary<string, object>> GetSubPageRecords(string subPageKey, int recordId, String token);
        Dictionary<string, object> GetSubPageRecord(string subPageKey, int recordId, String token);
        
        int CreateRecord(int pageId, Dictionary<string, object> dictionary, String token,
            bool quickadd = false);

        int CreateRecord(string pageKey, Dictionary<string, object> dictionary, String token,
            bool quickadd = false);
        
        int CreateSubRecord(int subPageId, int parentRecordId, Dictionary<string, object> dictionary,
            String token, bool quickadd = false);

        int CreateSubRecord(string subPageKey, int parentRecordId, Dictionary<string, object> dictionary,
            String token, bool quickadd = false);

        Task<int> CreateSubRecordAsync(int subPageId,
                                       int parentRecordId,
                                       Dictionary<string, object> dictionary,
                                       String token,
                                       bool quickadd = false);


        int DeleteRecord(int pageId, int recordId, DeleteOption[] deleteOptions, String token);

        void UpdateRecord(int pageId, Dictionary<string, object> dictionary, String token);
        void UpdateSubRecord(int subPageId, Dictionary<string, object> dictionary, String token);

        void UpdateSubRecord(string subPageKey, Dictionary<string, object> subscription, string token);

        

        void UpdateFile(Int32 fileId, String fileName, String description, Boolean isDefaultImage, Int32 longestDimension, Byte[] file, String token);
        
        FileDescription CreateFile(String pageName,
                                          Int32 recordId,
                                          String fileName,
                                          String description,
                                          Boolean isDefaultImage,
                                          Int32 longestDimension,
                                          Byte[] file,
                                          String token);

        Stream GetFile(Int32 fileId, String token);

        FileDescription GetFileDescription(Int32 fileId, String token);

        FileDescription[] GetFileDescriptions(String pageName, Int32 recordId, String token);

        List<Dictionary<string, object>> GetSubpageViewRecords(int viewId, int recordId, string token, string searchString="", string sort="", int top=0);
        List<Dictionary<string, object>> GetSubpageViewRecords(string viewKey, int recordId, string token, string searchString = "", string sort = "", int top = 0);

        List<Dictionary<string, object>> GetPageViewRecords(int viewId, string token, string searchString = "", string sort = "", int top = 0);
        List<Dictionary<string, object>> GetPageViewRecords(string viewKey, string token, string searchString = "", string sort = "", int top = 0);

        List<Dictionary<string, object>> GetSelectionsForPageDict(int pageId, int selectionId, String token);
        List<Dictionary<string, object>> GetSelectionsDict(int selectionId, String token, String search = "", String sort = "");
        SelectQueryResult GetSelectionRecords(int selectionId, String token, String search = "", String sort = "");

        void CopyPageRecord(int pageId, int recordId, int[] subpages, System.DateTime[] startDateTimes, bool updateOriginalRecord, string token);

        void CopyPageRecordAsync(int pageId, int recordId, int[] subpages, System.DateTime[] startDateTimes, bool updateOriginalRecord, string token);

        UserInfo GetContactInfo(string token);
        void CompleteTask(string token, int taskId, bool rejected, string comments);
        void UpdateSubRecordAsync(int subPageId, Dictionary<string, object> attributeDictionary, string token);
        int CreateSelection(SelectionDescription selectionDescription, string token);
        void DeleteSelection(int selectionId, string token);
        void DeleteSelectionRecords(int selectionId, string token);
        void RemoveSelection(int selectionId, int[] records, String token);
        void AddToSelection(int selectionId, int[] recordIds, string token);
    }
}

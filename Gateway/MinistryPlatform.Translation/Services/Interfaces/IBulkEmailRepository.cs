using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.Translation.Services.Interfaces
{
    public interface IBulkEmailRepository
    {
        List<BulkEmailPublication> GetPublications(string token);
    }
}
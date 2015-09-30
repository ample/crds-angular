﻿using MinistryPlatform.Models;

namespace MinistryPlatform.Translation.Services.Interfaces
{
    public interface IUserService
    {
        MinistryPlatformUser GetByUserId(string userId);
        MinistryPlatformUser GetByAuthenticationToken(string authToken);
    }
}

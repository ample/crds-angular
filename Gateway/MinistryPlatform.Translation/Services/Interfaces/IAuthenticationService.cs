﻿using System;
using System.Collections.Generic;
using MinistryPlatform.Models;

namespace MinistryPlatform.Translation.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Boolean ChangePassword(string token, string emailAddress, string firstName, string lastName, string password, string mobilephone);

        /// <summary>
        /// Change a users password
        /// </summary>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Boolean ChangePassword(string token, string newPassword);

        //get token using logged in user's credentials
        Dictionary<string, object> authenticate(string username, string password);

        //Get ID of currently logged in user
        int GetContactId(string token);

        //Get Participant IDs of a contact
        Participant GetParticipantRecord(string token);

    }
}

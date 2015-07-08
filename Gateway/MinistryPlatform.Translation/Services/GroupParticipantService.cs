﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Crossroads.Utilities.Extensions;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Services.Interfaces;

namespace MinistryPlatform.Translation.Services
{
    public class GroupParticipantService : BaseService, IGroupParticipantService
    {
        private IDbConnection _dbConnection;

        public GroupParticipantService(IDbConnection dbConnection)
        {
            this._dbConnection = dbConnection;
        }

        public List<GroupServingParticipant> GetServingParticipants(List<int> participants, long from, long to, int loggedInContactId)
        {
            var connection = _dbConnection;
            try
            {
                connection.Open();

                var command = CreateSqlCommand(participants, from, to);
                command.Connection = connection;
                var reader = command.ExecuteReader();
                var groupServingParticipants = new List<GroupServingParticipant>();
                var rowNumber = 0;
                while (reader.Read())
                {
                    var rowContactId = reader.GetInt32(reader.GetOrdinal("Contact_ID"));
                    var loggedInUser = (loggedInContactId == rowContactId);
                    rowNumber = rowNumber + 1;
                    var participant = new GroupServingParticipant();
                    participant.ContactId = rowContactId;
                    participant.EventType = reader.GetString(reader.GetOrdinal("Event_Type"));
                    participant.EventTypeId = reader.GetInt32(reader.GetOrdinal("Event_Type_ID"));
                    participant.GroupRoleId = reader.GetInt32(reader.GetOrdinal("Group_Role_ID"));
                    participant.DomainId = reader.GetInt32(reader.GetOrdinal("Domain_ID"));
                    participant.EventId = reader.GetInt32(reader.GetOrdinal("Event_ID"));
                    participant.EventStartDateTime = (DateTime) reader["Event_Start_Date"];
                    participant.EventTitle = reader.GetString(reader.GetOrdinal("Event_Title"));
                    participant.Room = SafeString(reader, "Room");
                    participant.GroupId = reader.GetInt32(reader.GetOrdinal("Group_ID"));
                    participant.GroupName = reader.GetString(reader.GetOrdinal("Group_Name"));
                    participant.GroupPrimaryContactEmail = reader.GetString(reader.GetOrdinal("Primary_Contact_Email"));
                    participant.OpportunityId = reader.GetInt32(reader.GetOrdinal("Opportunity_ID"));
                    participant.OpportunityMaximumNeeded = SafeInt(reader, "Maximum_Needed");
                    participant.OpportunityMinimumNeeded = SafeInt(reader, "Minimum_Needed");
                    participant.OpportunityRoleTitle = reader.GetString(reader.GetOrdinal("Role_Title"));
                    participant.OpportunityShiftEnd = GetTimeSpan(reader, "Shift_End");
                    participant.OpportunityShiftStart = GetTimeSpan(reader, "Shift_Start");
                    participant.OpportunitySignUpDeadline = reader.GetInt32(reader.GetOrdinal("Sign_Up_Deadline"));
                    participant.DeadlinePassedMessage = (SafeInt32(reader, "Deadline_Passed_Message") ?? Convert.ToInt32(AppSettings("DefaultDeadlinePassedMessage")));
                    participant.OpportunityTitle = reader.GetString(reader.GetOrdinal("Opportunity_Title"));
                    participant.ParticipantNickname = reader.GetString(reader.GetOrdinal("Nickname"));
                    participant.ParticipantEmail = SafeString(reader, "Email_Address");
                    participant.ParticipantId = reader.GetInt32(reader.GetOrdinal("Participant_ID"));
                    participant.ParticipantLastName = reader.GetString(reader.GetOrdinal("Last_Name"));
                    participant.RowNumber = rowNumber;
                    participant.Rsvp = GetRsvp(reader, "Rsvp");
                    participant.LoggedInUser = loggedInUser;
                    groupServingParticipants.Add(participant);
                }
                return
                    groupServingParticipants.OrderBy(g => g.EventStartDateTime)
                        .ThenBy(g => g.GroupName)
                        .ThenByDescending(g => g.LoggedInUser)
                        .ThenBy(g => g.ParticipantNickname)
                        .ToList();
            }
            finally
            {
                connection.Close();
            }
        }

        private static IDbCommand CreateSqlCommand(IReadOnlyList<int> participants, long from, long to)
        {
            var fromDate = from == 0 ? DateTime.Today : from.FromUnixTime();
            var toDate = to == 0 ? DateTime.Today.AddDays(29) : to.FromUnixTime();

            const string query = @"SELECT *
                    FROM MinistryPlatform.dbo.vw_crds_Serving_Participants v 
                    WHERE ( {0} ) 
                    AND Event_Start_Date >= @from 
                    AND Event_Start_Date <= @to";

            var participantSqlParameters = participants.Select((s, i) => "@participant" + i.ToString()).ToArray();
            var participantParameters =
                participants.Select((s, i) => string.Format("(v.Participant_ID = @participant{0})", i)).ToList();
            var participantWhere = string.Join(" OR ", participantParameters);

            using (IDbCommand command = new SqlCommand(string.Format(query, participantWhere)))
            {
                command.Parameters.Add(new SqlParameter("@from", fromDate) {DbType = DbType.DateTime});
                command.Parameters.Add(new SqlParameter("@to", toDate) {DbType = DbType.DateTime});

                //Add values to each participant parameter
                command.CommandType = CommandType.Text;
                for (var i = 0; i < participantParameters.Count; i++)
                {
                    var sqlParameter = new SqlParameter(participantSqlParameters[i], participants[i]);
                    command.Parameters.Add(sqlParameter);
                }
                return command;
            }
        }

        private static bool? GetRsvp(IDataRecord record, string columnName)
        {
            var ordinal = record.GetOrdinal(columnName);
            bool? rsvp = false;
            if (record.IsDBNull(ordinal))
            {
                rsvp = null;
            }
            else if (record.GetInt32(ordinal) == 1)
            {
                rsvp = true;
            }
            return rsvp;
        }

        private static TimeSpan GetTimeSpan(IDataRecord record, string columnName)
        {
            var columnIndex = record.GetOrdinal(columnName);
            var reader = record as SqlDataReader;
            if (reader == null) throw new Exception("The DataReader is not a SqlDataReader");

            var myTimeSpan = reader.GetTimeSpan(columnIndex);
            return myTimeSpan;
        }

        private static string SafeString(IDataRecord record, string fieldName)
        {
            var ordinal = record.GetOrdinal(fieldName);
            return !record.IsDBNull(ordinal) ? record.GetString(ordinal) : null;
        }

        private static int? SafeInt(IDataRecord record, string fieldName)
        {
            var ordinal = record.GetOrdinal(fieldName);
            return !record.IsDBNull(ordinal) ? record.GetInt16(ordinal) : (int?) null;
        }

        private static int? SafeInt32(IDataRecord record, string fieldName)
        {
            var ordinal = record.GetOrdinal(fieldName);
            return !record.IsDBNull(ordinal) ? record.GetInt32(ordinal) : (int?)null;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Extensions;
using MinistryPlatform.Translation.Services.Interfaces;

namespace MinistryPlatform.Translation.Services
{
    public class FormSubmissionService : BaseService, IFormSubmissionService
    {
        private readonly int _formResponsePageId = AppSettings("FormResponsePageId");
        private readonly int _formAnswerPageId = AppSettings("FormAnswerPageId");
        private readonly int _formFieldCustomPage = AppSettings("AllFormFieldsView");

        private readonly IMinistryPlatformService _ministryPlatformService;
        private IDbConnection _dbConnection;

        public FormSubmissionService(IMinistryPlatformService ministryPlatformService, IDbConnection dbConnection, IAuthenticationService authenticationService, IConfigurationWrapper configurationWrapper)
            : base(authenticationService,configurationWrapper)
        {
            _ministryPlatformService = ministryPlatformService;
            _dbConnection = dbConnection;
        }

        public int GetFormFieldId(int crossroadsId)
        {
            var searchString = string.Format(",{0}", crossroadsId);
            var formFields = _ministryPlatformService.GetPageViewRecords(_formFieldCustomPage, ApiLogin(), searchString);

            var field = formFields.Single();
            var formFieldId = field.ToInt("Form_Field_ID");
            return formFieldId;
        }

        public List<FormField> GetFieldsForForm(int formId)
        {
            var searchString = string.Format(",,,,{0}", formId);
            var formFields = _ministryPlatformService.GetPageViewRecords(_formFieldCustomPage, ApiLogin(), searchString);

            return formFields.Select(formField => new FormField
            {
                CrossroadsId = formField.ToInt("CrossroadsId"),
                FieldLabel = formField.ToString("Field_Label"),
                FieldOrder = formField.ToInt("Field_Order"),
                FieldType = formField.ToString("Field_Type"),
                FormFieldId = formField.ToInt("Form_Field_ID"),
                FormId = formField.ToInt("Form_ID"),
                FormTitle = formField.ToString("Form_Title"),
                Required = formField.ToBool("Required")
            }).ToList();
        }

        public List<TripFormResponse> GetTripFormResponses(int selectionId)
        {
            var connection = _dbConnection;
            try
            {
                connection.Open();

                var command = CreateTripFormResponsesSqlCommand(selectionId);
                command.Connection = connection;
                var reader = command.ExecuteReader();
                var responses = new List<TripFormResponse>();
                while (reader.Read())
                {
                    var response = new TripFormResponse();
                    response.ContactId = reader.GetInt32(reader.GetOrdinal("Contact_ID"));
                    response.ParticipantId = reader.GetInt32(reader.GetOrdinal("Participant_ID"));
                    response.PledgeCampaignId = reader.GetInt32(reader.GetOrdinal("Pledge_Campaign_ID"));
                    response.EventId = reader.GetInt32(reader.GetOrdinal("Event_ID"));

                    responses.Add(response);
                }
                return responses;
            }
            finally
            {
                connection.Close();
            }
        }

        private static IDbCommand CreateTripFormResponsesSqlCommand(int selectionId)
        {
            const string query = @"SELECT fr.Contact_ID, fr.Pledge_Campaign_ID, pc.Event_ID, p.Participant_ID
                                  FROM [MinistryPlatform].[dbo].[dp_Selected_Records] sr
                                  INNER JOIN [MinistryPlatform].[dbo].[Form_Responses] fr on sr.Record_ID = fr.Form_Response_ID
                                  INNER JOIN [MinistryPlatform].[dbo].[Participants] p on fr.Contact_ID = p.Contact_ID
                                  LEFT OUTER JOIN [MinistryPlatform].[dbo].[Pledge_Campaigns] pc on fr.Pledge_Campaign_ID = pc.Pledge_Campaign_ID
                                  WHERE sr.Selection_ID = @selectionId";

            using (IDbCommand command = new SqlCommand(string.Format(query)))
            {
                command.Parameters.Add(new SqlParameter("@selectionId", selectionId) { DbType = DbType.Int32 });
                command.CommandType = CommandType.Text;
                return command;
            }
        }

        public int SubmitFormResponse(FormResponse form)
        {
            var token = ApiLogin();
            var responseId = CreateFormResponse(form.FormId, form.ContactId, form.OpportunityId,
                form.OpportunityResponseId, token);
            foreach (var answer in form.FormAnswers)
            {
                answer.FormResponseId = responseId;
                CreateFormAnswer(answer, token);
            }
            return responseId;
        }

        private int CreateFormResponse(int formId, int contactId, int opportunityId, int opportunityResponseId, string token)
        {
            var formResponse = new Dictionary<string, object>
            {
                {"Form_ID", formId},
                {"Response_Date", DateTime.Today},
                {"Contact_ID", contactId},
                {"Opportunity_ID", opportunityId},
                {"Opportunity_Response", opportunityResponseId}
            };

            var responseId = _ministryPlatformService.CreateRecord(_formResponsePageId, formResponse, token, true);
            return responseId;
        }

        private void CreateFormAnswer(FormAnswer answer, string token)
        {
            var formAnswer = new Dictionary<string, object>
            {
                {"Form_Response_ID", answer.FormResponseId},
                {"Form_Field_ID", answer.FieldId},
                {"Response", answer.Response},
                {"Opportunity_Response", answer.OpportunityResponseId}
            };

            _ministryPlatformService.CreateRecord(_formAnswerPageId, formAnswer, token, true);
        }
    }
}
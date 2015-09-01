using System;
using System.Collections.Generic;
using System.Linq;
using crds_angular.Models.Crossroads.Trip;
using crds_angular.Services.Interfaces;
using Crossroads.Utilities.Extensions;
using Crossroads.Utilities.Interfaces;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Services.Interfaces;
using IDonationService = MinistryPlatform.Translation.Services.Interfaces.IDonationService;
using IDonorService = MinistryPlatform.Translation.Services.Interfaces.IDonorService;
using IGroupService = MinistryPlatform.Translation.Services.Interfaces.IGroupService;
using PledgeCampaign = crds_angular.Models.Crossroads.Stewardship.PledgeCampaign;

namespace crds_angular.Services
{
    public class TripService : MinistryPlatformBaseService, ITripService
    {
        private readonly IEventParticipantService _eventParticipantService;
        private readonly IDonationService _donationService;
        private readonly IGroupService _groupService;
        private readonly IFormSubmissionService _formSubmissionService;
        private readonly IEventService _mpEventService;
        private readonly IDonorService _mpDonorService;
        private readonly IPledgeService _mpPledgeService;
        private readonly ICampaignService _campaignService;
        private readonly IPrivateInviteService _privateInviteService;
        private readonly ICommunicationService _communicationService;
        private readonly IContactService _contactService;
        private readonly IConfigurationWrapper _configurationWrapper;

        public TripService(IEventParticipantService eventParticipant,
                           IDonationService donationService,
                           IGroupService groupService,
                           IFormSubmissionService formSubmissionService,
                           IEventService eventService,
                           IDonorService donorService,
                           IPledgeService pledgeService,
                           ICampaignService campaignService,
                           IPrivateInviteService privateInviteService,
                           ICommunicationService communicationService,
                           IContactService contactService,
            IConfigurationWrapper configurationWrapper)
        {
            _eventParticipantService = eventParticipant;
            _donationService = donationService;
            _groupService = groupService;
            _formSubmissionService = formSubmissionService;
            _mpEventService = eventService;
            _mpDonorService = donorService;
            _mpPledgeService = pledgeService;
            _campaignService = campaignService;
            _privateInviteService = privateInviteService;
            _communicationService = communicationService;
            _contactService = contactService;
            _configurationWrapper = configurationWrapper;
        }

        public List<TripGroupDto> GetGroupsByEventId(int eventId)
        {
            var mpGroups = _groupService.GetGroupsForEvent(eventId);

            return mpGroups.Select(record => new TripGroupDto
            {
                GroupId = record.GroupId,
                GroupName = record.Name
            }).ToList();
        }

        public TripFormResponseDto GetFormResponses(int selectionId, int selectionCount, int formResponseId)
        {
            var tripApplicantResponses = GetTripAplicants(selectionId, selectionCount, formResponseId);

            var dto = new TripFormResponseDto();
            if (tripApplicantResponses.Errors != null)
            {
                if (tripApplicantResponses.Errors.Count != 0)
                {
                    dto.Errors = tripApplicantResponses.Errors;
                    return dto;
                }
            }

            //get groups for event, type = go trip
            var eventId = tripApplicantResponses.TripInfo.EventId;
            var eventGroups = _mpEventService.GetGroupsForEvent(eventId); //need to add check for group type?  TM 8/17

            dto.Applicants = tripApplicantResponses.Applicants;
            dto.Groups = eventGroups.Select(s => new TripGroupDto {GroupId = s.GroupId, GroupName = s.Name}).ToList();
            dto.Campaign = new PledgeCampaign
            {
                FundraisingGoal = tripApplicantResponses.TripInfo.FundraisingGoal,
                PledgeCampaignId = tripApplicantResponses.TripInfo.PledgeCampaignId
            };

            return dto;
        }

        public TripCampaignDto GetTripCampaign(int pledgeCampaignId)
        {
            var campaign = _campaignService.GetPledgeCampaign(pledgeCampaignId);
            return new TripCampaignDto()
            {
                Id = campaign.Id,
                Name = campaign.Name,
                FormId = campaign.FormId,
                FormName = campaign.FormTitle,
                YoungestAgeAllowed = campaign.YoungestAgeAllowed,
                RegistrationEnd = campaign.RegistrationEnd,
                RegistrationStart = campaign.RegistrationStart,
                AgeExceptions = campaign.AgeExceptions
            };
        }

        private TripApplicantResponse GetTripAplicants(int selectionId, int selectionCount, int formResponseId)
        {
            var responses = formResponseId == -1
                ? _formSubmissionService.GetTripFormResponsesBySelectionId(selectionId)
                : _formSubmissionService.GetTripFormResponsesByRecordId(formResponseId);

            var messages = ValidateResponse(selectionCount, formResponseId, responses);
            return messages.Count > 0
                ? new TripApplicantResponse {Errors = messages.Select(m => new TripToolError {Message = m}).ToList()}
                : FormatTripResponse(responses);
        }

        private static TripApplicantResponse FormatTripResponse(List<TripFormResponse> responses)
        {
            var tripInfo = responses
                .Select(r =>
                            r.EventId != null
                                ? (r.PledgeCampaignId != null
                                    ? new TripInfo
                                    {
                                        EventId = (int) r.EventId,
                                        FundraisingGoal = r.FundraisingGoal,
                                        PledgeCampaignId = (int) r.PledgeCampaignId
                                    }
                                    : null)
                                : null);

            var applicants = responses.Select(record => new TripApplicant
            {
                ContactId = record.ContactId,
                DonorId = record.DonorId,
                ParticipantId = record.ParticipantId
            }).ToList();

            var resp = new TripApplicantResponse
            {
                Applicants = applicants,
                TripInfo = tripInfo.First()
            };
            return resp;
        }

        private static List<string> ValidateResponse(int selectionCount, int formResponseId, List<TripFormResponse> responses)
        {
            var messages = new List<string>();
            if (responses.Count == 0)
            {
                messages.Add("Could not retrieve records from Ministry Platform");
            }

            if (formResponseId == -1 && responses.Count != selectionCount)
            {
                messages.Add("Error Retrieving Selection");
                messages.Add(string.Format("You selected {0} records in Ministry Platform, but only {1} were retrieved.", selectionCount, responses.Count));
                messages.Add("Please verify records you selected.");
            }

            var campaignCount = responses.GroupBy(x => x.PledgeCampaignId)
                .Select(x => new {Date = x.Key, Values = x.Distinct().Count()}).Count();
            if (campaignCount > 1)
            {
                messages.Add("Invalid Trip Selection - Multiple Campaigns Selected");
            }
            return messages;
        }

        public List<TripParticipantDto> Search(string search)
        {
            var results = _eventParticipantService.TripParticipants(search);

            var participants = results.GroupBy(r =>
                                                   new
                                                   {
                                                       r.ParticipantId,
                                                       r.EmailAddress,
                                                       r.Lastname,
                                                       r.Nickname
                                                   }).Select(x => new TripParticipantDto()
                                                   {
                                                       ParticipantId = x.Key.ParticipantId,
                                                       Email = x.Key.EmailAddress,
                                                       Lastname = x.Key.Lastname,
                                                       Nickname = x.Key.Nickname,
                                                       ShowGiveButton = true,
                                                       ShowShareButtons = false
                                                   }).ToDictionary(y => y.ParticipantId);

            foreach (var result in results)
            {
                var tp = new TripDto();
                tp.EventParticipantId = result.EventParticipantId;
                tp.EventEnd = result.EventEndDate.ToString("MMM dd, yyyy");
                tp.EventId = result.EventId;
                tp.EventStartDate = result.EventStartDate.ToUnixTime();
                tp.EventStart = result.EventStartDate.ToString("MMM dd, yyyy");
                tp.EventTitle = result.EventTitle;
                tp.EventType = result.EventType;
                tp.ProgramId = result.ParticipantId;
                tp.ProgramName = result.ProgramName;
                var participant = participants[result.ParticipantId];
                participant.Trips.Add(tp);
            }

            return participants.Values.OrderBy(o => o.Lastname).ThenBy(o => o.Nickname).ToList();
        }

        public MyTripsDto GetMyTrips(int contactId, string token)
        {
            var trips = _donationService.GetMyTripDistributions(contactId, token).OrderBy(t => t.EventStartDate);
            var myTrips = new MyTripsDto();

            var events = new List<Trip>();
            var eventIds = new List<int>();
            foreach (var trip in trips.Where(trip => !eventIds.Contains(trip.EventId)))
            {
                var eventParticipantId = 0;
                var eventParticipantIds = _eventParticipantService.TripParticipants("," + trip.EventId + ",,,,,,,,,,,," + contactId).FirstOrDefault();
                if (eventParticipantIds != null)
                {
                    eventParticipantId = eventParticipantIds.EventParticipantId;
                }
                eventIds.Add(trip.EventId);
                events.Add(new Trip
                {
                    EventId = trip.EventId,
                    EventType = trip.EventTypeId.ToString(),
                    EventTitle = trip.EventTitle,
                    EventStartDate = trip.EventStartDate.ToString("MMM dd, yyyy"),
                    EventEndDate = trip.EventEndDate.ToString("MMM dd, yyyy"),
                    FundraisingDaysLeft = Math.Max(0, (trip.CampaignEndDate - DateTime.Today).Days),
                    FundraisingGoal = trip.TotalPledge,
                    EventParticipantId = eventParticipantId
                });
            }

            foreach (var e in events)
            {
                var donations = trips.Where(d => d.EventId == e.EventId).OrderByDescending(d => d.DonationDate).ToList();
                foreach (var donation in donations)
                {
                    var gift = new TripGift();
                    if (donation.AnonymousGift)
                    {
                        gift.DonorNickname = "Anonymous";
                        gift.DonorLastName = "";
                    }
                    else
                    {
                        gift.DonorNickname = donation.DonorNickname ?? donation.DonorFirstName;
                        gift.DonorLastName = donation.DonorLastName;
                    }
                    gift.DonorEmail = donation.DonorEmail;
                    gift.DonationDate = donation.DonationDate.ToShortDateString();
                    gift.DonationAmount = donation.DonationAmount;
                    gift.RegisteredDonor = donation.RegisteredDonor;
                    e.TripGifts.Add(gift);
                    e.TotalRaised += donation.DonationAmount;
                }
                myTrips.MyTrips.Add(e);
            }
            return myTrips;
        }

        public List<int> SaveParticipants(SaveTripParticipantsDto dto)
        {
            var groupParticipants = new List<int>();
            var groupStartDate = DateTime.Now;
            const int groupRoleId = 16; // wondering if eventually this will become user input?
            var events = _groupService.getAllEventsForGroup(dto.GroupId);

            foreach (var applicant in dto.Applicants)
            {
                if (!_groupService.ParticipantGroupMember(dto.GroupId, applicant.ParticipantId))
                {
                    var groupParticipantId = _groupService.addParticipantToGroup(applicant.ParticipantId, dto.GroupId, groupRoleId, groupStartDate);
                    groupParticipants.Add(groupParticipantId);
                }

                CreatePledge(dto, applicant);

                EventRegistration(events, applicant);
            }

            return groupParticipants;
        }

        public int GeneratePrivateInvite(PrivateInviteDto dto, string token)
        {
            var invite = _privateInviteService.Create(dto.PledgeCampaignId, dto.EmailAddress, dto.RecipientName, token);
            var communication = PrivateInviteCommunication(invite);
            _communicationService.SendMessage(communication);

            return invite.PrivateInvitationId;

        }

        private Communication PrivateInviteCommunication(PrivateInvite invite)
        {
            var template = _communicationService.GetTemplate(12302);
            var fromContact = _contactService.GetContactById(4);
            var mergeData = SetMergeData(invite.PledgeCampaignIdText, invite.PledgeCampaignId, invite.InvitationGuid, invite.RecipientName);

            return new Communication
            {
                AuthorUserId = 5,
                DomainId = 1,
                EmailBody = template.Body,
                EmailSubject = template.Subject,
                FromContactId = fromContact.Contact_ID,
                FromEmailAddress = fromContact.Email_Address,
                ReplyContactId = fromContact.Contact_ID,
                ReplyToEmailAddress = fromContact.Email_Address,
                ToContactId = fromContact.Contact_ID,
                ToEmailAddress = invite.EmailAddress,
                MergeData = mergeData
            };
        }

        private Dictionary<string, object> SetMergeData(string tripTitle, int pledgeCampaignId, string inviteGuid, string participantName)
        {
            var mergeData = new Dictionary<string, object>
            {
                {"TripTitle", tripTitle},
                {"PledgeCampaignID", pledgeCampaignId},
                {"InviteGUID", inviteGuid},
                {"ParticipantName", participantName},
                {"BaseUrl",_configurationWrapper.GetConfigValue("BaseUrl")}
            };
            return mergeData;
        }

        private void CreatePledge(SaveTripParticipantsDto dto, TripApplicant applicant)
        {
            int donorId;
            var addPledge = true;

            if (applicant.DonorId != null)
            {
                donorId = (int) applicant.DonorId;
                addPledge = !_mpPledgeService.DonorHasPledge(dto.Campaign.PledgeCampaignId, donorId);
            }
            else
            {
                donorId = _mpDonorService.CreateDonorRecord(applicant.ContactId, null, DateTime.Now);
            }

            if (addPledge)
            {
                _mpPledgeService.CreatePledge(donorId, dto.Campaign.PledgeCampaignId, dto.Campaign.FundraisingGoal);
            }
        }

        private void EventRegistration(IEnumerable<Event> events, TripApplicant applicant)
        {
            foreach (var e in events)
            {
                if (_mpEventService.EventHasParticipant(e.EventId, applicant.ParticipantId) == false)
                {
                    _mpEventService.registerParticipantForEvent(applicant.ParticipantId, e.EventId);
                }
            }
        }

        public bool ValidatePrivateInvite(int pledgeCampaignId, string guid, string token)
        {
            return _privateInviteService.PrivateInviteValid(pledgeCampaignId, guid);
        }
    }
}

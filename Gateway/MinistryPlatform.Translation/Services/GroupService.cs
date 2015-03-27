﻿using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using MinistryPlatform.Models;
using MinistryPlatform.Translation.Exceptions;
using MinistryPlatform.Translation.Services.Interfaces;

namespace MinistryPlatform.Translation.Services
{
    public class GroupService : BaseService, IGroupService
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly int GroupsParticipantsPageId = Convert.ToInt32(AppSettings("GroupsParticipants"));
        private readonly int GroupsPageId = Convert.ToInt32(AppSettings("Groups"));
        private readonly int GroupsEventsPageId = Convert.ToInt32(AppSettings("GroupsEvents"));
        private readonly int EventsGroupsPageId = Convert.ToInt32(AppSettings("EventsGroups"));
        private readonly int GroupsSubgroupsPageId = Convert.ToInt32(AppSettings("GroupsSubgroups"));
        private readonly int GetMyServingTeamsPageId = Convert.ToInt32(AppSettings("MyServingTeams"));

        private IMinistryPlatformService ministryPlatformService;

        public GroupService(IMinistryPlatformService ministryPlatformService)
        {
            this.ministryPlatformService = ministryPlatformService;
        }

        public int addParticipantToGroup(int participantId, int groupId, int groupRoleId, DateTime startDate,
            DateTime? endDate = null, Boolean? employeeRole = false)
        {
            logger.Debug("Adding participant " + participantId + " to group " + groupId);

            // TODO Basing "Full" on Group_Is_Full flag, pending outcome of SPIKE: US1080
            Group g = getGroupDetails(groupId);
            if (g.Full)
            {
                throw (new GroupFullException(g));
            }

            var values = new Dictionary<string, object>
            {
                {"Participant_ID", participantId},
                {"Group_Role_ID", groupRoleId},
                {"Start_Date", startDate},
                {"End_Date", endDate},
                {"Employee_Role", employeeRole}
            };

            int groupParticipantId =
                WithApiLogin<int>(
                    apiToken =>
                    {
                        return
                            (ministryPlatformService.CreateSubRecord(GroupsParticipantsPageId, groupId, values, apiToken,
                                true));
                    });

            // TODO Should we set Group_Is_Full flag here, or will that be done by a trigger?  Pending SPIKE: US1080

            logger.Debug("Added participant " + participantId + " to group " + groupId + ": record id: " +
                         groupParticipantId);
            return (groupParticipantId);
        }

        public Group getGroupDetails(int groupId)
        {
            return (WithApiLogin<Group>(apiToken =>
            {
                logger.Debug("Getting group details for group " + groupId);
                var groupDetails = ministryPlatformService.GetRecordDict(GroupsPageId, groupId, apiToken, false);
                if (groupDetails == null)
                {
                    logger.Debug("No group found for group id " + groupId);
                    return (null);
                }
                Group g = new Group();

                object gid = null;
                groupDetails.TryGetValue("Group_ID", out gid);
                if (gid != null)
                {
                    g.GroupId = (int) gid;
                }

                object gn = null;
                groupDetails.TryGetValue("Group_Name", out gn);
                if (gn != null)
                {
                    g.Name = (string) gn;
                }

                object gsz = null;
                groupDetails.TryGetValue("Target_Size", out gsz);
                if (gsz != null)
                {
                    g.TargetSize = (short) gsz;
                }

                object gf = null;
                groupDetails.TryGetValue("Group_Is_Full", out gf);
                if (gf != null)
                {
                    g.Full = (Boolean) gf;
                }

                object gwl = null;
                groupDetails.TryGetValue("Enable_Waiting_List", out gwl);
                if (gwl != null)
                {
                    g.WaitList = (Boolean) gwl;
                }

                if (g.WaitList)
                {
                    var subGroups = ministryPlatformService.GetSubPageRecords(GroupsSubgroupsPageId, groupId,
                        apiToken);
                    if (subGroups != null)
                    {
                        foreach (var i in subGroups)
                        {
                            if (i.ContainsValue("Wait List"))
                            {
                                object gd = null;
                                i.TryGetValue("dp_RecordID", out gd);
                                g.WaitListGroupId = (int) gd;
                                break;
                            }
                        }
                    }
                    else
                    {
                        logger.Debug("No wait list found for group id " + groupId);
                    }
                }

                logger.Debug("Getting participants for group " + groupId);
                var participants = ministryPlatformService.GetSubPageRecords(GroupsParticipantsPageId, groupId, apiToken);
                if (participants != null && participants.Count > 0)
                {
                    foreach (Dictionary<string, object> p in participants)
                    {
                        object pid = null;
                        p.TryGetValue("Participant_ID", out pid);
                        if (pid != null)
                        {
                            g.Participants.Add((int) pid);
                        }
                    }
                }
                else
                {
                    logger.Debug("No participants found for group id " + groupId);
                }

                logger.Debug("Group details: " + g);
                return (g);
            }));
        }

        public IList<Event> getAllEventsForGroup(int groupId)
        {
            string apiToken = apiLogin();

            // Get all the Groups->Events sub-page records
            var mpEvents = ministryPlatformService.GetSubPageRecords(GroupsEventsPageId, groupId, apiToken);
            if (mpEvents == null || mpEvents.Count == 0)
            {
                return (null);
            }

            var events = new List<Event>();
            foreach (Dictionary<string, object> e in mpEvents)
            {
                // The dp_RecordID in this case is the key of the Event_Group, now need to get the Event_ID
                object recordId = null;
                if (e.TryGetValue("dp_RecordID", out recordId))
                {
                    var eventGroup = ministryPlatformService.GetRecordDict(EventsGroupsPageId, (int) recordId, apiToken,
                        false);
                    if (eventGroup == null)
                    {
                        continue;
                    }

                    object eventId = null;
                    if (eventGroup.TryGetValue("Event_ID", out eventId))
                    {
                        Event evt = new Event();
                        evt.EventId = (int) eventId;
                        events.Add(evt);
                    }
                }
            }
            return (events);
        }

        public List<Group> GetMyServingTeams(int contactId, string token)
        {
            var searchString = ",,,," + contactId;
            var teams = ministryPlatformService.GetPageViewRecords(GetMyServingTeamsPageId, token, searchString);
            var groups = new List<Group>();
            foreach (var team in teams)
            {
                var group = new Group
                {
                    GroupId = (int) team["Group_ID"],
                    Name = (string) team["Group_Name"],
                    GroupRole = (string) team["Role_Title"]
                };
                groups.Add(group);
            }
            return groups;
        }

        public bool checkIfUserInGroup(int participantId, IList<int> groupParticipants)
        {
            return groupParticipants.Contains(participantId);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Crossroads.Utilities.Interfaces;
using log4net;
using MinistryPlatform.Translation.Extensions;
using MinistryPlatform.Translation.Models;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class ContactRepository : BaseRepository, IContactRepository
    {
        private readonly int _addressesPageId;
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly int _congregationDefaultId;
        private readonly int _contactsPageId;
        private readonly int _householdDefaultSourceId;
        private readonly int _householdPositionDefaultId;
        private readonly int _householdsPageId;
        private readonly ILog _logger = LogManager.GetLogger(typeof (ContactRepository));

        private readonly IMinistryPlatformService _ministryPlatformService;
        private readonly int _participantsPageId;
        private readonly int _securityRolesSubPageId;

        public ContactRepository(IMinistryPlatformService ministryPlatformService, IAuthenticationRepository authenticationService, IConfigurationWrapper configuration)
            : base(authenticationService, configuration)
        {
            _ministryPlatformService = ministryPlatformService;
            _configurationWrapper = configuration;

            _householdsPageId = configuration.GetConfigIntValue("Households");
            _securityRolesSubPageId = configuration.GetConfigIntValue("SecurityRolesSubPageId");
            _congregationDefaultId = configuration.GetConfigIntValue("Congregation_Default_ID");
            _householdDefaultSourceId = configuration.GetConfigIntValue("Household_Default_Source_ID");
            _householdPositionDefaultId = configuration.GetConfigIntValue("Household_Position_Default_ID");
            _addressesPageId = configuration.GetConfigIntValue("Addresses");
            _contactsPageId = configuration.GetConfigIntValue("Contacts");
            _participantsPageId = configuration.GetConfigIntValue("Participants");
        }

        public int CreateContactForGuestGiver(string emailAddress, string displayName)
        {
            var contactDonor = new MpContactDonor
            {
                Details = new MpContactDetails
                {
                    DisplayName = displayName,
                    EmailAddress = emailAddress
                }
            };
            return (CreateContact(contactDonor));
        }

        public int CreateContactForNewDonor(MpContactDonor mpContactDonor)
        {
            return (CreateContact(mpContactDonor));
        }

        public int CreateContactForSponsoredChild(string firstName, string lastName, string town, string idCard)
        {
            var householdId = CreateAddressHouseholdForSponsoredChild(town, lastName);

            var contact = new MpMyContact
            {
                First_Name = firstName,
                Last_Name = lastName,
                ID_Number = idCard,
                Household_ID = householdId
            };

            return CreateContact(contact);
        }

        public MpMyContact GetContactById(int contactId)
        {
            var searchString = string.Format(",\"{0}\"", contactId);

            var pageViewRecords = _ministryPlatformService.GetPageViewRecords("AllIndividualsWithContactId", ApiLogin(), searchString);

            if (pageViewRecords.Count > 1)
            {
                throw new ApplicationException("GetContactById returned multiple records");
            }

            return ParseProfileRecord(pageViewRecords[0]);
        }

        public MpMyContact GetContactByIdCard(string idCard)
        {
            var searchString = string.Format(new String(',', 33) + "\"{0}\"", idCard);
            var pageViewRecords = _ministryPlatformService.GetPageViewRecords("AllIndividualsWithContactId", ApiLogin(), searchString);
            if (pageViewRecords.Count > 1)
            {
                throw new ApplicationException("GetContactById returned multiple records");
            }

            if (pageViewRecords.Count == 0)
            {
                return null;
            }

            return ParseProfileRecord(pageViewRecords[0]);
        }

        public MpMyContact GetContactByParticipantId(int participantId)
        {
            var token = ApiLogin();
            var searchString = string.Format("{0},", participantId);
            var contacts = _ministryPlatformService.GetPageViewRecords("ContactByParticipantId", token, searchString);
            var c = contacts.SingleOrDefault();
            if (c == null)
            {
                return null;
            }
            var contact = new MpMyContact
            {
                Contact_ID = c.ToInt("Contact_ID"),
                Email_Address = c.ToString("Email_Address"),
                Last_Name = c.ToString("Last_Name"),
                First_Name = c.ToString("First_Name")
            };
            return contact;
        }

        public string GetContactEmail(int contactId)
        {
            try
            {
                var recordsDict = _ministryPlatformService.GetRecordDict(_contactsPageId, contactId, ApiLogin());

                var contactEmail = recordsDict["Email_Address"].ToString();

                return contactEmail;
            }
            catch (NullReferenceException)
            {
                _logger.Debug(string.Format("Trying to email address of {0} failed", contactId));
                return string.Empty;
            }
        }

        public int GetContactIdByEmail(string email)
        {
            var records = _ministryPlatformService.GetRecordsDict(_configurationWrapper.GetConfigIntValue("Contacts"), ApiLogin(), (email));
            if (records.Count > 1)
            {
                throw new Exception("User email did not return exactly one user record");
            }
            if (records.Count < 1)
            {
                return 0;
            }

            var record = records[0];
            return record.ToInt("dp_RecordID");
        }

        public int GetContactIdByParticipantId(int participantId)
        {
            var token = ApiLogin();
            var participant = _ministryPlatformService.GetRecordDict(_participantsPageId, participantId, token);
            return participant.ToInt("Contact_ID");
        }

        public IList<int> GetContactIdByRoleId(int roleId, string token)
        {
            var records = _ministryPlatformService.GetSubPageRecords(_securityRolesSubPageId, roleId, token);

            return records.Select(record => (int) record["Contact_ID"]).ToList();
        }

        public List<MpHouseholdMember> GetHouseholdFamilyMembers(int householdId)
        {
            var token = ApiLogin();
            var familyRecords = _ministryPlatformService.GetSubpageViewRecords("HouseholdMembers", householdId, token);
            var family = familyRecords.Select(famRec => new MpHouseholdMember
            {
                ContactId = famRec.ToInt("Contact_ID"),
                FirstName = famRec.ToString("First_Name"),
                Nickname = famRec.ToString("Nickname"),
                LastName = famRec.ToString("Last_Name"),
                DateOfBirth = famRec.ToDate("Date_of_Birth"),
                HouseholdPosition = famRec.ToString("Household_Position"),
                StatementTypeId = famRec.ContainsKey("Statement_Type_ID") ? famRec.ToInt("Statement_Type_ID") : (int?) null,
                DonorId = famRec.ContainsKey("Donor_ID") ? famRec.ToInt("Donor_ID") : (int?) null
            }).ToList();
            return family;
        }


        public MpMyContact GetMyProfile(string token)
        {
            var recordsDict = _ministryPlatformService.GetRecordsDict("MyProfile", token);

            if (recordsDict.Count > 1)
            {
                throw new ApplicationException("GetMyProfile returned multiple records");
            }

            var contact = ParseProfileRecord(recordsDict[0]);

            return contact;
        }

        public List<Dictionary<string, object>> StaffContacts()
        {
            var token = ApiLogin();
            var userRoleStaff = _configurationWrapper.GetConfigIntValue("StaffUserRoleId");
            var records = _ministryPlatformService.GetSubpageViewRecords("UserDetails", userRoleStaff, token);
            return records;
        }

        public void UpdateContact(int contactId, Dictionary<string, object> profileDictionary)
        {
            var retValue = WithApiLogin<int>(token =>
            {
                try
                {
                    _ministryPlatformService.UpdateRecord(_configurationWrapper.GetConfigIntValue("Contacts"), profileDictionary, token);
                    return 1;
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Error Saving mpContact: " + e.Message);
                }
            });
        }

        public void UpdateContact(int contactId,
                                  Dictionary<string, object> profileDictionary,
                                  Dictionary<string, object> householdDictionary,
                                  Dictionary<string, object> addressDictionary)
        {
            WithApiLogin<int>(token =>
            {
                try
                {
                    _ministryPlatformService.UpdateRecord(_configurationWrapper.GetConfigIntValue("Contacts"), profileDictionary, token);
                    if (addressDictionary["Address_ID"] != null)
                    {
                        //address exists, update it
                        _ministryPlatformService.UpdateRecord(_configurationWrapper.GetConfigIntValue("Addresses"), addressDictionary, token);
                    }
                    else
                    {
                        //address does not exist, create it, then attach to household
                        var addressId = _ministryPlatformService.CreateRecord(_configurationWrapper.GetConfigIntValue("Addresses"), addressDictionary, token);
                        householdDictionary.Add("Address_ID", addressId);
                    }
                    _ministryPlatformService.UpdateRecord(_configurationWrapper.GetConfigIntValue("Households"), householdDictionary, token);
                    return 1;
                }
                catch (Exception e)
                {
                    return 0;
                }
            });
        }

        private static MpMyContact ParseProfileRecord(Dictionary<string, object> recordsDict)
        {
            var contact = new MpMyContact
            {
                Address_ID = recordsDict.ToNullableInt("Address_ID"),
                Address_Line_1 = recordsDict.ToString("Address_Line_1"),
                Address_Line_2 = recordsDict.ToString("Address_Line_2"),
                Congregation_ID = recordsDict.ToNullableInt("Congregation_ID"),
                Household_ID = recordsDict.ToInt("Household_ID"),
                Household_Name = recordsDict.ToString("Household_Name"),
                City = recordsDict.ToString("City"),
                State = recordsDict.ToString("State"),
                County = recordsDict.ToString("County"),
                Postal_Code = recordsDict.ToString("Postal_Code"),
                Contact_ID = recordsDict.ToInt("Contact_ID"),
                Date_Of_Birth = recordsDict.ToDateAsString("Date_of_Birth"),
                Email_Address = recordsDict.ToString("Email_Address"),
                Employer_Name = recordsDict.ToString("Employer_Name"),
                First_Name = recordsDict.ToString("First_Name"),
                Foreign_Country = recordsDict.ToString("Foreign_Country"),
                Gender_ID = recordsDict.ToNullableInt("Gender_ID"),
                Home_Phone = recordsDict.ToString("Home_Phone"),
                Last_Name = recordsDict.ToString("Last_Name"),
                Maiden_Name = recordsDict.ToString("Maiden_Name"),
                Marital_Status_ID = recordsDict.ToNullableInt("Marital_Status_ID"),
                Middle_Name = recordsDict.ToString("Middle_Name"),
                Mobile_Carrier = recordsDict.ToNullableInt("Mobile_Carrier_ID"),
                Mobile_Phone = recordsDict.ToString("Mobile_Phone"),
                Nickname = recordsDict.ToString("Nickname"),
                Age = recordsDict.ToInt("Age"),
                Passport_Number = recordsDict.ToString("Passport_Number"),
                Passport_Country = recordsDict.ToString("Passport_Country"),
                Passport_Expiration = ParseExpirationDate(recordsDict.ToNullableDate("Passport_Expiration")),
                Passport_Firstname = recordsDict.ToString("Passport_Firstname"),
                Passport_Lastname = recordsDict.ToString("Passport_Lastname"),
                Passport_Middlename = recordsDict.ToString("Passport_Middlename")
            };
            if (recordsDict.ContainsKey("Participant_Start_Date"))
            {
                contact.Participant_Start_Date = recordsDict.ToDate("Participant_Start_Date");
            }
            if (recordsDict.ContainsKey("Attendance_Start_Date"))
            {
                contact.Attendance_Start_Date = recordsDict.ToNullableDate("Attendance_Start_Date");
            }

            if (recordsDict.ContainsKey("ID_Card"))
            {
                contact.ID_Number = recordsDict.ToString("ID_Card");
            }
            return contact;
        }

        private static string ParseExpirationDate(DateTime? date)
        {
            if (date != null)
            {
                return String.Format("{0:MM/dd/yyyy}", date);
            }
            return null;
        }

        private static string ParseAnniversaryDate(DateTime? anniversary)
        {
            if (anniversary != null)
            {
                return String.Format("{0:MM/yyyy}", anniversary);
            }
            return null;
        }

        public MpContact CreateSimpleContact(string firstName, string lastName, string email, string dob, string mobile)
        {
            var contactId = CreateContact(new MpMyContact
            {
                Date_Of_Birth = dob,
                First_Name = firstName,
                Last_Name = lastName,
                Email_Address = email,
                Mobile_Phone = mobile
            });
            return new MpContact() { ContactId = contactId, EmailAddress = email};
        }

        private int CreateAddressHouseholdForSponsoredChild(string town, string lastName)
        {
            if (!String.IsNullOrEmpty(town))
            {
                var address = new MpPostalAddress
                {
                    City = town,
                    Line1 = "Not Known"
                };
                return CreateHouseholdAndAddress(lastName, address, ApiLogin());
            }
            else
            {
                return -1;
            }
        }

        private int CreateContact(MpMyContact contact)
        {
            var contactDictionary = new Dictionary<string, object>
            {
                {"Company", false},
                {"Last_Name", contact.Last_Name},
                {"First_Name", contact.First_Name},
                {"Email_Address", contact.Email_Address },
                {"Display_Name", contact.Last_Name + ", " + contact.First_Name},
                {"Nickname", contact.First_Name},
                {"ID_Card", contact.ID_Number}
            };

            if (contact.Household_ID > 0)
            {
                contactDictionary.Add("HouseHold_ID", contact.Household_ID);
                contactDictionary.Add("Household_Position_ID", _householdPositionDefaultId);
            }

            if (contact.Mobile_Phone != string.Empty)
            {
                contactDictionary.Add("Mobile_Phone", contact.Mobile_Phone);
            }

            if (contact.Date_Of_Birth != string.Empty)
            {
                contactDictionary.Add("Date_Of_Birth", contact.Date_Of_Birth);
            }

            try
            {
                var token = ApiLogin();
                return (_ministryPlatformService.CreateRecord(_contactsPageId, contactDictionary, token, false));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error creating Contact, firstName: {0} lastName: {1} idCard: {2}",
                                        contact.First_Name,
                                        contact.Last_Name,
                                        contact.ID_Number);
                _logger.Error(msg, e);
                throw (new ApplicationException(msg, e));
            }
        }

        private int CreateContact(MpContactDonor mpContactDonor)
        {
            var token = ApiLogin();

            var emailAddress = mpContactDonor.Details.EmailAddress;
            var displayName = mpContactDonor.Details.DisplayName;
            int? householdId = null;
            if (mpContactDonor.Details.HasAddress)
            {
                try
                {
                    householdId = CreateHouseholdAndAddress(displayName, mpContactDonor.Details.Address, token);
                }
                catch (Exception e)
                {
                    var msg = string.Format("Error creating household and address for emailAddress: {0} displayName: {1}",
                                            emailAddress,
                                            displayName);
                    _logger.Error(msg, e);
                    throw (new ApplicationException(msg, e));
                }
            }

            var contactDictionary = new Dictionary<string, object>
            {
                {"Email_Address", emailAddress},
                {"Company", false},
                {"Display_Name", displayName},
                {"Nickname", displayName},
                {"Household_ID", householdId},
                {"Household_Position_ID", _householdPositionDefaultId}
            };

            try
            {
                return (_ministryPlatformService.CreateRecord(_contactsPageId, contactDictionary, token));
            }
            catch (Exception e)
            {
                var msg = string.Format("Error creating mpContact, emailAddress: {0} displayName: {1}",
                                        emailAddress,
                                        displayName);
                _logger.Error(msg, e);
                throw (new ApplicationException(msg, e));
            }
        }

        private int CreateHouseholdAndAddress(string householdName, MpPostalAddress address, string apiToken)
        {
            var addressDictionary = new Dictionary<string, object>
            {
                {"Address_Line_1", address.Line1},
                {"Address_Line_2", address.Line2},
                {"City", address.City},
                {"State/Region", address.State},
                {"Postal_Code", address.PostalCode}
            };
            var addressId = _ministryPlatformService.CreateRecord(_addressesPageId, addressDictionary, apiToken);

            var householdDictionary = new Dictionary<string, object>
            {
                {"Household_Name", householdName},
                {"Congregation_ID", _congregationDefaultId},
                {"Household_Source_ID", _householdDefaultSourceId},
                {"Address_ID", addressId}
            };

            return (_ministryPlatformService.CreateRecord(_householdsPageId, householdDictionary, apiToken));
        }
    }
}
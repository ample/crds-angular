﻿using System;

namespace MinistryPlatform.Models
{
    /// <summary>
    /// Represents a MP Contact and potentially a Donor associated to the contact.  
    /// This could be for a registered User who has given online, or a Guest giver 
    /// who has previously given online, or for someone who has given cash or checks 
    /// directly without ever logging in.  There are various properties on this object
    /// that can be referenced to determine the state of the ContactDonor.
    /// </summary>
    public class ContactDonor
    {
        public int ContactId { get; set; }
        public int DonorId { get; set; }
        public string StatementFreq { get; set; }
        public string StatementType { get; set; }
        public string StatementMethod { get; set; }
        public DateTime SetupDate { get; set; }
        public string ProcessorId { get; set; }
        public string Email { get; set; }
        public bool RegisteredUser;

        /// <summary>
        /// Returns true if this ContactDonor represents an existing MP Contact, false if not.
        /// </summary>
        public bool ExistingContact { get { return (ContactId > 0); } }

        /// <summary>
        /// Returns true if this ContactDonor represents an existing MP Donor, false if not.
        /// An existing Donor implies that there is an existing Contact.
        /// </summary>
        public bool ExistingDonor { get { return (ExistingContact && DonorId > 0); } }

        /// <summary>
        /// Returns true if this ContactDonor has a record setup at the payment processor, false if not.
        /// </summary>
        public bool HasPaymentProcessorRecord { get { return (!String.IsNullOrWhiteSpace(ProcessorId)); } }
    }
}

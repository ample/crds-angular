using System;

namespace crds_angular.Models.Crossroads
{
    public class CreateDonationDTO
    {
        public string program_id { get; set; }
        public int amount { get; set; }
        public int donor_id { get; set; }
        public string email_address { get; set; }
        public string pymt_type { get; set; }
    }
}
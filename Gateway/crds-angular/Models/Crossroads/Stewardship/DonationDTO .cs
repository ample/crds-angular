using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace crds_angular.Models.Crossroads.Stewardship
{
    // ReSharper disable once InconsistentNaming
    public class DonationDTO
    {
        [JsonProperty(PropertyName = "program_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ProgramId { get; set; }
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("donation_id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "batch_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? BatchId { get; set; }

        [JsonProperty("status"), JsonConverter(typeof(StringEnumConverter))]
        public DonationStatus Status { get; set; }

        [JsonProperty("donation_date")]
        public DateTime DonationDate { get; set; }

        #region Distributions Property
        [JsonIgnore]
        private readonly List<DonationDistributionDTO> _distributions = new List<DonationDistributionDTO>();
        [JsonProperty(PropertyName = "distributions", NullValueHandling = NullValueHandling.Ignore)]
        public List<DonationDistributionDTO> Distributions { get { return (_distributions); } }
        #endregion

        [JsonProperty(PropertyName = "source_type", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(StringEnumConverter))]
        public PaymentType SourceType { get; set; }

        [JsonProperty(PropertyName = "source_type_description", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceTypeDescription { get; set; }

        [JsonProperty(PropertyName = "card_type", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(StringEnumConverter))]
        public CreditCardType? CardType { get; set; }
    }

    public enum DonationStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 1,
        [EnumMember(Value = "Deposited")]
        Deposited = 2,
        [EnumMember(Value = "Declined")]
        Declined = 3,
        [EnumMember(Value = "Succeeded")]
        Succeeded = 4
    }

    public enum PaymentType
    {
        [EnumMember(Value = "Adjustment")]
        Adjustment = 7,
        [EnumMember(Value = "Bank")]
        Bank = 5,
        [EnumMember(Value = "Cash")]
        Cash = 2,
        [EnumMember(Value = "Check")]
        Check = 1,
        [EnumMember(Value = "CreditCard")]
        CreditCard = 4,
        [EnumMember(Value = "InternalScholarship")]
        InternalScholarship = 9,
        [EnumMember(Value = "MoneyOrder")]
        MoneyOrder = 12,
        [EnumMember(Value = "NonCashAsset")]
        NonCashAsset = 6,
        [EnumMember(Value = "Other")]
        Other = 11,
        [EnumMember(Value = "Transfer")]
        Transfer = 13
    }

    public enum CreditCardType
    {
        [EnumMember(Value = "Visa")]
        Visa,
        [EnumMember(Value = "MasterCard")]
        MasterCard,
        [EnumMember(Value = "Discover")]
        Discover,
        [EnumMember(Value = "AmericanExpress")]
        AmericanExpress
    }
}
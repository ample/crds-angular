﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace crds_angular.Models.Crossroads.Stewardship
{
    public class DonationSourceDTO
    {
        [JsonIgnore]
        public string ProcessorAccountId { get; set; }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(StringEnumConverter))]
        public PaymentType SourceType { get; set; }

        [JsonProperty(PropertyName = "account_holder_name", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountHolderName { get; set; }

        [JsonProperty(PropertyName = "account_holder_type", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountHolderType { get; set; }

        [JsonProperty(PropertyName = "routing", NullValueHandling = NullValueHandling.Ignore)]
        public string RoutingNumber { get; set; }

        [JsonProperty(PropertyName = "last4", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountNumberLast4 { get; set; }

        [JsonProperty(PropertyName = "check_number", NullValueHandling = NullValueHandling.Ignore)]
        public string CheckNumber { get; internal set; }

        [JsonProperty(PropertyName = "brand", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(StringEnumConverter))]
        public CreditCardType? CardType { get; set; }

        [JsonProperty(PropertyName = "payment_processor_id", NullValueHandling = NullValueHandling.Ignore)]
        public string PaymentProcessorId { get; set; }

        [JsonProperty(PropertyName = "exp_date", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ExpirationDate;

        [JsonProperty(PropertyName = "address_zip", NullValueHandling = NullValueHandling.Ignore)]
        public string PostalCode;
    }
}
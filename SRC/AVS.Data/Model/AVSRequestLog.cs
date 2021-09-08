﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Data.Model
{
    [Table("AVSRequestLOG")]
    public class AVSRequestLOG
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
        public string Currency { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DataCreated { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool EnableUpdate { get; set; }
    }
}

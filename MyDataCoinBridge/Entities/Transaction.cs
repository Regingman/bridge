﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Entities
{
    public class Transaction
    {
        [Key]
        public Guid TxId { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public decimal Amount { get; set; }

        public DateTime TxDate { get; set; }

        public int Type { get; set; }
    }

    public enum TransactionType
    {
        Advertising = 1,
        Refferal = 2,
        Send = 3
    }
}

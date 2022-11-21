using System;

namespace MyDataCoinBridge.Models
{
    public class TransactionRequest
    {
        public Guid TxId { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public decimal Amount { get; set; }

        public DateTime TxDate { get; set; }

        public int Type { get; set; }
    }
}

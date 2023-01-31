using System;
using System.Collections.Generic;

namespace MyDataCoinBridge.Models.Transaction
{
    public class AllDataFromStatisticRequest
    {
        public List<TotalEarned> TotalEarneds { get; set; }
        public List<TransactionRequest> TotalTransactions { get; set; }
    }

    public class TransactionRequest
    {
        public Guid TxId { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public decimal Amount { get; set; }

        public DateTime TxDate { get; set; }

        public string Type { get; set; }
    }

    public class TotalEarned
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}

namespace Customerapp.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Transaction
    {
        public int TransactionId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; } // Deposit / Withdraw

        public DateTime TransactionDate { get; set; }

        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }
    }
}

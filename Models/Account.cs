namespace Customerapp.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Account
    {
        public int AccountId { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string AccountType { get; set; } // Savings / Current

        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}

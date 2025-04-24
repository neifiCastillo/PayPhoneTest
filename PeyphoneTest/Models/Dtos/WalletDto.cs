namespace PeyphoneTest.Models.Dtos
{
    public class WalletDto
    {
        public string DocumentId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}

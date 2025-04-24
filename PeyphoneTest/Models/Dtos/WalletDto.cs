namespace PeyphoneTest.Models.Dtos
{
    public class WalletDto
    {
        public int? Id { get; set; }
        public string DocumentId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

    }
}

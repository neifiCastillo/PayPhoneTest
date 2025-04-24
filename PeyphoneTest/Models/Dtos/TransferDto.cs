namespace PeyphoneTest.Models.Dtos
{
    public class TransferDto
    {
        public int FromWalletId { get; set; }
        public int ToWalletId { get; set; }
        public decimal Amount { get; set; }
    }
}

namespace PeyphoneTest.Models.Dtos
{
    public class HistoryMovementDto
    {

        public decimal Amount { get; set; }

        public string Type { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}

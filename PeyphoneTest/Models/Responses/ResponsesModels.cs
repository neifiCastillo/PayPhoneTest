namespace PeyphoneTest.Models.Responses
{
    public class ResponsesModels
    {
        public class CreateWalletResponse
        {
            public bool Success { get; set; }
            public int WalletId { get; set; }
        }

        public class TransferResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;

namespace PeyphoneTest.Models;

public partial class HistoryMovement
{
    public int Id { get; set; }

    public int WalletId { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}

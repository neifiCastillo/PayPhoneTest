using System;
using System.Collections.Generic;

namespace PeyphoneTest.Models;

public partial class Wallet
{
    public int Id { get; set; }

    public string DocumentId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<HistoryMovement> HistoryMovements { get; set; } = new List<HistoryMovement>();
}

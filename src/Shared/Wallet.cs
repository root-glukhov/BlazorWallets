using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace BlazorWallets.Shared;

public class Wallet
{
    public int Id { get; set; }
    public string Address { get; set; }

    [NotMapped]
    public decimal Balance { get; set; }
}

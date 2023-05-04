using BlazorWallets.Server.Data;
using BlazorWallets.Server.Services;
using BlazorWallets.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWallets.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly WalletService _walletService;

    public WalletController(AppDbContext context, WalletService walletService)
    {
        _context = context;
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<IEnumerable<Wallet>> Get()
    {
        var wallets = _context.Wallets.ToList();
        return await _walletService.GetWalletsAsync(wallets);
    }
}

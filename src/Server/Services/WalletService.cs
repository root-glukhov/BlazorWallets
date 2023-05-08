using BlazorWallets.Shared;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace BlazorWallets.Server.Services;

public class WalletService
{
    private readonly Web3 web3;

    public WalletService(IConfiguration configuration)
    {
        web3 = new Web3(configuration["RpcEndpoint"]);
    }

    public async Task<IEnumerable<Wallet>> GetWalletsAsync(List<Wallet> wallets)
    {
        int batchSize = 1000;
        int index = 0;
        var batchTasks = new List<Task>();

        for (int i = 0; i < wallets.Count; i += batchSize)
        {
            var batchWallets = wallets.Skip(i).Take(batchSize).ToList();

            var batchRequest = new RpcRequestResponseBatch();

            foreach (var wallet in batchWallets)
            {
                var batchItem = new RpcRequestResponseBatchItem<EthGetBalance, HexBigInteger>(
                    (EthGetBalance)web3.Eth.GetBalance,
                    web3.Eth.GetBalance.BuildRequest(wallet.Address, BlockParameter.CreateLatest(), index++));

                batchRequest.BatchItems.Add(batchItem);
            }

            batchTasks.Add(Task.Run(async () => {
                var response = await web3.Client.SendBatchRequestAsync(batchRequest);

                for (int j = 0; j < response.BatchItems.Count; j++)
                {
                    var result = response.BatchItems[j];
                    if (result.HasError && result.RpcError.Code == 429)
                    {
                        throw new TooManyRequestsException();
                    } 
                    else
                    {
                        var balance = Web3.Convert.FromWei((HexBigInteger)result.RawResponse);
                        batchWallets[j].Balance = balance;
                    }
                }
            }));
        }

        await Task.WhenAll(batchTasks);

        return wallets.OrderByDescending(x => x.Balance);
    }
}

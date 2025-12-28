using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIONonceExtension
{

    public static async Task CheckNonce<TViewModel, TDBContext>(this IIONonce<TViewModel, TDBContext> input, string? headerNonce)
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    where TViewModel : IIOViewModel<TDBContext>, new()
    {
        if (headerNonce == null)
        {
            throw new IOInvalidNonceException();
        }

        string? nonce = input.Session?.Get(IOSessionConstants.Nonce);
        if (nonce == null)
        {
            throw new IOInvalidNonceException();
        }

        string decryptedNonce = await input.ViewModel.DecryptString(headerNonce);
        if (!nonce.Equals(decryptedNonce))
        {
            throw new IOInvalidNonceException();
        }
    }

    public static string UpdateNonce<TViewModel, TDBContext>(this IIONonce<TViewModel, TDBContext> input)
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    where TViewModel : IIOViewModel<TDBContext>, new()
    {
        string nonce = IORandomUtilities.GenerateRandomAlphaNumericString(8);
        input.Session?.Set(IOSessionConstants.Nonce, nonce);

        return nonce;
    }
}

using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIONonceExtension
{

    public static void CheckNonce<TViewModel, TDBContext>(this IIONonce<TViewModel, TDBContext> input, ISession session, string? headerNonce)
    where TDBContext : IODatabaseContext<TDBContext>
    where TViewModel : IIOViewModel<TDBContext>, new()
    {
        if (headerNonce == null)
        {
            throw new IOInvalidNonceException();
        }
        
        if (!session.IsAvailable)
        {
            throw new IOInvalidNonceException();
        }

        string? nonce = session.GetString(IOSessionConstants.Nonce);
        if (nonce == null)
        {
            throw new IOInvalidNonceException();
        }

        string decryptedNonce = input.ViewModel.DecryptString(headerNonce);
        if (!nonce.Equals(decryptedNonce))
        {
            throw new IOInvalidNonceException();
        }
    }

    public static string UpdateNonce<TViewModel, TDBContext>(this IIONonce<TViewModel, TDBContext> input, ISession session)
    where TDBContext : IODatabaseContext<TDBContext>
    where TViewModel : IIOViewModel<TDBContext>, new()
    {
        string nonce = IORandomUtilities.GenerateRandomAlphaNumericString(8);
        if (session.IsAvailable)
        {
            session.SetString(IOSessionConstants.Nonce, nonce);
        }

        return nonce;
    }
}

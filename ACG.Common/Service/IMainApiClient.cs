using System;
using System.Threading.Tasks;

namespace ACG.Common.Service
{
    public interface IMainApiClient
    {
        Task<T> Get<T>(string endpoint);

        Task<T> Post<T>(string endpoint, Object payload);
    }
}

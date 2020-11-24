using System;
using System.Threading.Tasks;

namespace ADAPT.JohnDeere.core.Service
{
    public interface IJDApiClient
    {
        Task<T> Call<T>(string endpoint, string accessToken);
    }
}

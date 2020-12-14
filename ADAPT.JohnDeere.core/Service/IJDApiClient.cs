using System;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ADAPT.JohnDeere.core.Service
{
    public interface IJDApiClient
    {
        Task<T> Get<T>(string endpoint, string accessToken);

        Task Download(string endpoint, string fileName, string accessToken);
    }
}

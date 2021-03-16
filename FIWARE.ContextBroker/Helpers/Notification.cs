using System.Collections.Generic;
using FIWARE.ContextBroker.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FIWARE.ContextBroker.Helpers
{
  public class Notification<T> where T : class, new()
  {
    private IEnumerable<T> notificationData;
    public string SubscriptionId { get; set; }
    public JArray Data
    {
      set
      {
        var stringvalue = value.ToString();
        notificationData = JsonConvert.DeserializeObject<IEnumerable<T>>(stringvalue, new FiwareNormalizedJsonConverter<T>());
      }
    }
    public IEnumerable<T> DataTyped
    {
      get
      {
        return notificationData;
      }
    }
  }
}

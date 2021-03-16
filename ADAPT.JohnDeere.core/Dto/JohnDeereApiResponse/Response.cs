using System;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Response<T>
    {
        public Link[] Links { get; set; }
        public int Total { get; set; }

        public T[] Values { get; set; }
    }
}

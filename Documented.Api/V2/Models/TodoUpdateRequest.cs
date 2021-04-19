using Documented.Api.Data;

namespace Documented.Api.V2.Models
{
    public class TodoUpdateRequest: TodoCreateRequest
    {
        public Status CurrentStatus { get; set; }
    }
}

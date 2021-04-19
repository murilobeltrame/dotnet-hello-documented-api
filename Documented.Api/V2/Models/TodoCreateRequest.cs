using System;

namespace Documented.Api.V2.Models
{
    public class TodoCreateRequest
    {
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
    }
}

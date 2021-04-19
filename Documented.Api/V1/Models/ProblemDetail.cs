using System;

namespace Documented.Api.V1.Models
{
    public class ProblemDetail
    {
        /// <summary>
        /// The kind of the error
        /// </summary>
        /// <example>-77</example>
        public int ErrorCode { get; set; }

        /// <summary>
        /// The request trace id. It's used to ask for support when an issue happens.
        /// </summary>
        /// <example>45cbba15-976c-4387-9e64-35e7c97c052a</example>
        public Guid TraceId { get; set; }

        /// <summary>
        /// The friendly error message that can be shown on User Interface.
        /// </summary>
        /// <example>An error curred.</example>
        public string ErrorMessage { get; set; }
    }
}

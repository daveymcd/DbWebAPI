using System;

namespace DbWebAPI.Models
{
    /// <summary>
    /// DbWebApi.Models.ErrorViewModel
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>Request Id</summary>
        public string RequestId { get; set; }
        /// <summary>Show/NoShow</summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

using System;

namespace Vision.Models {

    public class RequestError {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

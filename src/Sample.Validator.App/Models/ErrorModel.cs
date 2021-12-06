using System.Collections.Generic;

namespace Sample.Validator.App.Models
{
    public class ErrorModel
    {
        public ErrorModel(string message, string code = null, string reference = null, IEnumerable<ErrorModel> innerErrors = null)
        {
            Message = message;
            Code = code;
            Reference = reference;
            InnerErrors = innerErrors;
        }
        public ErrorModel() : this(null, null, null, null) { }

        public string Message { get; set; }

        public string Code { get; set; }

        public string Reference { get; set; }

        public IEnumerable<ErrorModel> InnerErrors { get; set; }
    }
}

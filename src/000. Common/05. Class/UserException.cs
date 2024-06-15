using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _000.Common;
using _000.Common._01._Definition;

namespace _000.Common._05._Class
{
    public class UserException : Exception
    {
        public E_ERROR_CODE ErrorCode { get; }

        public UserException(string message, E_ERROR_CODE errorCode)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }

        public string UserMessage => $"Error : {Common.GetEnumName(ErrorCode)}\n\tMessage : {base.Message}";
        public string UserDetailMessage => $"Error : {Common.GetEnumInfo(ErrorCode)}\n\tMessage : {base.Message}";
    }
}

using System;

namespace SFA.DAS.Apprenticeships.Api.Types.Exceptions
{
    public class UnknownDeliveryModeException : Exception
        {
            public UnknownDeliveryModeException(string message)
            : base(message)
        {
        }
    }
}

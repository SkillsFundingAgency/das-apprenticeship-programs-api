﻿using System;

namespace SFA.DAS.Apprenticeships.Api.Types.Exceptions
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}

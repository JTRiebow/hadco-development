using System;

namespace Hadco.Common.DataTransferObjects
{
    public class BaseTimerDto
    {
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
    }
}

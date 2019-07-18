using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Core.Shared.Enums;

namespace IPA.Core.Client.Dal.Models
{
    public class TimerEventArgs : EventArgs
    {
        public TimerType Timer { get; set; }
    }
}

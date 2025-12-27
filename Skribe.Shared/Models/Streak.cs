using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skribe.Shared.Models
{
    public class Streak
    {
        public int Current { get; set; }
        public int Longest { get; set; }

        public DateTime? LastCompletedDate { get; set; }

        public void Update(DateTime nowUtc)
        {
            var today = nowUtc.Date;

            if (LastCompletedDate == null)
            {
                Current = 1;
                Longest = Math.Max(Longest, Current);
                LastCompletedDate = today;
                return;
            }

            var lastDate = LastCompletedDate.Value.Date;
            var daysDifference = (today - lastDate).Days;

            if (daysDifference == 0)
            {
                // Already counted today; nothing to do.
                return;
            }

            if (daysDifference == 1)
            {
                Current++;
            }
            else
            {
                Current = 1;
            }

            if (Current > Longest)
            {
                Longest = Current;
            }

            LastCompletedDate = today;
        }

        public void Reset()
        {
            Current = 0;
            LastCompletedDate = null;
        }
    }
}

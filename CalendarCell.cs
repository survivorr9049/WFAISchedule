using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAISchedule {
    public struct CalendarCell {
        public List<string> subjects;
        public int day;
        public CalendarCell(List<string> subjects, int day) {
            this.subjects = subjects;
            this.day = day;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAISchedule {
    public enum CellType {
        Lecture,
        Practice,
        Empty
    }
    public struct ScheduleCell {
        public Vector2 occupation;
        public string textData;
        public CellType type;
        public ScheduleCell(Vector2 occupation, string textData, CellType type) {
            this.occupation = occupation;
            this.textData = textData;
            this.type = type;
        }
    }
}

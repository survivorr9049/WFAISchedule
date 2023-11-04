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

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
        public bool always;
        public ScheduleCell(Vector2 occupation, string textData, CellType type, bool always) {
            this.occupation = occupation;
            this.textData = textData;
            this.type = type;
            this.always = always;
        }
    }
}

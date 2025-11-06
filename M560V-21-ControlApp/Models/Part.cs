namespace M560V_21_ControlApp.Models
{
    public class Part
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }

        public double StockWidth { get; set; }
        public double StockDepth { get; set; }
        public double StockHeight { get; set; }

        public double Op10PickXOffset { get; set; }
        public double Op10PickYOffset { get; set; }
        public double Op10PickZOffset { get; set; }

        public double Op20PickXOffset { get; set; }
        public double Op20PickYOffset { get; set; }
        public double Op20PickZOffset { get; set; }

        public double Op20FinXOffset { get; set; }
        public double Op20FinYOffset { get; set; }
        public double Op20FinZOffset { get; set; }

        public double Op10VisePSI { get; set; }
        public double Op20VisePSI { get; set; }

        public string Op10ProgramName { get; set; }
        public string Op20ProgramName { get; set; }

        public double Op10CycleTime { get; set; }
        public double Op20CycleTime { get; set; }
    }
}

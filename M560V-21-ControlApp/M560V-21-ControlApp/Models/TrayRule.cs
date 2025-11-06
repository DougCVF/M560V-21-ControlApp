namespace M560V_21_ControlApp.Models
{
    public class TrayRule
    {
        public int Id { get; set; }
        public string Name { get; set; } // Small, Medium, Large
        public double MinWidth { get; set; }
        public double MaxWidth { get; set; }
        public double Depth { get; set; }
        public double Height { get; set; }
        public int Slots { get; set; }
    }
}


// Models/Rack.cs
namespace M560V_21_ControlApp.Models
{
    public class Rack
    {
        public int RackId { get; set; }
        public string Name { get; set; }
    }

    public class RackRow
    {
        public int RackRowId { get; set; }
        public int RackId { get; set; }
        public int RowIndex { get; set; }
        public bool Enabled { get; set; }
    }

    public class RackAssignment
    {
        public int RackAssignmentId { get; set; }
        public int RackRowId { get; set; }
        public string PartNumber { get; set; }
        public int QuantityPlanned { get; set; }
        public double SpacingInches { get; set; }
        public string LastEditedUtc { get; set; }
    }
}

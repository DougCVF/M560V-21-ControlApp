// Models/ViseJaw.cs
namespace M560V_21_ControlApp.Models
{
    public class ViseJawLocation
    {
        public int JawLocationId { get; set; }
        public int RackId { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string Label { get; set; }
    }

    public class ViseJawAssignment
    {
        public int ViseJawAssignmentId { get; set; }
        public int JawLocationId { get; set; }
        public string PartNumber { get; set; }
        public string LastEditedUtc { get; set; }
    }
}

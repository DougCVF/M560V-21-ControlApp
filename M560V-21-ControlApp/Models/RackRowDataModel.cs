namespace M560V_21_ControlApp.Models
{
    public class RackRowDataModel
    {
        public int Id { get; set; }
        public int RackNumber { get; set; }
        public int RowNumber { get; set; }
        public int? AssignedPartId { get; set; }
        public int? TrayTypeId { get; set; }
        public bool Active { get; set; }
        public string SelectedPartNumber { get; set; }
        public int? SelectedPartId { get; set; }
    }
}

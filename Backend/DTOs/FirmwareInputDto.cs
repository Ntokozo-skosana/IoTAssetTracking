namespace Backend.DTOs
{
    public class FirmwareInputDto
    {
        public int FirmwareId { get; set; }
        public string Version { get; set; } = string.Empty;
        
        public string Status { get; set; } = "Active";
        public int DeviceTypeId { get; set; }
    }
}
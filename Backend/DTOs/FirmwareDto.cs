namespace Backend.DTOs
{
    public class FirmwareDto
    {
        public int FirmwareId {get;set;}
        public string Version {get;set;} = string.Empty;

        public string Status {get;set;} = string.Empty;
        public DateTime ReleasedAt {get;set;}
        
        public int DeviceTypeId {get;set;}
        public string DeviceTypeName {get;set;} = string.Empty;
    }
}
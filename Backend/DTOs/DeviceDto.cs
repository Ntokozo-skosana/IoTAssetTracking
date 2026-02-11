namespace Backend.DTOs
{
    public class DeviceDto
    {
        public int DeviceId {get;set;}
        public string SerialNumber {get;set;} = string.Empty;
        
        public string Status {get;set;} = string.Empty;
        public DateTime CreatedAt {get;set;}

        public int DeviceTypeId {get;set;}
        public string DeviceTypeName {get;set;} = string.Empty;

        public int FirmwareId {get;set;}
        public string FirmwareVersion {get;set;} = string.Empty;

        public int? GroupId {get;set;}
        public string? GroupName {get;set;}
    }
}
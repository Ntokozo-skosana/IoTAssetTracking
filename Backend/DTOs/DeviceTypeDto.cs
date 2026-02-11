namespace Backend.DTOs
{
    public class DeviceTypeDto
    {
        public int DeviceTypeId {get;set;}
        public string Name {get;set;} = string.Empty;
        
        public string? Description {get;set;}
        public bool IsActive {get;set;}
        public DateTime CreatedAt {get;set;}
    }
}
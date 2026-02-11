namespace Backend.DTOs
{
    public class DeviceGroupDto
    {
        public int GroupId {get;set;}
        public string Name {get;set;} = string.Empty;

        public DateTime CreatedAt {get;set;}
        public int? ParentGroupId {get;set;}
        
        public string? ParentGroupName {get;set;}
        public List<DeviceGroupDto> ChildGroups {get;set;} = new List<DeviceGroupDto>();
    }
}
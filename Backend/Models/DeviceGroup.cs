using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models{
    public class DeviceGroup
    {
        [Key]
        public int GroupId {get;set;}
        public string Name {get;set;} = string.Empty;

        public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
        public int? ParentGroupId {get;set;}

        [ForeignKey("ParentGroupId")]
        public DeviceGroup? ParentGroup {get;set;}

        public ICollection<DeviceGroup> ChildGroups {get;set;} = new List<DeviceGroup>();
        public ICollection<Device> Devices {get;set;} = new List<Device>();
    }
}
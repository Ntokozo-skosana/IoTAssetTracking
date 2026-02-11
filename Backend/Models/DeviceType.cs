using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models {
    public class DeviceType 
    {
        [Key]
        public int DeviceTypeId {get;set;}
        public string Name {get;set;} = string.Empty;
        
        public string? Description {get;set;}
        public bool IsActive {get;set;} = true;
        public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
        
        public ICollection<Device> Devices {get;set;} = new List<Device>();
        public ICollection<Firmware> Firmwares {get;set;} = new List<Firmware>();
    }
}
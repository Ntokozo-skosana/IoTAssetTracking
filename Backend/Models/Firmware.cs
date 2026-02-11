using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models{
     public class Firmware
     {
        [Key]
         public int FirmwareId {get;set;} 
         public string Version {get;set;} = string.Empty;
         public string Status {get;set;} = "Active";

         public DateTime ReleasedAt {get;set;} = DateTime.UtcNow; 
         public int DeviceTypeId {get;set;}
         
         [ForeignKey("DeviceTypeId")]
         public DeviceType DeviceType { get; set; } = null!;
         public ICollection<Device> Devices { get; set; } = new List<Device>();
     }   
}
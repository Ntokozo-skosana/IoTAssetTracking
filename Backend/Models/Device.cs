using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models {
    public class Device 
    {
        [Key]
        public int DeviceId {get;set;} 
        public string SerialNumber {get;set;} = string.Empty; 

        public string Status {get;set;} = "Active";
        public DateTime CreatedAt {get;set;}= DateTime.UtcNow;

        public int DeviceTypeId {get;set;}
        public int FirmwareId {get;set;}
        public int? GroupId {get;set;}

        [ForeignKey("DeviceTypeId")]
        public DeviceType DeviceType {get;set;} = null!;

        [ForeignKey("FirmwareId")]
        public Firmware Firmware {get;set;}=null!;

        [ForeignKey("GroupId")]
        public DeviceGroup? Group {get;set;}
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_POE.Models
{
    public class ClaimAttachment
    {
        [Key]
        public int ClaimAttachmentID { get; set; }

        [Required]
        [ForeignKey("Claim")]
        public int ClaimID { get; set; }

        [Required(ErrorMessage = "File name is required.")]
        [StringLength(255)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "File path is required.")]
        [StringLength(500)]
        [Display(Name = "File Path")]
        public string FilePath { get; set; }

        public virtual Claim Claim { get; set; }
    }
}

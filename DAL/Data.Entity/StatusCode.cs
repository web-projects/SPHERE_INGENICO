using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IPA.DAL.Data.Entity
{
    public class StatusCode
    {
        [Required(ErrorMessage="Code is Required.")]
    	[MaxLength(30)]
    	public string Code { get; set; }
        [Required(ErrorMessage="Active is Required.")]
    	public bool Active { get; set; }
        [Required(ErrorMessage="StatusCodeID is Required.")]
    	public int StatusCodeID { get; set; }
        [Required(ErrorMessage="DisplayMessage is Required.")]
    	[MaxLength(400)]
    	public string DisplayMessage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Status> Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StatusCode()
        {
            this.Status = new HashSet<Status>();
        }
    }
}

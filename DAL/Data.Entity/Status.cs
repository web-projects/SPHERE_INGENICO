using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPA.DAL.Data.Entity
{
    public class Status
    {
        [Required(ErrorMessage="StatusID is Required.")]
    	public long StatusID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Model.DB
{
    [Table(name: "USER")]
    public class mDuty
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public mDuty Parent { get; set; }

        //public int ShowIndex { get; set; }

    }
}

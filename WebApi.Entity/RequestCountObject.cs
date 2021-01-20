using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;


namespace WebApi.Entity
{
     [Table("RequestCountObject")]
   public class RequestCountObject
    {

         public string CreateYear { get; set; }
         public int SUMCount { get; set; }
         public int FailCount { get; set; }
         public int SuccessCount { get; set; }
    }
}

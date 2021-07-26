using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.DataProduct.RestService.Integration.Tests.TestFixtures
{
    /// <summary>
    /// GenericResponse
    /// </summary>
    public class GenericResponse
    {
   
        /// <summary>
        /// Response status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        ///  defined Status code.
        /// </summary>

        public string Code { get; set; }

        /// <summary>
        /// Status message.
        /// </summary>

        public string Message { get; set; }

    }
}

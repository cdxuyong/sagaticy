using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Data
{
    public enum UpdateBehavior
    {
        /// <summary>
        /// No interference with the DataAdapter's Update command. If Update encounters
        /// an error, the update stops.  Additional rows in the Datatable are uneffected.
        /// </summary>
        Standard,
        /// <summary>
        /// If the DataAdapter's Update command encounters an error, the update will
        /// continue. The Update command will try to update the remaining rows. 
        /// </summary>
        Continue,
        /// <summary>
        /// If the DataAdapter encounters an error, all updated rows will be rolled back.
        /// </summary>
        Transactional
    }
}

using System.Collections.Generic;

namespace BizUnit.TestSteps.Sql
{
    /// <summary>
    ///     Database row to be validated
    /// </summary>
    public class DbRowToValidate
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public DbRowToValidate()
        {
            Cells = new List<DbCellToValidate>();
        }

        /// <summary>
        ///     The cells to be validated
        /// </summary>
        public IList<DbCellToValidate> Cells { get; set; }
    }
}
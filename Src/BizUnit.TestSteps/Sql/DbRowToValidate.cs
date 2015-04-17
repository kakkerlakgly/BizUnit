
namespace BizUnit.TestSteps.Sql 
{
    using System.Collections.Generic;

    ///<summary>
    /// Database row to be validated
    ///</summary>
    public class DbRowToValidate
    {
        ///<summary>
        /// The cells to be validated
        ///</summary>
        public List<DbCellToValidate> Cells { get; set; }

        ///<summary>
        /// Default constructor
        ///</summary>
        public DbRowToValidate()
        {
            Cells = new List<DbCellToValidate>(); 
        }
    }
}

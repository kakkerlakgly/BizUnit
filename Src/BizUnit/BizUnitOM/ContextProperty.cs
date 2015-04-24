using System;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// </summary>
    [Obsolete("ContextProperty has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class ContextProperty
    {
        /// <summary>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public object GetPropertyValue(Context ctx)
        {
            if (null != ctx)
            {
                return ctx.GetObject(Name);
            }

            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _000.Common._03._Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class HistoryAttribute : System.Attribute
    {
        #region Handler

        #endregion

        #region Fields

        #endregion

        #region Properties

        public double Version { get; set; }
        public double Revision { get; set; }
        public string Changed { get; set; }
        private string Developer { get; set; }
        public string Description { get; set; }

        #endregion

        #region Ctor

        public HistoryAttribute(double version, double revision, string developer, string description)
        {
            Version = version;
            Revision = revision;
            Developer = developer;
            Description = description;
        }

        #endregion

        #region Event

        #endregion

        #region Accessor

        public override string ToString()
        {
            return $"[v{Version}.{Revision}] Changed : {Changed}, Dev : {Developer}, Desc : {Description}";
        }

        #endregion

        #region Mutator

        #endregion

        #region Predicate

        #endregion
    }
}

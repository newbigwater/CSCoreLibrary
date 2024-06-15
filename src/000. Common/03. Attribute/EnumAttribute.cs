using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _000.Common._01._Definition;

namespace _000.Common._03._Attribute
{
    public class EnumAttribute : System.Attribute
    {
        /* DTO (Data Transfer Object) */
        #region ▶  Event Handle             ◀



        #endregion // Event Handle

        #region ▶  Fields                   ◀

        private string                                              _name                = "";
        private Dictionary<E_LANGUAGE_TYPE, DescriptionAttribute>   _dicDesc             = new Dictionary<E_LANGUAGE_TYPE, DescriptionAttribute>();

        #endregion // Fields

        #region ▶  Properties               ◀

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string this[E_LANGUAGE_TYPE type] => _dicDesc[type]?.Description ?? "";

        #endregion // Properties

        #region ▶  Constructor              ◀

        private EnumAttribute(string name) { Name = name; }

        public EnumAttribute(string name, DescriptionAttribute attr) : this(name) { _dicDesc.Add(attr.LanguageType, attr); }

        public EnumAttribute(string name, string description) : this(name, new KoreanDescriptionAttribute(description)) { }

        public EnumAttribute(string name, params DescriptionAttribute[] descAttrList)
            : this(name)
        {
            foreach (var descAttr in descAttrList)
                _dicDesc.Add(descAttr.LanguageType, descAttr);
        }

        #endregion // Constructor
    }
}

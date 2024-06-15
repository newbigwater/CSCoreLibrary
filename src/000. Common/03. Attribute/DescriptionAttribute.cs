using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _000.Common._01._Definition;

namespace _000.Common._03._Attribute
{
    public class DescriptionAttribute : System.Attribute
    {
        #region Fields

        private E_LANGUAGE_TYPE _languageType;
        private string          _description;

        #endregion

        #region Properties

        public E_LANGUAGE_TYPE LanguageType
        {
            get => _languageType;
            set => _languageType = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        #endregion

        #region Constructor

        public DescriptionAttribute(string description, E_LANGUAGE_TYPE languageType = E_LANGUAGE_TYPE.Korean)
        {
            _description = description;
            _languageType = languageType;
        }

        #endregion

        #region Accessor

        public override string ToString() => Description;

        #endregion

        #region Mutator

        #endregion

        #region Predicate

        #endregion
    }

    public class KoreanDescriptionAttribute : DescriptionAttribute
    {
        /* DAO (Data Access Object) */
        #region ▶  Event Handle             ◀

        #endregion // Event Handle

        #region ▶  Constructor              ◀

        public KoreanDescriptionAttribute(string description) : base(description, E_LANGUAGE_TYPE.Korean) { }

        #endregion // Constructor

        #region ▶  Method                   ◀

        #region ▶  Method : Predicate	    ◀

        #endregion // Method : Predicate

        #region ▶  Method : Event Handler   ◀



        #endregion // Method : Event Handler

        #region ▶  Method : Function	    ◀



        #endregion // Method : Function

        #endregion // Method
    }

    public class EnglishDescriptionAttribute : DescriptionAttribute
    {
        /* DAO (Data Access Object) */
        #region ▶  Event Handle             ◀

        #endregion // Event Handle

        #region ▶  Constructor              ◀

        public EnglishDescriptionAttribute(string description) : base(description, E_LANGUAGE_TYPE.English) { }

        #endregion // Constructor

        #region ▶  Method                   ◀

        #region ▶  Method : Predicate	    ◀

        #endregion // Method : Predicate

        #region ▶  Method : Event Handler   ◀



        #endregion // Method : Event Handler

        #region ▶  Method : Function	    ◀



        #endregion // Method : Function

        #endregion // Method
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using _000.Common._01._Definition;
using _000.Common._03._Attribute;
using _000.Common._04._Struct;

namespace _000.Common
{
    public static partial class Common
    {
        #region EnumAttribute

        /// <summary>
        /// Enum 값에 설정된 Attribute 중 <see cref="EnumAttribute"/> 개체를 반환합니다.
        /// </summary>
        /// <param name="enumType"><see cref="EnumAttribute"/>가 설정된 <see cref="Enum"/>의 Type</param>
        /// <param name="enumValue"><see cref="Enum"/> 중 특정 Field</param>
        /// <returns>
        /// <paramref name="enumType" />의 Field 중 <paramref name="enumValue" /> 매개 변수와 매칭되는 Field에 설정된 <see cref="EnumAttribute"/> 개체
        /// </returns>
        public static EnumAttribute GetEnumAttribute(Type enumType, string enumValue)
        {
            var fieldInfo = enumType.GetField(enumValue);
            if (null == fieldInfo)
                return null;

            return ((EnumAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumAttribute), false))?[0];
        }

        /// <summary>
        /// <see cref="EnumAttribute"/>가 설정된 <see cref="Enum"/> Type의 <paramref name="value" /> 매개 변수의 <see cref="EnumAttribute"/> 개체를 반환합니다.
        /// </summary>
        /// <param name="value"><see cref="Enum"/> Type 값</param>
        /// <returns>
        /// <paramref name="value"/> 매개변수에 지정된 <see cref="EnumAttribute"/>[] 개체
        /// </returns>
        public static EnumAttribute[] GetEnumAttribute(Enum value)
        {
            EnumAttribute attr = GetEnumAttribute(value.GetType(), value.ToString());
            if (null != attr)
                return new EnumAttribute[] { attr };
            else
            {
                List<string> enumFlags = value.ToString().Split(',').ToList();
                EnumAttribute[] enumAttArr = new EnumAttribute[enumFlags.Count];
                for (int i = 0; i < enumFlags.Count; ++i)
                {
                    enumAttArr[i] = (GetEnumAttribute(value.GetType(), enumFlags[i].Trim()));
                }

                return enumAttArr;
            }
        }

        /// <summary>
        /// <see cref="EnumAttribute"/>가 설정된 <see cref="Enum"/> Type의 <paramref name="value" /> 매개 변수의 사용자가 정의한 Display Name을 반환합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator">다중 선택된 Enum 값일 경우 각 값의 Display Name 사이에 설정된 구분자를 추가합니다.</param>
        /// <returns>
        /// <paramref name="value"/> 매개변수에 지정된 Display Name
        /// </returns>
        public static string GetEnumName(Enum value, string separator = " | ")
        {
            var attrArr = GetEnumAttribute(value);
            if (1 == attrArr.Length)
                return attrArr[0].Name;
            else if (2 <= attrArr.Length)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var enumAttribute in attrArr)
                {
                    if (sb.Length == 0)
                        sb.Append(enumAttribute.Name);
                    else
                        sb.Append($"{separator}{enumAttribute.Name}");
                }

                return sb.ToString();
            }
            return value.ToString();
        }

        /// <summary>
        /// <see cref="EnumAttribute"/>가 설정된 <see cref="Enum"/> Type의 <paramref name="value" /> 매개 변수의 사용자가 정의한 Description을 반환합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator">다중 선택된 Enum 값일 경우 각 값의 Description 사이에 설정된 구분자를 추가합니다.</param>
        /// <param name="langType">출력할 설명의 언어를 선택합니다.</param>
        /// <returns>
        /// <paramref name="value"/> 매개변수에 지정된 Description
        /// </returns>
        public static string GetEnumDescription(Enum value, string separator = " | ", E_LANGUAGE_TYPE langType = E_LANGUAGE_TYPE.Korean)
        {
            var attrArr = GetEnumAttribute(value);
            if (1 == attrArr.Length)
                return attrArr[0][langType];
            else if (2 <= attrArr.Length)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var enumAttribute in attrArr)
                {
                    if (sb.Length == 0)
                        sb.Append(enumAttribute[langType]);
                    else
                        sb.Append($"{separator}{enumAttribute[langType]}");
                }

                return sb.ToString();
            }
            return value.ToString();
        }

        /// <summary>
        /// <see cref="EnumAttribute"/>가 설정된 <see cref="Enum"/> Type의 <paramref name="value" /> 매개 변수의 사용자가 정의한 Enum의 정보를 반환합니다.
        /// <br/>Format : [Display Name] Description
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator">다중 선택된 Enum 값일 경우 각 값의 Information 사이에 설정된 구분자를 추가합니다.</param>
        /// <param name="langType">출력할 설명의 언어를 선택합니다.</param>
        /// <returns>
        /// <paramref name="value"/> 매개변수에 지정된 Information
        /// </returns>
        public static string GetEnumInfo(Enum value, string separator = " | ", E_LANGUAGE_TYPE langType = E_LANGUAGE_TYPE.Korean)
        {
            var attrArr = GetEnumAttribute(value);
            if (1 == attrArr.Length)
                return $"[{attrArr[0].Name}] {attrArr[0][langType]}";
            else if (2 <= attrArr.Length)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var enumAttribute in attrArr)
                {
                    if (sb.Length == 0)
                        sb.Append($"[{enumAttribute.Name}] {enumAttribute[langType]}");
                    else
                        sb.Append($"{separator}[{enumAttribute.Name}] {enumAttribute[langType]}");
                }

                return sb.ToString();
            }
            return value.ToString();
        }

        #endregion //EnumAttribute

        #region File System

        /// <summary>
        /// 드라이브의 메모리 사용량을 <see cref="ExtDriveInfo"/> 개체로 반환합니다.
        /// </summary>
        /// <param name="path">드라이브의 경로
        /// <br/> Ex) D://Test -> D: Drive로 계산
        /// </param>
        /// <returns><paramref name="path"/> 매개 변수로 전달 받은 경로의 Drive의 정보를 담은 <see cref="DriveInfoApdeptor"/> 개체</returns>
        public static ExtDriveInfo GetDriveSize(string path)
        {
            ExtDriveInfo driveInfoApdeptor = new ExtDriveInfo()
            {
                DrvInfo = new DriveInfo(path)
            };
            return driveInfoApdeptor;
        }

        /// <summary>
        /// 전달 받은 디렉토리 경로의 사이즈를 구해 <see cref="long"/> 타입 값으로 반환합니다.
        /// </summary>
        /// <param name="path">Directory 경로</param>
        /// <returns><paramref name="path" /> 매개 변수로 전달된 Directory의 <see cref="byte"/> 사이즈</returns>
        public static long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path))
                return 0;

            long size = 0;
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (FileInfo fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                size += fi.Length;

            return size;
        }

        #endregion
    }
}

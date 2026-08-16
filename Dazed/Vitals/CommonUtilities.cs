using OpenTK.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Dazed.Vitals
{
    public static class CommonUtilities
    {
        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        public static string SetFirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static string ApplyIndentation(this string input, int indentationAmount, bool asLines = false)
        {
            string str = "";
            for (int i = 0; i < indentationAmount; i++)
                if (!asLines)
                    str += "    ";
                else
                    str += "----";
            return str + input;
        }
        public static bool IsOfTypeAndExists<T>(this object refObject)
        {
            if (refObject == null)
                return false;
            if (refObject.GetType() != typeof(T))
                return false;
            return true;
        }
        public static bool IsOfTypeAndExists<T>(this object refObject, out object outRefObject)
        {
            outRefObject = refObject;
            if (refObject == null)
                return false;
            if (refObject.GetType() != typeof(T))
                return false;
            return true;
        }

        public static bool IsOfTypeOrSubTypeAndExists<T>(this object refObject)
        {
            if (refObject == null)
                return false;
            if (refObject.GetType() != typeof(T))
                if (refObject.GetType().IsSubclassOf(typeof(T)))
                    return true;
                else
                    return false;
            return true;
        }

        public static float Normalize(this float value)
        {
            float result = value;
            if (value > 1)
                result = 1;
            if (value < -1)
                result = -1;
            return result;
        }

        public static Vector3 GetDirection(this Transform transform, ObjectDirection direction)
        {
            switch (direction)
            {
                case ObjectDirection.Right:
                    return (Vector3)transform.Right;
                case ObjectDirection.Left:
                    return (Vector3)(-transform.Right);
                case ObjectDirection.Up:
                    return (Vector3)transform.Up;
                case ObjectDirection.Down:
                    return (Vector3)(-transform.Up);
                case ObjectDirection.Forward:
                    return (Vector3)transform.Forward;
                case ObjectDirection.Backward:
                    return (Vector3)(-transform.Forward);
            }
            return default;
        }
        public static Vector3 GetOppositeDirection(this Transform transform, ObjectDirection direction)
        {
            switch (direction)
            {
                case ObjectDirection.Right:
                    return (Vector3)(-transform.Right);
                case ObjectDirection.Left:
                    return (Vector3)transform.Right;
                case ObjectDirection.Up:
                    return (Vector3)(-transform.Up);
                case ObjectDirection.Down:
                    return (Vector3)transform.Up;
                case ObjectDirection.Forward:
                    return (Vector3)(-transform.Forward);
                case ObjectDirection.Backward:
                    return (Vector3)transform.Forward;
            }
            return default;
        }

        public static float ToFloat(this string value)
        {
            char[] valueChars = value.ToCharArray();
            float whole = 0;
            float decimal_ = 0;
            float currentDecimal = 0;
            if (value.Contains('.') || value.Contains(','))
                for (int c = valueChars.Length - 1; c >= 0; c--)
                {
                    char cha = valueChars[c];
                    bool added = false;
                    if (cha != '.' && !added)
                    {
                        currentDecimal -= 1;
                        added = true;
                    }
                    if (cha != ',' && !added)
                    {
                        currentDecimal -= 1;
                        added = true;
                    }
                    if (cha == '.' || cha == ',')
                        break;
                }
            for (int i = valueChars.Length - 1; i >= 0; i--)
            {
                if (!char.IsNumber(valueChars[i]) && (valueChars[i] != '.' || valueChars[i] != ','))
                    continue;
                float num = (float)char.GetNumericValue(valueChars[i]);
                if (currentDecimal < 0)
                {
                    currentDecimal += 1;
                    decimal_ /= 10;
                    decimal_ += num;
                }
                if (currentDecimal >= 0)
                {
                    whole += num * MathF.Pow(10, currentDecimal);
                    currentDecimal += 1;
                }
            }
            whole += decimal_;
            return whole;
        }
        public static int ToInt(this string value)
        {
            char[] chars = value.ToCharArray();
            int finalNum = 0;
            bool isNegative = chars[0] == '-';
            for (int i = 0; i < chars.Length; i++)
                if (char.IsNumber(chars[i]))
                {
                    finalNum += (int)char.GetNumericValue(chars[i]);
                    if (i != chars.Length - 1)
                        finalNum *= 10;
                }
                else if (chars[i] == '.' || chars[i] == ',')
                    break;
            return finalNum * (isNegative ? -1 : 1);
        }
        public static Vector3 ToVector3(this string value)
        {
            char[] chars = value.ToLower().ToCharArray();
            Vector3 result = new();
            string tempNumString = "";
            uint currentAxisIndex = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == ',')
                    continue;
                if (chars[i] == 'z' || currentAxisIndex == 3)
                {
                    currentAxisIndex = 3;
                    if (chars[i] == 'z')
                    {
                        result.Y = tempNumString.ToFloat();
                        tempNumString = "";
                    }
                    tempNumString += chars[i];
                }
                if (chars[i] == 'y' || currentAxisIndex == 2)
                {
                    currentAxisIndex = 2;
                    if (chars[i] == 'y')
                    {
                        result.X = tempNumString.ToFloat();
                        tempNumString = "";
                    }
                    tempNumString += chars[i];
                }
                if (chars[i] == 'x' || currentAxisIndex == 1)
                {
                    currentAxisIndex = 1;
                    tempNumString += chars[i];
                }
            }
            result.Z = tempNumString.ToFloat();
            return result;
        }
        public static Vector3 FromStringToVector3(string value)
        {
            char[] chars = value.ToLower().ToCharArray();
            Vector3 result = new();
            string tempNumString = "";
            uint currentAxisIndex = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == ',')
                {
                    if (currentAxisIndex == 0)
                        result.X = tempNumString.ToFloat();
                    else if (currentAxisIndex == 1)
                        result.Y = tempNumString.ToFloat();
                    currentAxisIndex += 1;
                    tempNumString = "";
                }
                else if (chars[i] == ' ')
                    continue;
                else
                    tempNumString += chars[i];
            }
            result.Z = tempNumString.ToFloat();
            return result;
        }
        public static OpenTK.Mathematics.Vector4 ToVector4(this Color color) => new(color.R, color.G, color.B, color.A);
        public static OpenTK.Mathematics.Vector4 ToVector4(this Color4 color) => new(color.R, color.G, color.B, color.A);
    }
}

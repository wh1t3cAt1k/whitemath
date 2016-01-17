using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace whiteMath
{
    /// <summary>
    /// Struct representing a complex number as a pair of two numeric values.
    /// </summary>
    public partial struct Complex
    {
        private double real;
        private double img;

        /// <summary>
        /// Standard constructor of a complex number.
        /// </summary>
        /// <param name="real"></param>
        /// <param name="img"></param>
        public Complex(double real, double img)
        {
            this.real = real;
            this.img = img;
        }

        // ------------------------------------------------
        // ---------Real, imaginary parts, ----------------
        // ---------Module and adjoint numbers-------------
        // ------------------------------------------------

        public double RealCounterPart { get { return real; } internal set { real = value; } }
        public double ImaginaryCounterPart { get { return img; } internal set { img = value; } }

        /// <summary>
        /// Performs calculation of complex number module
        /// </summary>
        public double Module { get { return Math.Sqrt(real*real + img*img); } }

        /// <summary>
        /// Returns the adjoint complex number for the current number
        /// </summary>
        public Complex Adjoint { get { return new Complex(this.real, -this.img); } }

        /// <summary>
        /// Returns the angle in radians for the current complex number.
        /// </summary>
        public double Angle
        {
            get
            {
                // Если мнимая часть больше действительной, то
                // косинус меньше синуса, поэтому возвращать нужно арккосинус угла - будет больше точность.
                // -
                if (img > real)
                {
                    return Math.Acos(real / this.Module);
                }
                else
                {
                    return Math.Asin(img / this.Module);
                }
            }
        }

        // ------------------------------------------------
        // -----------Arithmetic operators-----------------
        // ------------------------------------------------

        public static Complex operator +(Complex one, Complex two)
        {
            return new Complex(one.real+two.real, one.img+two.img);
        }

        public static Complex operator -(Complex one, Complex two)
        {
            return new Complex(one.real-two.real, one.img-two.img);
        }

        public static Complex operator *(Complex one, Complex two)
        {
            return new Complex(one.real * two.real - one.img * two.img, one.img * two.real + one.real * two.img);
        }

        public static Complex operator /(Complex one, Complex two)
        {
            double denominator = two.real * two.real + two.img * two.img;
            
            return 
                new Complex(
                    (one.real * two.real + one.img * two.img) / denominator, 
                    (one.img * two.real - one.real * two.img) / denominator);
        }

        public static Complex operator -(Complex one)
        {
            return new Complex(-one.real, -one.img);
        }

        // ------------------------------------------------
        // -----------Comparison operators-----------------
        // ------------------------------------------------

        public static bool operator ==(Complex one, Complex two)
        {
            return one.real == two.real && one.img == two.img;
        }

        public static bool operator !=(Complex one, Complex two)
        {
            return !(one == two);
        }

        // --------------------- Equals, hashcode and toString()

        public override bool Equals(object obj)
        {
            if (obj is Complex)
            {
                return ((Complex)obj).img == this.img && ((Complex)obj).real == this.real;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return real.GetHashCode() + img.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0} {1} {2}i)", real, (img<0 ? "-" : "+"), img);
        }

        // --------------------- CONVERSION OPERATORS

        /// <summary>
        /// Implicit conversion of real to complex numbers
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Complex(double value)
        {
            return new Complex(value, 0);
        }

        // ---------------------------------------------------
        // ---------------------- STRING PARSE METHOD

        /// <summary>
        /// Parses the string containing the Complex number.
        /// Syntax:
        /// 
        /// 1. [re; im]
        /// 2. (re; im)
        /// 3. re; im
        /// 
        /// Numerator and denominator should be written in the format in accordance 
        /// to their own Parse methods.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Complex Parse(string value)
        {
            value = value.Replace(" ", ""); // убираем пробелы

            // Убираем внешние скобки
            // -
            if (value[0] == '[' && value[value.Length - 1] == ']'
                || value[0] == '(' && value[value.Length-1] ==')')
                    value = value.Substring(1, value.Length - 2);

            string[] split = value.Split(';');

            Complex tmp = new Complex(double.Parse(split[0]), double.Parse(split[1]));
            return tmp;
        }
    }
}

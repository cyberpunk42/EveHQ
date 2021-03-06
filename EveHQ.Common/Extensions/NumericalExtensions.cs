﻿// ==============================================================================
// 
// EveHQ - An Eve-Online™ character assistance application
// Copyright © 2005-2015  EveHQ Development Team
//   
// This file is part of EveHQ.
//  
// The source code for EveHQ is free and you may redistribute 
// it and/or modify it under the terms of the MIT License. 
// 
// Refer to the NOTICES file in the root folder of EVEHQ source
// project for details of 3rd party components that are covered
// under their own, separate licenses.
// 
// EveHQ is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the MIT 
// license below for details.
// 
// ------------------------------------------------------------------------------
// 
// The MIT License (MIT)
// 
// Copyright © 2005-2015  EveHQ Development Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ------------------------------------------------------------------------------
// 
// <copyright file="NumericalExtensions.cs" company="EveHQ Development Team">
//     Copyright © 2005-2015  EveHQ Development Team
// </copyright>
// 
// ==============================================================================

namespace EveHQ.Common.Extensions
{
    /// <summary>
    ///     The numerical extensions.
    /// </summary>
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    public static class NumericalExtensions
    {
        #region Public Methods and Operators

        /// <summary>The to invariant string.</summary>
        /// <param name="number">The number.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToInvariantString(this int number)
        {
            return number.ToInvariantString(null);
        }

        /// <summary>The to invariant string.</summary>
        /// <param name="number">The number.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToInvariantString(this int number, string formatter)
        {
            return number.ToString(formatter, CultureInfo.InvariantCulture);
        }

        /// <summary>The to invariant string.</summary>
        /// <param name="number">The number.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToInvariantString(this long number)
        {
            return number.ToInvariantString(null);
        }

        /// <summary>The to invariant string.</summary>
        /// <param name="number">The number.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToInvariantString(this long number, string formatter)
        {
            return number.ToString(formatter, CultureInfo.InvariantCulture);
        }

        /// <summary>The to invariant string.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="string" />.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "boolean values should be serialized to lower for use in xml or json.")]
        public static string ToInvariantString(this bool value)
        {
            return value.ToString().ToLower(CultureInfo.InvariantCulture);
        }

        /// <summary>The to invariant string.</summary>
        /// <param name="number">The number.</param>
        /// <param name="decimalPlaces">The decimal places.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToInvariantString(this double number, int decimalPlaces)
        {
            const string Formatter = "F{0}";

            return number.ToString(Formatter.FormatInvariant(decimalPlaces), CultureInfo.InvariantCulture);
        }

        /// <summary>The to invariant string.</summary>
        /// <param name="number">The number.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToInvariantString(this double number, string formatter)
        {
            return number.ToString(formatter, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
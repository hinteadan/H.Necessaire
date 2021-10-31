using System;

namespace Org.BouncyCastle.Math.Field
{
    internal interface IPolynomialExtensionField
        : IExtensionField
    {
        IPolynomial MinimalPolynomial { get; }
    }
}

using System;

namespace Org.BouncyCastle.Cms
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    internal class CmsAttributeTableGenerationException
		: CmsException
	{
		public CmsAttributeTableGenerationException()
		{
		}

		public CmsAttributeTableGenerationException(
			string name)
			: base(name)
		{
		}

		public CmsAttributeTableGenerationException(
			string		name,
			Exception	e)
			: base(name, e)
		{
		}
	}
}

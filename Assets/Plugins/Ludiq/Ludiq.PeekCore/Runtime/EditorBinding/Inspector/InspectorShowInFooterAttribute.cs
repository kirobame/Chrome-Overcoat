using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorShowInFooterAttribute : Attribute
	{
		public InspectorShowInFooterAttribute() { }
	}
}
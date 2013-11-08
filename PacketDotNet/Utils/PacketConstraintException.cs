using System;

namespace PacketDotNet
{
	public class PacketConstraintException : Exception
	{

        public PacketConstraintException (String typeName, int minimumLength, int actualLength) 
			: base(String.Format("The packet of type {0} must have at least {1} bytes, {2} bytes were provided.", typeName, minimumLength, actualLength))
		{
			TypeName = typeName;
			MinimumLength = minimumLength;
			ActualLength = actualLength;
		}

        public PacketConstraintException (String typeName, string reason)
			: base(String.Format("The packet of type {0} cannot be created: {1}.", typeName, reason))
		{
			TypeName = typeName;
			OtherReason = reason;
		}

		public string TypeName 
		{
			get;
			private set;
		}

		public int? MinimumLength
		{
			get;
			private set;
		}

		public int? ActualLength
		{
			get;
			private set;
		}

		public string OtherReason 
		{
			get;
			private set;
		}
	}
}


using System.Runtime.Serialization;

namespace TechShop.Helpers.Exceptions
{
    [Serializable]
    public class ProductInUseException : Exception
    {
        private static readonly string DefaultMessage = "This product cannot be deleted because it is being used in another record.";

        public ProductInUseException()
            : base(DefaultMessage)
        {
        }

        public ProductInUseException(string message)
            : base(message)
        {
        }

        protected ProductInUseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);
        }
    }
}

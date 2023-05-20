using System.Runtime.Serialization;

namespace CheckoutCart.Helpers.Exceptions
{
    [Serializable]
    public class OrderNotFoundException : Exception
    {
     
        public OrderNotFoundException(string message)
            : base(message)
        {
        }

        protected OrderNotFoundException(SerializationInfo info, StreamingContext context)
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

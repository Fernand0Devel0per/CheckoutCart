using System.Runtime.Serialization;

namespace CheckoutCart.Helpers.Exceptions
{
    [Serializable]
    public class InvalidOrderStatusException : Exception
    {
     
        public InvalidOrderStatusException(string message)
            : base(message)
        {
        }

        protected InvalidOrderStatusException(SerializationInfo info, StreamingContext context)
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

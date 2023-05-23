using System.Runtime.Serialization;

namespace TechShop.Helpers.Exceptions
{
    [Serializable]
    public class StatusNotFoundException : Exception
    {
     
        public StatusNotFoundException(string message)
            : base(message)
        {
        }

        protected StatusNotFoundException(SerializationInfo info, StreamingContext context)
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

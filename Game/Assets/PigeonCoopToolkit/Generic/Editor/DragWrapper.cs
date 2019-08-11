namespace PigeonCoopToolkit.Generic.Editor
{
    //Tangoing with one of Unity's many quirks.
    public class DragWrapper
    {
    
        private object value;

        public object Value
        {
            get { return value; }
        }

        public DragWrapper(object value)
        {
            this.value = value;
        }

    }
}
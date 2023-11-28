namespace ThGold.Event
{
    /// <summary>
    /// �¼��¼��ӿ�
    /// </summary>
    public class Event : IEvent
    {
        // GETTER / SETTER
        /// <summary>
        /// The _type_string.
        /// </summary>
        private string _type_string;
        string IEvent.type
        {
            get
            {
                return _type_string;
            }
            set
            {
                _type_string = value;

            }
        }

        /// <summary>
        /// The _target_object.
        /// </summary>
        private object _target_object;
        object IEvent.target
        {
            get
            {
                return _target_object;
            }
            set
            {
                _target_object = value;

            }
        }
        private object _data;
        public object data {
            get
            {
                return _data;
            }
            set
            {
                _data = value;

            }
        }

        ///<summary>
        ///	 Constructor ���ִ�
        ///</summary>
        public Event(string aType_str)
        {
            //
            _type_string = aType_str;
        }
        /// <summary>
        /// ���ִ� ��������
        /// </summary>
        /// <param name="aType_str"></param>
        /// <param name="obj"></param>
        public Event(string aType_str, object obj)
        {
            //
            _type_string = aType_str;
            _data = obj;
        }
        /// <summary>
        /// Deconstructor
        /// </summary>
        //~Event ( )
        //{
        //	Debug.Log ("Event.deconstructor()");
        //}
    }
}
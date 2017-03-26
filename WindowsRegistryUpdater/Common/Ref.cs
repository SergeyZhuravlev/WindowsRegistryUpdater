using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceWrapper
{
    public class Ref<T> where T : class
    {
        public static implicit operator T(Ref<T> w)
        {
            return w.Value;
        }

        public Ref(T t)
        {
            _t = t;
        }

        public Ref(Ref<T> t)
        {
            _t = t._t;
        }

        public Ref()
        {
            _t = default(T);
        }

        public T Value
        {
            get
            {
                return _t;
            }

            set
            {
                _t = value;
            }
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null && _t != null)
            {
                return false;
            }

            // If parameter cannot be cast to Ref or T return false.
            var p = obj as Ref<T>;
            var p2 = obj as T;
            if ((object)p == null && (object)p2 == null)
            {
                return false;
            }
                            
            if((object)p2 != null)
                return Value == p2;
            else
                return Value == p.Value;
        }

        public bool Equals(Ref<T> p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value);
        }

        public bool Equals(T p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p);
        }

        public override int GetHashCode()
        {
            return _t == null ? 0.GetHashCode() : _t.GetHashCode();
        }

        public static bool operator ==(Ref<T> a, Ref<T> b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
                return true;

            if ((object)a != null && (object)b == null)
                return (object)a.Value == null;
            if ((object)b != null && (object)a == null)
                return (object)b.Value == null;

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a.Value, b.Value))
                return true;

            // If one is null, but not both, return false.
            if (((object)a.Value == null) || ((object)b.Value == null))
                return false;

            // Return true if the fields match:
            return a.Value == b.Value;
        }

        public static bool operator !=(Ref<T> a, Ref<T> b)
        {
            return !(a == b);
        }

        public static bool operator ==(Ref<T> a, T b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
                return true;

            if ((object)a != null && (object)b == null)
                return (object)a.Value == null;

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a.Value, b))
                return true;

            // If one is null, but not both, return false.
            if (((object)a.Value == null) || ((object)b == null))
                return false;

            // Return true if the fields match:
            return a.Value == b;
        }

        public static bool operator !=(Ref<T> a, T b)
        {
            return !(a == b);
        }

        public static bool operator ==(T a, Ref<T> b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
                return true;

            if ((object)b != null && (object)a == null)
                return (object)b.Value == null;

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b.Value))
                return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b.Value == null))
                return false;

            // Return true if the fields match:
            return a == b.Value;
        }

        public static bool operator !=(T a, Ref<T> b)
        {
            return !(a == b);
        }


        public override string ToString()
        {
            return _t.ToString();
        }

        public void Assign(T value)
        {
            _t = value;
        }

        public void Assign(Ref<T> value)
        {
            _t = value.Value;
        }

        public void Reset()
        {
            _t = null;
        }

        private T _t;
    }
}

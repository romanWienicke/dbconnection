using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace ConneXion.Data
{

    public class EditableBindingList<T> : BindingList<T> where T : BaseEditable, new()
    {
        public EditableBindingList() { }

        public EditableBindingList(IList<T> list) : base(list) { }

        private ListSortDirection SortDirection { get; set; }
        private PropertyDescriptor SortProperty { get; set; }

        protected override object AddNewCore()
        {
            T item = new T();
            item.Id = -1;
            base.Items.Add(item);
            return item;
        }

        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        private int SortList(BaseEditable a, BaseEditable b)
        {
            if (a == b || SortProperty == null) return 0;

            object lhsValue = a == null ? null : SortProperty.GetValue(a);
            object rhsValue = b == null ? null : SortProperty.GetValue(b);

            int result = 0;
            if (lhsValue == null)
            {
                result = -1;
            }
            else if (rhsValue == null)
            {
                result = 1;
            }
            else
            {
                result = Comparer.Default.Compare(a, b);
            }
            if (SortDirection == ListSortDirection.Descending)
            {
                return result * -1;
            }
            return result;
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            SortProperty = prop;
            SortDirection = direction;

            ((List<T>)Items).Sort(SortList);
        }

        protected override void RemoveSortCore()
        {
            SortDirection = ListSortDirection.Ascending;
            SortProperty = null;
        }

        protected override bool IsSortedCore
        {
            get
            {
                for (int i = 0; i < Items.Count - 1; ++i)
                {
                    BaseEditable a = Items[i];
                    BaseEditable b = Items[i + 1];
                    PropertyDescriptor property = SortPropertyCore;
                    if (property != null && SortList(Items[i], Items[i + 1]) >= 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}


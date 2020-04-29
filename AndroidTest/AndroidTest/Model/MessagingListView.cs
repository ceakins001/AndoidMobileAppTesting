using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;


namespace AndroidTest.Model
{
    public class MessagingListView: ListView
    {
        public static readonly BindableProperty ItemsProperty = BindableProperty.Create("Items", typeof(IEnumerable<MessagingListItem>), typeof(MessagingListView), new List<MessagingListItem>());

        public IEnumerable<MessagingListItem> Items
        {
            get { return (IEnumerable<MessagingListItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;


        public void NotifyItemSelected(object item, int index)
        {
            if (ItemSelected != null)
            {
                ItemSelected(this, new SelectedItemChangedEventArgs(item, index));
            }
        }
    }
}

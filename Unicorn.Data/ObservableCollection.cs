using System;
using System.Collections.Generic;
using System.Text;

namespace Unicorn
{
	/// <summary>
	/// Stores information for GetDirty event.
	/// </summary>
	public class ListItemEventArgs <T> : EventArgs
	{
		public ListItemEventArgs( T item )
		{
			this.Item = item;
		}

		public readonly T Item;
	}

	//public delegate void ItemInsertedEventHandler<T>( object sender, ListItemEventArgs<T> e );
	//public delegate void ListItemEventHandler<T>( object sender, ListItemEventArgs<T> e );
	public delegate void ListItemEventHandler<T>( object sender, ListItemEventArgs<T> e );

    public interface INotifyCollectionChanged<T>
    {
        event ListItemEventHandler<T> ItemInserted;
        event ListItemEventHandler<T> BeforeItemRemove;
        event EventHandler AnItemRemoved;
    }

	[Serializable()]
	public class ObservableCollection<T>: System.Collections.Generic.List<T>, INotifyCollectionChanged<T>
	{
		public event ListItemEventHandler<T> ItemInserted;
		public event ListItemEventHandler<T> BeforeItemRemove;
		public event EventHandler AnItemRemoved;

		public new void Add( T item )
		{
			base.Add( item );
			if ( ItemInserted != null )
				ItemInserted( this, new ListItemEventArgs<T>( item ) );
		}
		public new void AddRange( IEnumerable<T> collection )
		{
			base.AddRange( collection );
			if ( ItemInserted != null )
				foreach ( T t in collection )
					ItemInserted( this, new ListItemEventArgs<T>( t ) );
		}

		public new virtual void Clear()
		{
			if ( BeforeItemRemove != null )
				foreach( T t in this )
					BeforeItemRemove( this, new ListItemEventArgs<T>( t ) );
			base.Clear();
			if ( AnItemRemoved != null )
				AnItemRemoved( this, new EventArgs() );
		}

		public new void Insert( int index, T item )
		{
			base.Insert( index, item );
			if ( ItemInserted != null )
				ItemInserted( this, new ListItemEventArgs<T>( item ) );
		}
		public new void InsertRange( int index, IEnumerable<T> collection )
		{
			base.InsertRange( index, collection );
			if ( ItemInserted != null )
				foreach( T t in collection )
					ItemInserted( this, new ListItemEventArgs<T>( t ) );
		}

		public new bool Remove( T item )
		{
			if ( BeforeItemRemove != null )
				BeforeItemRemove( this, new ListItemEventArgs<T>( item ) );
			bool b = base.Remove( item );
			if ( AnItemRemoved != null )
				AnItemRemoved( this, new EventArgs() );
            return b;
		}
		public new int RemoveAll( Predicate<T> match )
		{
			if ( BeforeItemRemove != null )
				foreach( T t in this )
					BeforeItemRemove( this, new ListItemEventArgs<T>( t ) );
			return base.RemoveAll( match );
			if ( AnItemRemoved != null )
				AnItemRemoved( this, new EventArgs() );
		}
		public new void RemoveAt( int index )
		{
			if ( BeforeItemRemove != null )
				BeforeItemRemove( this, new ListItemEventArgs<T>( this[index] ) );
			base.RemoveAt( index );
			if ( AnItemRemoved != null )
				AnItemRemoved( this, new EventArgs() );
		}
		public new void RemoveRange( int index, int count )
		{
			if ( BeforeItemRemove != null )
				for ( int i = index; i < index + count; i++ )
					BeforeItemRemove( this, new ListItemEventArgs<T>( this[i] ) );
			base.RemoveRange( index, count );
			if ( AnItemRemoved != null )
				AnItemRemoved( this, new EventArgs() );
		}
		public new T this[int index]
		{
			get { return base[index]; }
			set
			{
				if ( base[index] != null && BeforeItemRemove != null )
					BeforeItemRemove( this, new ListItemEventArgs<T>( base[index] ) );
				if ( value != null && ItemInserted != null )
					ItemInserted( this, new ListItemEventArgs<T>( value ) );
				base[index] = value;
			}
		}
		public T GetLast()
		{
			return base[Count - 1];
		}


    }
}

using MauiApp1.BackEnd.Shared;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;

namespace MauiApp1.Front.Components.Shared;

public partial class MultiSelect : ContentView
{
	// Backing store for checkboxes (keyed by the item's value)
	private readonly Dictionary<object, CheckBox> _checkboxes = new();
	private bool _suppressSelectedValuesChanged;
	private INotifyCollectionChanged? _selectedValuesNotifier;
	private INotifyCollectionChanged? _itemsSourceNotifier;

	public MultiSelect()
	{
		InitializeComponent();
		// Try to hook picker notifications and set its items whenever this control is constructed.
		try
		{
			HookPickerNotifications();
			ApplyPickerItemsSource();
		}
        catch
        {
            // best-effort only; ignore failures to avoid breaking construction
        }

        // Ensure the items stack is scrollable so action buttons aren't pushed off-screen
        // when the list of items grows too large. Wrap ItemsStack in a ScrollView
        // at runtime if the XAML hasn't already done so.
        try
		{
			if (ItemsStack != null && ItemsStack.Parent is not ScrollView)
			{
				var parent = ItemsStack.Parent;
				var scroll = new ScrollView { Content = ItemsStack, VerticalOptions = LayoutOptions.FillAndExpand };
				// Cap the scrollable area so action buttons remain visible when the list is long.
				const double maxHeight = 300.0; // adjust as needed
				try
				{
					// If the platform/framework exposes a MaximumHeightRequest, prefer that.
					var maxProp = scroll.GetType().GetProperty("MaximumHeightRequest");
					if (maxProp != null && maxProp.CanWrite)
					{
						maxProp.SetValue(scroll, maxHeight);
					}
					else
					{
						scroll.HeightRequest = maxHeight;
					}
				}
				catch
				{
					// best-effort only
					scroll.HeightRequest = maxHeight;
				}

				if (parent is Layout<View> layout)
				{
					// Preserve the position in the parent's children collection
					var idx = layout.Children.IndexOf(ItemsStack);
					if (idx >= 0)
					{
						layout.Children.RemoveAt(idx);
						layout.Children.Insert(idx, scroll);

						// Preserve Grid attached properties when applicable
						if (layout is Microsoft.Maui.Controls.Grid)
						{
							Microsoft.Maui.Controls.Grid.SetRow(scroll, Microsoft.Maui.Controls.Grid.GetRow(ItemsStack));
							Microsoft.Maui.Controls.Grid.SetColumn(scroll, Microsoft.Maui.Controls.Grid.GetColumn(ItemsStack));
							Microsoft.Maui.Controls.Grid.SetRowSpan(scroll, Microsoft.Maui.Controls.Grid.GetRowSpan(ItemsStack));
							Microsoft.Maui.Controls.Grid.SetColumnSpan(scroll, Microsoft.Maui.Controls.Grid.GetColumnSpan(ItemsStack));
						}
					}
				}
				else if (parent is ContentView cv)
				{
					cv.Content = scroll;
				}
			}
		}
		catch
		{
			// best-effort only; ignore failures to avoid breaking construction
		}
		
	}


	void ApplyPickerItemsSource()
	{
		if (MultiPicker == null)
			return;

		var prop = MultiPicker.GetType().GetProperty("ItemsSource");
		if (prop != null && prop.CanWrite)
		{
			prop.SetValue(MultiPicker, ItemsSource);
			return;
		}

		// Fallback: try to find a field backing
		var f = MultiPicker.GetType().GetField("itemsSource", BindingFlags.Instance | BindingFlags.NonPublic);
		if (f != null)
			f.SetValue(MultiPicker, ItemsSource);
	}

	void HookPickerNotifications()
	{
		if (MultiPicker == null)
			return;

		// If the control supports INotifyPropertyChanged, monitor it for selection-related property updates.
		if (MultiPicker is INotifyPropertyChanged inpc)
		{
			inpc.PropertyChanged += Picker_PropertyChanged;
			return;
		}

		// Try to attach to common selection events by name.
		var evt = MultiPicker.GetType().GetEvent("SelectionChanged") ?? MultiPicker.GetType().GetEvent("SelectedItemsChanged") ?? MultiPicker.GetType().GetEvent("SelectedValueChanged");
		if (evt != null)
		{
			try
			{
				var handler = new EventHandler((s, e) => SyncSelectedValuesFromPicker());
				evt.AddEventHandler(MultiPicker, handler);
			}
			catch
			{
				// ignore if delegate signature doesn't match
			}
		}
	}

	private void Picker_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (string.Equals(e.PropertyName, "SelectedItems", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(e.PropertyName, "SelectedItem", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(e.PropertyName, "SelectedValues", StringComparison.OrdinalIgnoreCase))
		{
			SyncSelectedValuesFromPicker();
		}
	}

	void SyncSelectedValuesFromPicker()
	{
		if (MultiPicker == null)
			return;

		object? selected = null;
		var p = MultiPicker.GetType().GetProperty("SelectedItems") ?? MultiPicker.GetType().GetProperty("SelectedValues") ?? MultiPicker.GetType().GetProperty("Selected");
		if (p != null)
			selected = p.GetValue(MultiPicker);
		else
		{
			var single = MultiPicker.GetType().GetProperty("SelectedItem");
			if (single != null)
				selected = single.GetValue(MultiPicker);
		}

		if (selected == null)
			return;

		var values = new List<object>();
		if (selected is IEnumerable enumerable && !(selected is string))
		{
			foreach (var it in enumerable)
			{
				if (it == null) continue;
				// Map to value using SelectedValuePath when available
				if (!string.IsNullOrWhiteSpace(SelectedValuePath))
				{
					var vp = it.GetType().GetProperty(SelectedValuePath);
					if (vp != null)
						values.Add(vp.GetValue(it) ?? it);
					else
						values.Add(it);
				}
				else
				{
					values.Add(it);
				}
			}
		}
		else
		{
			// single value
			var it = selected;
			if (!string.IsNullOrWhiteSpace(SelectedValuePath) && it != null)
			{
				var vp = it.GetType().GetProperty(SelectedValuePath);
				if (vp != null)
					values.Add(vp.GetValue(it) ?? it);
				else
					values.Add(it);
			}
			else if (it != null)
			{
				values.Add(it);
			}
		}

		// Update SelectedValues without re-entrancy
		try
		{
			_suppressSelectedValuesChanged = true;
			if (SelectedValues is IList list)
			{
				list.Clear();
				foreach (var v in values)
					list.Add(v);
			}
			else
			{
				SetValue(SelectedValuesProperty, values.ToList());
			}
		}
		finally
		{
			_suppressSelectedValuesChanged = false;
		}
	}

	public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
		nameof(ItemsSource),
		typeof(System.Collections.IEnumerable),
		typeof(MultiSelect),
		default(System.Collections.IEnumerable),
		propertyChanged: OnItemsSourceChanged);

	public System.Collections.IEnumerable ItemsSource
	{
		get => (System.Collections.IEnumerable?)GetValue(ItemsSourceProperty) ?? Array.Empty<object>();
		set => SetValue(ItemsSourceProperty, value);
	}

	public static readonly BindableProperty SelectedValuesProperty = BindableProperty.Create(
		nameof(SelectedValues),
		typeof(System.Collections.IList),
		typeof(MultiSelect),
		default(System.Collections.IList),
		BindingMode.TwoWay,
		propertyChanged: OnSelectedValuesChanged);

	public System.Collections.IList SelectedValues
	{
		get => (System.Collections.IList?)GetValue(SelectedValuesProperty) ?? new System.Collections.ArrayList();
		set => SetValue(SelectedValuesProperty, value);
	}

	public static readonly BindableProperty EditableProperty = BindableProperty.Create(
		nameof(Editable),
		typeof(bool),
		typeof(MultiSelect),
		true,
		propertyChanged: OnEditableChanged);

	public bool Editable
	{
		get => (bool)GetValue(EditableProperty);
		set => SetValue(EditableProperty, value);
	}

	public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
		nameof(Header),
		typeof(string),
		typeof(MultiSelect),
		string.Empty,
		propertyChanged: OnHeaderChanged);

	public string Header
	{
		get => (string)GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	private static void OnItemsSourceChanged(BindableObject bindable, object oldVal, object newVal)
	{
		if (bindable is MultiSelect ctrl)
		{
			// unsubscribe previous collection change notifications
			if (oldVal is INotifyCollectionChanged oldColl)
				oldColl.CollectionChanged -= ctrl.ItemsSource_CollectionChanged;

			// subscribe to new collection change notifications when possible
			if (newVal is INotifyCollectionChanged newColl)
				newColl.CollectionChanged += ctrl.ItemsSource_CollectionChanged;

			ctrl._itemsSourceNotifier = newVal as INotifyCollectionChanged;

			// Ensure the inner picker's ItemsSource is set to the new collection
			// before we attempt to add items to it in RebuildList.
			ctrl.ApplyPickerItemsSource();

			// rebuild UI and propagate to picker
			ctrl.RebuildList();
		}
	}

	private void ItemsSource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		// Rebuild list on the UI thread when the underlying collection changes (add/remove/reset, etc.)
		try
		{
			// Use Dispatcher if available to ensure UI thread access
			if (Dispatcher?.IsDispatchRequired ?? false)
			{
				Dispatcher.Dispatch(() => RebuildList());
			}
			else
			{
				RebuildList();
			}
		}
		catch
		{
			// Best-effort: if dispatch fails, still attempt to rebuild
			RebuildList();
		}
	}

	public static readonly BindableProperty DisplayMemberPathProperty = BindableProperty.Create(
		nameof(DisplayMemberPath),
		typeof(string),
		typeof(MultiSelect),
		default(string),
		propertyChanged: OnDisplayMemberPathChanged);

	public string DisplayMemberPath
	{
		get => (string)GetValue(DisplayMemberPathProperty);
		set => SetValue(DisplayMemberPathProperty, value);
	}

	private static void OnDisplayMemberPathChanged(BindableObject bindable, object oldVal, object newVal)
	{
		if (bindable is MultiSelect ctrl)
			ctrl.RebuildList();
	}

	public static readonly BindableProperty SelectedValuePathProperty = BindableProperty.Create(
		nameof(SelectedValuePath),
		typeof(string),
		typeof(MultiSelect),
		default(string),
		propertyChanged: OnSelectedValuePathChanged);

	public string SelectedValuePath
	{
		get => (string)GetValue(SelectedValuePathProperty);
		set => SetValue(SelectedValuePathProperty, value);
	}

	private static void OnSelectedValuePathChanged(BindableObject bindable, object oldVal, object newVal)
	{
		if (bindable is MultiSelect ctrl)
			ctrl.RebuildList();
	}

	private static void OnSelectedValuesChanged(BindableObject bindable, object oldVal, object newVal)
	{
		if (bindable is MultiSelect ctrl)
		{
			if (ctrl._suppressSelectedValuesChanged)
				return;

			// unsubscribe old collection change handler
			if (oldVal is INotifyCollectionChanged oldColl)
				oldColl.CollectionChanged -= ctrl.SelectedValues_CollectionChanged;

			// subscribe new collection change handler if supported
			if (newVal is INotifyCollectionChanged newColl)
				newColl.CollectionChanged += ctrl.SelectedValues_CollectionChanged;

			ctrl._selectedValuesNotifier = newVal as INotifyCollectionChanged;

			ctrl.UpdateCheckboxesFromSelectedValues();
		}
	}

	private void SelectedValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		// When collection contents change, reflect them in the UI
		if (_suppressSelectedValuesChanged)
			return;

		UpdateCheckboxesFromSelectedValues();
	}

	private static void OnEditableChanged(BindableObject bindable, object oldVal, object newVal)
	{
		if (bindable is MultiSelect ctrl)
		{
			var editable = (bool)newVal;
			foreach (var cb in ctrl._checkboxes.Values)
				cb.IsEnabled = editable;
		}
	}

	private static void OnHeaderChanged(BindableObject bindable, object oldVal, object newVal)
	{
		if (bindable is MultiSelect ctrl)
		{
			ctrl.HeaderLabel.Text = newVal as string ?? string.Empty;
			ctrl.HeaderLabel.IsVisible = !string.IsNullOrWhiteSpace(ctrl.HeaderLabel.Text);
		}
	}

	private void RebuildList()
	{
		_checkboxes.Clear();
		// Ensure ItemsStack (the popup content) is present and cleared so the picker popup shows items and action buttons
		if (ItemsStack != null)
			ItemsStack.Children.Clear();
        var items = ItemsSource;
        

        if (items == null)
			return;
		

        foreach (var obj in items)
		{
			// determine display and value via TextValuePair or reflection
			string display;
			object? value;

			if (obj is TextValuePair<string, int> tvp)
			{
				display = tvp.Text?.ToString() ?? string.Empty;
				value = tvp.Value;
			}
			else
			{
				// use DisplayMemberPath / SelectedValuePath when available
				if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
				{
					var dp = obj?.GetType().GetProperty(DisplayMemberPath);
					var dv = dp?.GetValue(obj);
					display = dv?.ToString() ?? string.Empty;
				}
				else
				{
					display = obj?.ToString() ?? string.Empty;
				}

				if (!string.IsNullOrWhiteSpace(SelectedValuePath))
				{
					var vp = obj?.GetType().GetProperty(SelectedValuePath);
					value = vp?.GetValue(obj) ?? obj;
				}
				else
				{
					value = obj;
				}
			}

			var row = new HorizontalStackLayout { Spacing = 8 };

			var cb = new CheckBox
			{
				IsChecked = SelectedValues?.Contains(value) == true,
				IsEnabled = Editable,
				VerticalOptions = LayoutOptions.Center
			};

			_checkboxes[value ?? obj ?? new object()] = cb;

			cb.CheckedChanged += (s, e) => OnCheckBoxToggled(value ?? obj ?? new object(), e.Value);

			var lbl = new Label
			{
				Text = display,
				VerticalTextAlignment = TextAlignment.Center
			};

			row.Add(cb);
			row.Add(lbl);
			// Do not add UI rows to the inner picker's ItemsSource. ApplyPickerItemsSource
			// assigns the data ItemsSource (which may be a strongly-typed collection),
			// so adding a HorizontalStackLayout here can cause an ArgumentException.
			// MultiPicker.ItemsSource.Add(row);
			if (ItemsStack != null)
			{
				ItemsStack.Children.Add(row);
			}
        }
	}

	private void OnCheckBoxToggled(object value, bool isChecked)
	{
		try
		{
			_suppressSelectedValuesChanged = true;

			// If SelectedValues supports collection change notifications (e.g., ObservableCollection<int>),
			// modify it in-place so bindings receive CollectionChanged events.
			if (SelectedValues is INotifyCollectionChanged && SelectedValues is IList mutableList)
			{
				if (isChecked)
				{
					if (!mutableList.Contains(value))
						mutableList.Add(value);
				}
				else
				{
					if (mutableList.Contains(value))
						mutableList.Remove(value);
				}
			}
			else
			{
				// Fallback: create a new list instance and set the bindable property so property change fires.
				var list = new List<object>();
				if (SelectedValues != null)
				{
					foreach (var it in SelectedValues)
						list.Add(it);
				}

				if (isChecked)
				{
					if (!list.Contains(value))
						list.Add(value);
				}
				else
				{
					if (list.Contains(value))
						list.Remove(value);
				}

				// assign a copy to trigger bindings
				SetValue(SelectedValuesProperty, list.ToList());
			}
		}
		finally
		{
			_suppressSelectedValuesChanged = false;
		}
	}

	private void UpdateCheckboxesFromSelectedValues()
	{
		if (SelectedValues == null)
			return;

		foreach (var kvp in _checkboxes)
		{
			var val = kvp.Key;
			var cb = kvp.Value;
			var should = SelectedValues.Contains(val);
			if (cb.IsChecked != should)
				cb.IsChecked = should;
		}
	}

	public IList<object> GetSelectedValues()
	{
		var result = new List<object>();
		if (SelectedValues == null) return result;
		foreach (var it in SelectedValues)
			result.Add(it);
		return result;
	}
}

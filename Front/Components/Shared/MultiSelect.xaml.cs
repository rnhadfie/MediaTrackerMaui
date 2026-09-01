using MauiApp1.BackEnd.Shared;
using System.Collections.Specialized;

namespace MauiApp1.Front.Components.Shared;

public partial class MultiSelect : ContentView
{
	// Backing store for checkboxes
	private readonly Dictionary<int, CheckBox> _checkboxes = new();
	private bool _suppressSelectedValuesChanged;
	private INotifyCollectionChanged? _selectedValuesNotifier;

	public MultiSelect()
	{
		InitializeComponent();
	}

	public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
		nameof(ItemsSource),
		typeof(IList<TextValuePair<string, int>>),
		typeof(MultiSelect),
		default(IList<TextValuePair<string, int>>),
		propertyChanged: OnItemsSourceChanged);

	public IList<TextValuePair<string, int>> ItemsSource
	{
		get => (IList<TextValuePair<string, int>>)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public static readonly BindableProperty SelectedValuesProperty = BindableProperty.Create(
		nameof(SelectedValues),
		typeof(IList<int>),
		typeof(MultiSelect),
		default(IList<int>),
		BindingMode.TwoWay,
		propertyChanged: OnSelectedValuesChanged);

	public IList<int> SelectedValues
	{
		get => (IList<int>)GetValue(SelectedValuesProperty);
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
		ItemsStack.Children.Clear();

		var items = ItemsSource;
		if (items == null)
			return;

		foreach (var pair in items)
		{
			var row = new HorizontalStackLayout { Spacing = 8 };

			var cb = new CheckBox
			{
				IsChecked = SelectedValues?.Contains(pair.Value) == true,
				IsEnabled = Editable,
				VerticalOptions = LayoutOptions.Center
			};

			_checkboxes[pair.Value] = cb;

			cb.CheckedChanged += (s, e) => OnCheckBoxToggled(pair.Value, e.Value);

			var lbl = new Label
			{
				Text = pair.Text?.ToString() ?? string.Empty,
				VerticalTextAlignment = TextAlignment.Center
			};

			row.Add(cb);
			row.Add(lbl);

			ItemsStack.Children.Add(row);
		}
	}

	private void OnCheckBoxToggled(int value, bool isChecked)
	{
		try
		{
			_suppressSelectedValuesChanged = true;

			// If SelectedValues supports collection change notifications (e.g., ObservableCollection<int>),
			// modify it in-place so bindings receive CollectionChanged events.
			if (SelectedValues is INotifyCollectionChanged && SelectedValues is IList<int> mutableList)
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
				var list = SelectedValues?.ToList() ?? new List<int>();
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

	public IList<int> GetSelectedValues()
	{
		return SelectedValues?.ToList() ?? new List<int>();
	}
}

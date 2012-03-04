using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace uknit.Controls
{
	public partial class RowCounterControl : UserControl
	{
		public static readonly DependencyProperty TensDigitProperty = DependencyProperty.Register(
			"TensDigit",
			typeof(string),
			typeof(RowCounterControl),
			new PropertyMetadata(OnTensDigitChanged));

		public string TensDigit
		{
			get
			{
				return (string)GetValue(TensDigitProperty);
			}
			set
			{
				SetValue(TensDigitProperty, value);
			}
		}

		static void OnTensDigitChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as RowCounterControl).Tens.Text = args.NewValue as string;
		}

		public static readonly DependencyProperty OnesDigitProperty = DependencyProperty.Register(
			"OnesDigit",
			typeof(string),
			typeof(RowCounterControl),
			new PropertyMetadata(OnOnesDigitChanged));

		public string OnesDigit
		{
			get
			{
				return (string)GetValue(OnesDigitProperty);
			}
			set
			{
				SetValue(OnesDigitProperty, value);
			}
		}

		static void OnOnesDigitChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as RowCounterControl).Ones.Text = args.NewValue as string;
		}

		public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
			"Fill",
			typeof(Color),
			typeof(RowCounterControl),
			new PropertyMetadata(OnCounterColorChanged));

		public Color Fill
		{
			get
			{
				return (Color)GetValue(FillProperty);
			}

			set
			{
				SetValue(FillProperty, value);
			}
		}

		static void OnCounterColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as RowCounterControl).counterFill1.Color = (Color)args.NewValue;
			(obj as RowCounterControl).counterFill2.Color = (Color)args.NewValue;
		}

		public static readonly DependencyProperty NeedleWidthProperty = DependencyProperty.Register(
			"NeedleWidth",
			typeof(double),
			typeof(RowCounterControl),
			new PropertyMetadata(OnNeedleWidthChanged));

		public double NeedleWidth
		{
			get
			{
				return (double)GetValue(NeedleWidthProperty);
			}

			set
			{
				SetValue(NeedleWidthProperty, value);
			}
		}

		static void OnNeedleWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			RowCounterControl control = obj as RowCounterControl;
			double needleRenderWidth = ((double)args.NewValue - control.RowCounterColor.RenderSize.Width) / 2;
			control.RowCounterNeedleLeft.Width = needleRenderWidth;
			control.RowCounterNeedleRight.Width = needleRenderWidth;
		}

		public RowCounterControl()
		{
			// Required to initialize variables
			InitializeComponent();
		}
	}
}

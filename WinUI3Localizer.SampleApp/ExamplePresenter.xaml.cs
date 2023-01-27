using ColorCode;
using ColorCode.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace WinUI3Localizer.SampleApp;

[ContentProperty(Name = nameof(Example))]
public sealed partial class ExamplePresenter : UserControl
{
    public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
        nameof(Theme),
        typeof(ElementTheme),
        typeof(ExamplePresenter),
        new PropertyMetadata(ElementTheme.Default, (d, e) => (d as ExamplePresenter)?.UpdateSampleCode()));

    public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
        nameof(HeaderText),
        typeof(string),
        typeof(ExamplePresenter),
        new PropertyMetadata(default));

    public static readonly DependencyProperty HeaderTextStyleProperty = DependencyProperty.Register(
        nameof(HeaderTextStyle),
        typeof(Style),
        typeof(ExamplePresenter),
        new PropertyMetadata(default));

    public static readonly DependencyProperty ExampleProperty = DependencyProperty.Register(
        nameof(Example),
        typeof(object),
        typeof(ExamplePresenter),
        new PropertyMetadata(default));

    public static readonly DependencyProperty XamlSampleCodeProperty = DependencyProperty.Register(
        nameof(XamlSampleCode),
        typeof(string),
        typeof(ExamplePresenter),
        new PropertyMetadata(default, (d, e) => (d as ExamplePresenter)?.UpdateSampleCode()));

    public static readonly DependencyProperty CSharpSampleCodeProperty = DependencyProperty.Register(
        nameof(CSharpSampleCode),
        typeof(string),
        typeof(ExamplePresenter),
        new PropertyMetadata(default, (d, e) => (d as ExamplePresenter)?.UpdateSampleCode()));

    public static readonly DependencyProperty ExampleLabelProperty = DependencyProperty.Register(
        nameof(ExampleLabel),
        typeof(string),
        typeof(ExamplePresenter),
        new PropertyMetadata("Example"));

    public static readonly DependencyProperty CodeLabelProperty = DependencyProperty.Register(
        nameof(CodeLabel),
        typeof(string),
        typeof(ExamplePresenter),
        new PropertyMetadata("Code"));

    public static readonly DependencyProperty ExamplePresenterWidthProperty = DependencyProperty.Register(
        nameof(ExamplePresenterWidth),
        typeof(GridLength),
        typeof(ExamplePresenter),
        new PropertyMetadata(default));

    public ExamplePresenter()
    {
        InitializeComponent();
    }

    public ElementTheme Theme
    {
        get => (ElementTheme)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public Style HeaderTextStyle
    {
        get => (Style)GetValue(HeaderTextStyleProperty);
        set => SetValue(HeaderTextStyleProperty, value);
    }

    public object Example
    {
        get => (object)GetValue(ExampleProperty);
        set => SetValue(ExampleProperty, value);
    }

    public string XamlSampleCode
    {
        get => (string)GetValue(XamlSampleCodeProperty);
        set => SetValue(XamlSampleCodeProperty, value);
    }

    public string CSharpSampleCode
    {
        get => (string)GetValue(CSharpSampleCodeProperty);
        set => SetValue(CSharpSampleCodeProperty, value);
    }

    public string ExampleLabel
    {
        get => (string)GetValue(ExampleLabelProperty);
        set => SetValue(ExampleLabelProperty, value);
    }

    public string CodeLabel
    {
        get => (string)GetValue(CodeLabelProperty);
        set => SetValue(CodeLabelProperty, value);
    }

    public GridLength ExamplePresenterWidth
    {
        get => (GridLength)GetValue(ExamplePresenterWidthProperty);
        set => SetValue(ExamplePresenterWidthProperty, value);
    }

    private static void UpdateFormatterDarkThemeColors(RichTextBlockFormatter formatter)
    {
        _ = formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttribute]);
        _ = formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeQuotes]);
        _ = formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeValue]);
        _ = formatter.Styles.Remove(formatter.Styles[ScopeName.HtmlComment]);
        _ = formatter.Styles.Remove(formatter.Styles[ScopeName.XmlDelimiter]);
        _ = formatter.Styles.Remove(formatter.Styles[ScopeName.XmlName]);
        _ = formatter.Styles.Remove(formatter.Styles[ScopeName.Keyword]);

        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttribute)
        {
            Foreground = "#FF87CEFA",
            ReferenceName = "xmlAttribute"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeQuotes)
        {
            Foreground = "#FFFFA07A",
            ReferenceName = "xmlAttributeQuotes"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeValue)
        {
            Foreground = "#FFFFA07A",
            ReferenceName = "xmlAttributeValue"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.HtmlComment)
        {
            Foreground = "#FF6B8E23",
            ReferenceName = "htmlComment"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlDelimiter)
        {
            Foreground = "#FF808080",
            ReferenceName = "xmlDelimiter"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlName)
        {
            Foreground = "#FF5F82E8",
            ReferenceName = "xmlName"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.Keyword)
        {
            Foreground = "#FF5F82E8",
            ReferenceName = "Keyword"
        });
    }

    private void UpdateSampleCode()
    {
        RichTextBlockFormatter formatter = new(Theme);
        UpdateFormatterDarkThemeColors(formatter);

        this.XamlSampleCodeRichTextBlock.Blocks.Clear();
        formatter.FormatRichTextBlock(
            XamlSampleCode,
            Languages.Xml,
            this.XamlSampleCodeRichTextBlock);
        this.XamlSampleCodeBlock.Visibility = string.IsNullOrEmpty(XamlSampleCode) is false
            ? Visibility.Visible
            : Visibility.Collapsed;

        this.CSharpSampleCodeRichTextBlock.Blocks.Clear();
        formatter.FormatRichTextBlock(
            CSharpSampleCode,
            Languages.CSharp,
            this.CSharpSampleCodeRichTextBlock);
        this.CSharpSampleCodeBlock.Visibility = string.IsNullOrEmpty(CSharpSampleCode) is false
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
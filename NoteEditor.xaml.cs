using Newtonsoft.Json.Linq;
using STTNote.ActionMessages;
using STTNote.Enums;
using STTNote.Extensions;
using STTNote.Helpers;
using STTNote.Models;
using STTNote.Views;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace STTNote
{
    /// <summary>
    /// Interaction logic for NoteEditor.xaml
    /// </summary>
    public partial class NoteEditor : BaseWindow
    {
        public Note OriginalNote { set; get; }
        public EditingMode Mode { set; get; }
        private Note _processingNote { set; get; }

        public int DefaultFontSize { set; get; } = 14;

        public static double TextBoxScale { set; get; } = 1.0;

        public NoteEditor()
        {
            InitializeComponent();
            BoxPassword.DataContext = this;
            _processingNote = OriginalNote;
            txtContent.Document.LineHeight = 5;
            txtContent.FontSize = DefaultFontSize;
            txtContent.Focus();

            this.Width = 1000;
            this.Height = 700;

            InitControlSizes();
        }

        private void InitControlSizes()
        {
            var thisWidth = this.Width > 0 ? this.Width : 1000;
            var thisHeight = this.Height > 0 ? this.Height : 700;
            var bodyTitleSpace = 5;
            var bottomSpace = 5;

            var GroundWidth = thisWidth - 10;
            var GroundHeight = thisHeight - 10;

            BodyContent.Height = GroundHeight - (GridToolbox.ActualHeight + bodyTitleSpace + bottomSpace);
            txtContent.Height = GroundHeight - 5;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveNote();
        }

        private void SaveNote()
        {
            var textRange = new TextRange(
                txtContent.Document.ContentStart,
                txtContent.Document.ContentEnd
            );

            var xmlDocument = textRange.GetValueByName<string>("System.Windows.Documents.ITextRange.Xml");
            if (_processingNote == null) _processingNote = new Note();

            _processingNote.Content = xmlDocument;
            _processingNote.Title = !string.IsNullOrEmpty(txtTitle.Text) ? txtTitle.Text : DateTime.Now.ToString("yyyy/MM/dd - hh:mm:ss");

            switch (Mode)
            {
                case EditingMode.AddNew:
                    {
                        if (string.IsNullOrEmpty(txtTitle.Text.Trim()) && string.IsNullOrEmpty(textRange.Text.Trim())) return;
                        _processingNote.NoteStatusCode = NoteStatus.New.ToString();
                        _processingNote.CreateDate = DateTime.Now;
                        _processingNote.Password = !string.IsNullOrEmpty(txtPasword.Password) ? CryptoHelper.Encrypt(txtPasword.Password) : string.Empty;
                    }
                    break;

                case EditingMode.Edit:
                    {
                        if (string.IsNullOrEmpty(OriginalNote?.Password) && !string.IsNullOrEmpty(txtPasword.Password))
                        {
                            _processingNote.Password = CryptoHelper.Encrypt(txtPasword.Password);
                        }
                    }
                    break;

                case EditingMode.ReadOnly:
                    break;
            }

            var message = new SaveNoteMessage
            {
                Note = _processingNote,
                Mode = Mode
            };
            ReferrencesHelper.SendMessage(message);
        }

        private void lblClose_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowsHelper.CloseWindow(WindowId);
        }

        private void ToolStripButtonStrikeout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TextRange range = new TextRange(txtContent.Selection.Start, txtContent.Selection.End);

            TextDecorationCollection tdc = (TextDecorationCollection)txtContent.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (tdc == null || !tdc.Equals(TextDecorations.Strikethrough))
            {
                tdc = TextDecorations.Strikethrough;
            }
            else
            {
                tdc = new TextDecorationCollection();
            }
            range.ApplyPropertyValue(Inline.TextDecorationsProperty, tdc);
        }

        private void ToolStripButtonAlignLeft_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ToolStripButtonAlignLeft.IsChecked == true)
            {
                ToolStripButtonAlignCenter.IsChecked = false;
                ToolStripButtonAlignRight.IsChecked = false;
            }
        }

        private void ToolStripButtonAlignCenter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ToolStripButtonAlignCenter.IsChecked == true)
            {
                ToolStripButtonAlignLeft.IsChecked = false;
                ToolStripButtonAlignRight.IsChecked = false;
            }
        }

        private void ToolStripButtonAlignRight_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ToolStripButtonAlignRight.IsChecked == true)
            {
                ToolStripButtonAlignLeft.IsChecked = false;
                ToolStripButtonAlignCenter.IsChecked = false;
            }
        }

        private void SetContent(Note note)
        {
            txtContent.Document.Blocks.Clear();
            txtTitle.Text = note.Title;
            txtPasword.Password = note.Password;

            MemoryStream stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(note.Content));
            txtContent.Selection.Load(stream, System.Windows.DataFormats.Xaml);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MoveWindow(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _processingNote = OriginalNote.Clone();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape) this.Close();
                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    SaveNote();
                    Mode = EditingMode.Edit;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitControlSizes();
        }

        private void BoxFontFamily_DropDownClosed(object sender, EventArgs e)
        {
            string fontName = (string)boxFontFamily.SelectedItem;

            if (fontName != null)
            {
                txtContent.Selection.ApplyPropertyValue(System.Windows.Controls.RichTextBox.FontFamilyProperty, fontName);
                txtContent.Focus();
            }
        }

        private void BoxFontSize_DropDownClosed(object sender, EventArgs e)
        {
            string fontHeight = (string)boxFontSize.SelectedItem;

            if (fontHeight != null)
            {
                txtContent.Selection.ApplyPropertyValue(System.Windows.Controls.RichTextBox.FontSizeProperty, fontHeight);
                txtContent.Focus();
            }
        }

        private void TxtContent_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectionRange = new TextRange(txtContent.Selection.Start, txtContent.Selection.End);

            // Update Font Family
            var fontFamily = selectionRange.GetPropertyValue(FlowDocument.FontFamilyProperty);
            boxFontFamily.SelectedValue = fontFamily.ToString();

            // Update font size
            var fontSizeObj = selectionRange.GetPropertyValue(FlowDocument.FontSizeProperty);
            if (fontSizeObj?.ToString()?.IsNumeric() ?? false)
            {
                var fontSize = fontSizeObj?.ToString().ToInt32() ?? DefaultFontSize;
                boxFontSize.SelectedValue = fontSize.ToString();
            }

            // Update Bold state
            var fontWeightProperty = selectionRange.GetPropertyValue(FontWeightProperty);
            if (fontWeightProperty.ToString() == "Bold")
            {
                ToolStripButtonBold.IsChecked = true;
            }
            else
            {
                ToolStripButtonBold.IsChecked = false;
            }

            // Update Itatic state
            var fontStyleProperty = selectionRange.GetPropertyValue(FontStyleProperty);
            if (fontStyleProperty.ToString() == "Italic")
            {
                ToolStripButtonItalic.IsChecked = true;
            }
            else
            {
                ToolStripButtonItalic.IsChecked = false;
            }

            var textDecorationsProperty = selectionRange.GetPropertyValue(Inline.TextDecorationsProperty);
            // Update Underline state
            if (textDecorationsProperty == TextDecorations.Underline)
            {
                ToolStripButtonUnderline.IsChecked = true;
            }
            else
            {
                ToolStripButtonUnderline.IsChecked = false;
            }

            // Update Strikethrough state
            if (textDecorationsProperty == TextDecorations.Strikethrough)
            {
                ToolStripButtonStrikeout.IsChecked = true;
            }
            else
            {
                ToolStripButtonStrikeout.IsChecked = false;
            }

            // Update Text Aligment
            var textAlignmentProperty = selectionRange.GetPropertyValue(FlowDocument.TextAlignmentProperty).ToString();
            if (textAlignmentProperty == "Left")
            {
                ToolStripButtonAlignLeft.IsChecked = true;
                ToolStripButtonAlignCenter.IsChecked = false;
                ToolStripButtonAlignRight.IsChecked = false;
            }

            if (textAlignmentProperty == "Center")
            {
                ToolStripButtonAlignLeft.IsChecked = false;
                ToolStripButtonAlignCenter.IsChecked = true;
                ToolStripButtonAlignRight.IsChecked = false;
            }

            if (textAlignmentProperty == "Right")
            {
                ToolStripButtonAlignLeft.IsChecked = false;
                ToolStripButtonAlignCenter.IsChecked = false;
                ToolStripButtonAlignRight.IsChecked = true;
            }

            //Update Text Color
            var textColor = selectionRange.GetPropertyValue(FlowDocument.ForegroundProperty).ToString();
            if (!string.IsNullOrEmpty(textColor) && textColor.StartsWith("#"))
            {
                try
                {
                    GridContentColor.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(textColor);
                }
                catch
                {
                    Console.WriteLine();
                }
            }
        }

        private void ToolBarCommands_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        private void BodyContent_Loaded(object sender, RoutedEventArgs e)
        {
            if (Mode == EditingMode.Edit)
            {
                SetContent(_processingNote);
            }
            txtContent.Selection.Select(txtContent.Document.ContentStart, txtContent.Document.ContentStart);

            InitControlSizes();
            this.InitializeComponent();
        }

        private void ImgDragMove_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MoveWindow(e);
        }

        private void MoveWindow(MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }

        private void GridContentColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var selectedColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));

                GridContentColor.Background = selectedColorBrush;

                TextRange range = new TextRange(txtContent.Selection.Start, txtContent.Selection.End);
                range.ApplyPropertyValue(FlowDocument.ForegroundProperty, selectedColorBrush);
            }
        }

        private void BodyContent_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0) DoZoomTextbox(0.1);
                else if (e.Delta < 0) DoZoomTextbox(-0.1);
            }
        }

        /// TO DO: Zoom make textbox move -> no more moving
        private void DoZoomTextbox(double changeValue)
        {
            var newValue = TextBoxScale + changeValue;
            if (newValue <= 0) newValue = 0.1;
            TextBoxScale = newValue;
            txtContent.LayoutTransform.SetValueByName("ScaleX", newValue);
            txtContent.LayoutTransform.SetValueByName("ScaleY", newValue);
        }
    }
}
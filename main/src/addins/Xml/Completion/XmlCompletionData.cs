//
// MonoDevelop XML Editor
//
// Copyright (C) 2005-2007 Matthew Ward
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Gdk;
using Mono.TextEditor;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Xml.Editor;

namespace MonoDevelop.Xml.Completion
{
	/// <summary>
	/// Holds the text for  namespace, child element or attribute 
	/// autocomplete (intellisense).
	/// </summary>
	public class XmlCompletionData : CompletionData
	{
		string text;
		DataType dataType = DataType.XmlElement;
		string description = String.Empty;
		
		/// <summary>
		/// The type of text held in this object.
		/// </summary>
		public enum DataType {
			XmlElement = 1,
			XmlAttribute = 2,
			NamespaceUri = 3,
			XmlAttributeValue = 4
		}
		
		public XmlCompletionData(string text)
			: this(text, String.Empty, DataType.XmlElement)
		{
		}
		
		public XmlCompletionData(string text, string description)
			: this(text, description, DataType.XmlElement)
		{
		}

		public XmlCompletionData(string text, DataType dataType)
			: this(text, String.Empty, dataType)
		{
		}		

		public XmlCompletionData(string text, string description, DataType dataType)
		{
			this.text = text;
			this.description = description;
			this.dataType = dataType;  
		}		
		
		public DataType XmlCompletionDataType {
			get { return dataType; }
		}
		
		public override IconId Icon {
			get { return base.Icon.IsNull ? (IconId)Gtk.Stock.GoForward : base.Icon; }
		}
		
		public override string DisplayText {
			get { return text; }
		}
		
		public override string CompletionText {
			get {
				if(!XmlEditorOptions.AutoInsertFragments)
					return text;
				
				if ((dataType == DataType.XmlElement) || (dataType == DataType.XmlAttributeValue))
					return text;
				
				if (dataType == DataType.NamespaceUri)
					return String.Concat("\"", text, "\"");
				
				// Move caret in the middle of the attribute quotes.
				return String.Concat (text, "=\"|\"");
			}
		}

		public override void InsertCompletionText (CompletionListWindow window, ref KeyActions ka, Gdk.Key closeChar, char keyChar, ModifierType modifier)
		{
			if (XmlEditorOptions.AutoInsertFragments && dataType == DataType.XmlAttribute) {
				var textEditorDataProvier = window.CompletionWidget as ITextEditorDataProvider;
				var textBuffer = window.CompletionWidget as ITextBuffer;
				base.InsertCompletionText (window, ref ka, closeChar, keyChar, modifier);
				if (textEditorDataProvier != null && textBuffer != null)
					textEditorDataProvier.GetTextEditorData ().SetSkipChar (textBuffer.CursorPosition, '"');
				IdeApp.CommandService.DispatchCommand (TextEditorCommands.ShowCompletionWindow);
				ka &= ~KeyActions.CloseWindow;
			} else {
				base.InsertCompletionText (window, ref ka, closeChar, keyChar, modifier);
			}
		}
		
		/// <summary>
		/// Returns the xml item's documentation as retrieved from
		/// the xs:annotation/xs:documentation element.
		/// </summary>
		public override string Description {
			get { return description; }
		}
	}
}

//
// JumperDecorator.cs
//
// Author:
//       Artem Kovalev <nomit007@gmail.com>
//
// Copyright (c) 2013 Nomit
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using MonoDevelop.Ide.Gui;
using Mono.TextEditor;

namespace MonoDevelop.MouseJumper
{
    public class JumperDecorator
    {
        UnderlineTextSegmentMarker marker;
        TextDocument textDocument;

        public void Draw(Document document, double mouseX, double mouseY)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            textDocument = document.Editor.Document;

            var editor = document.Editor;

            var location = editor.Parent.PointToLocation(mouseX, mouseY);
            var offset = editor.LocationToOffset(location);

            var startWord = editor.FindCurrentWordStart(offset);
            var endWord = editor.FindCurrentWordEnd(offset);

            marker = new UnderlineTextSegmentMarker(
                editor.ColorStyle.KeywordNamespace.Foreground, 
                new TextSegment(startWord, endWord - startWord));
            marker.IsVisible = true;
            marker.Wave = false;


            textDocument.AddMarker(marker);
            editor.Parent.QueueDraw();
        }

        public void Clean()
        {
            try
            {
                if (textDocument != null && marker != null)
                {    
                    textDocument.RemoveMarker(marker);
                }
            }
            finally
            {
                marker = null;
            }

        }
    }
}


//
// StartupHandler.cs
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
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using Gtk;
using MonoDevelop.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using Gdk;
using System;
using Mono.Addins;

namespace MonoDevelop.MouseJumper
{
    [Extension(Path = "/MonoDevelop/Ide/StartupHandlers", NodeName = "Class")]
    public class StartupHandler : CommandHandler
    {
        readonly JumperDecorator decorator = new JumperDecorator();

        protected override void Run()
        {
            base.Run();
            IdeApp.Workbench.DocumentOpened += HandleDocumentOpened;
            IdeApp.Workbench.DocumentClosed += HandleDocumentClosed;
        }

        void HandleDocumentOpened(object sender, DocumentEventArgs e)
        {
            e.Document.Editor.Parent.MotionNotifyEvent += HandleMotionNotifyEvent;
            e.Document.Editor.Parent.ButtonReleaseEvent += HandleButtonReleaseEvent;
        }

        void HandleDocumentClosed(object sender, DocumentEventArgs e)
        {
            e.Document.Editor.Parent.MotionNotifyEvent -= HandleMotionNotifyEvent;
            e.Document.Editor.Parent.ButtonReleaseEvent -= HandleButtonReleaseEvent;
        }

        void HandleMotionNotifyEvent(object o, MotionNotifyEventArgs args)
        {
            decorator.Clean();

            var doc = IdeApp.Workbench.ActiveDocument;
            if (doc == null || !IsCtrlPush(args.Event.State))
                return;

            decorator.Draw(doc, args.Event.X, args.Event.Y);

        }

        static bool IsCtrlPush(ModifierType state)
        {
            return (state & ModifierType.ControlMask) == ModifierType.ControlMask;
        }

        void HandleButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            var doc = IdeApp.Workbench.ActiveDocument;

            if (args.Event.Button != 1
                || !IsCtrlPush(args.Event.State)
                || doc == null)
                return;

            ResolveResult resolveResult;
            var item = CurrentRefactoryOperationsHandler.GetItem(doc, out resolveResult);

            if (item == null)
                return;

            if (item is INamedElement)
                IdeApp.ProjectOperations.JumpToDeclaration((INamedElement)item);
            else if (item is IVariable)
                IdeApp.ProjectOperations.JumpToDeclaration((IVariable)item);
        }
    }
}

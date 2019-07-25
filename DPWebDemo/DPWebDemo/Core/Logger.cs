using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DPWebDemo
{
    public class Logger : TraceListener
    {
        private TextBox textBox;

        public Logger(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public override void Write(string message)
        {
            textBox.AppendText(message);
            textBox.ScrollToEnd();
        }

        public override void WriteLine(string message)
        {
            textBox.AppendText(message);
            textBox.AppendText(Environment.NewLine);
            textBox.ScrollToEnd();
        }

        public static IDisposable TraceMehtod([CallerMemberName] string methodName = null)
        {
            return new SafeLogger(methodName);
        }

        private class SafeLogger : IDisposable
        {
            private string methodName;

            public SafeLogger(string methodName)
            {
                this.methodName = methodName;
                Trace.WriteLine("Begin: " + methodName);
            }

            public void Dispose()
            {
                Trace.WriteLine("End: " + methodName);
            }
        }

    }
}

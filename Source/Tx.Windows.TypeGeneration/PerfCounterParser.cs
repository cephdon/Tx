﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;

namespace Tx.Windows
{
    public class PerfCounterParser
    {
        public static Dictionary<string, string> Parse(string dataSource)
        {
            var allCounters = PdhUtils.Parse(dataSource);
            var generated = new Dictionary<string, string>();

            foreach (string counterSet in allCounters.Keys)
            {
                string setName = NameUtils.CreateIdentifier(counterSet);

                StringBuilder sb = new StringBuilder("// This code was generated by EtwEventTypeGen");
                sb.AppendLine(setName);
                sb.Append("namespace Tx.Windows.Counters.");
                sb.AppendLine(setName);
                sb.AppendLine();
                sb.AppendLine("{");

                foreach (string counter in allCounters[counterSet])
                {
                    EmitCounter(counterSet, counter, ref sb);
                }

                sb.AppendLine("}");
                generated.Add(setName, sb.ToString());
            }

            return generated;
        }

        static void EmitCounter(string counterSet, string counter, ref StringBuilder sb)
        {
            sb.Append("    [PerformanceCounter(\"");
            sb.Append(counterSet);
            sb.Append("\", \"");
            sb.Append(counter);
            sb.AppendLine("\")]");

            string c = counter
                .Replace("%", "Percent")
                .Replace("#", "Count of")
                .Replace("(","")
                .Replace(")","")
                .Trim();

            string className = NameUtils.CreateIdentifier(c).Trim('_');
            sb.Append("    public class ");
            sb.Append(className);
            sb.AppendLine(" : PerformanceSample");
            sb.AppendLine("    {");
            sb.Append("        public ");
            sb.Append(className);
            sb.AppendLine("(PerformanceSample other)");
            sb.AppendLine("        : base(other)");
            sb.AppendLine("        { }");
            sb.AppendLine("    }");
            sb.AppendLine();
        }
    }
}
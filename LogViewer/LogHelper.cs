using MissionPlanner.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewer
{
    public interface ILogHelper
    {
        public void LoadLog(string filename);
        public IEnumerable<ControllerMessage> GetMessages();
        public IEnumerable<ParameterNode> GetParametersTree();
        public LogData GetData(string type, string name, string instance);
    }

    public class ControllerMessage
    {
        public string Message { get; set; }
        public DateTime MessageTime { get; set; }

        public override string ToString()
        {
            return $"{MessageTime.ToLongTimeString()}: {Message}";
        }
    }

    public class ParameterNode
    {
        public string Name { get; set; }
        public List<ParameterNode> Nodes { get; set; }
        public ParameterNode()
        {
            Nodes = new List<ParameterNode>();
        }

        public ParameterNode (string name, string type, string instance) : this()
        {
            Name = name;
            Type = type;
            Instance = instance;
        }
        public override string ToString()
        {
            return Name;
        }

        public string Type { get; set; }
        public string Instance { get; set; }
    }

    public class ParameterValue
    {
        public ParameterValue(double value, DateTime time, int lineNum)
        {
            Value = value;
            Time = time;
            LineNum = lineNum;
        }

        public int LineNum { get; set; }
        public DateTime Time { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class LogData
    {
        private double[] values;
        private double[] times;
        private double[] lines;
        public LogData(IEnumerable<ParameterValue> rawData)
        {
            RawData = rawData;
        }

        public IEnumerable<ParameterValue> RawData { get; set; }

        public double[] GetValues()
        {
            if (values == null)
            {
                values = RawData.Select(r => r.Value).ToArray();
            }
            return values;
        }

        public double[] GetTimes()
        {
            if (times == null)
            {
                times = RawData.Select(r => r.Time.ToOADate()).ToArray();
            }
            return times;
        }

        public double[] GetLines()
        {
            if (lines == null)
            {
                lines = RawData.Select(r => (double)r.LineNum).ToArray();
            }
            return lines;
        }
    }

    public class LogHelper : ILogHelper
    {

        DFLogBuffer logdata;
        DFLog dflog;
        private List<ControllerMessage> messagesCache = new List<ControllerMessage>();

        private List<ParameterNode> parameterTree = null;

        public LogData GetData(string type, string name, string instance)
        {
            List<ParameterValue> result = new List<ParameterValue>();

            int col = dflog.FindMessageOffset(type, name);
            if (col == -1)
            {
                throw new Exception("Ololol");
            }

            int error = 0;

            long dataCounter = 0; // row counter
            double value_prev = 0;

            foreach (var item in logdata.GetEnumeratorType(type))
            {
                // same message type, with no instance, or same message with instance
                if (item.msgtype == type && (instance == "" || item.instance == instance))
                {
                    try
                    {
                        double value = double.Parse(item.items[col],
                            System.Globalization.CultureInfo.InvariantCulture);

                        // abandon realy bad data
                        if (Math.Abs(value) > 9.15e8)
                        {
                            dataCounter++;
                            continue;
                        }

                        /*if (dataModifier.IsValid())
                        {
                            if ((dataCounter != 0) && Math.Abs(value - value_prev) > 1e5)
                            {
                                // there is a glitch in the data, reject it by replacing it with the previous value
                                value = value_prev;
                            }

                            value_prev = value;

                            if (dataModifier.doOffsetFirst)
                            {
                                value += dataModifier.offset;
                                value *= dataModifier.scalar;
                            }
                            else
                            {
                                value *= dataModifier.scalar;
                                value += dataModifier.offset;
                            }
                        }*/

                        result.Add(new ParameterValue(value, item.time, item.lineno));
                    }
                    catch
                    {
                        error++;
                        //log.Info("Bad Data : " + type + " " + col + " " + a);
                        if (error >= 500)
                        {
                            //CustomMessageBox.Show("There is to much bad data - failing");
                            break;
                        }
                    }
                }

                dataCounter++;
            }
            return new LogData(result);
        }

        public IEnumerable<ControllerMessage> GetMessages()
        {
            if (!dflog.logformat.ContainsKey("MSG"))
                return messagesCache;

            foreach (DFLog.DFItem item in logdata.GetEnumeratorType("MSG"))
            {
                if (item.msgtype == "MSG")
                {

                    int index = dflog.FindMessageOffset("MSG", "Message");
                    if (index == -1)
                    {
                        continue;
                    }
                    string message = item.items[index].ToString().Trim();
                    messagesCache.Add(new ControllerMessage() { Message = message, MessageTime = item.time });
                }
            }
            return messagesCache;
        }

        public IEnumerable<ParameterNode> GetParametersTree()
        {
            if (parameterTree != null)
            {
                return parameterTree;
            }
            parameterTree = new List<ParameterNode>();
            List<string> parameters = logdata.SeenMessageTypes;
            var sorted = new SortedList(dflog.logformat);
            foreach (DFLog.Label item in sorted.Values)
            {
                if (parameters.Contains(item.Name))
                {
                    ParameterNode parameterNode = new ParameterNode(item.Name, null, null);
                    var instance = logdata.InstanceType.ContainsKey(item.Id);
                    if (instance)
                    {
                        List<string> value = logdata.InstanceType[item.Id].value;
                        if (value.Count > 1)
                        {
                            foreach (var instanceinfo in value)
                            {
                                var instNode = new ParameterNode(instanceinfo, null, null);
                                parameterNode.Nodes.Add(instNode);
                                foreach (var item1 in item.FieldNames)
                                {
                                    instNode.Nodes.Add(new ParameterNode(item1, item.Name, instanceinfo));
                                }
                            }
                        }
                        else
                        {
                            foreach (var item1 in item.FieldNames)
                            {
                                parameterNode.Nodes.Add(new ParameterNode(item1, item.Name, "0"));
                            }
                        }
                    }
                    else
                    {
                        foreach (var item1 in item.FieldNames)
                        {
                            parameterNode.Nodes.Add(new ParameterNode(item1, item.Name, ""));
                        }
                    }
                    parameterTree.Add(parameterNode);
                }
            }
            return parameterTree;
        }

        public void LoadLog(string filename)
        {
            try
            {
                logdata = new DFLogBuffer(filename);
                dflog = logdata.dflog;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

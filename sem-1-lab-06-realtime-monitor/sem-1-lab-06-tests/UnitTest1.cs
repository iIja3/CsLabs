using System;
using System.Linq;
using NUnit.Framework;
using lab6;

namespace lab6.Tests
{
    [TestFixture]
    public class MonitorTests
    {
        [Test]
        public void RenderGraph_HasTwentyColumns()
        {
            Assert.That(Monitor.RenderGraph(new double[] { 0, 50, 100 }).Length, Is.EqualTo(20));
        }


        [Test]
        public void RenderGraph_EmptyInput_ReturnsTwentyBlankColumns()
        {
            var columns = Monitor.RenderGraph(Array.Empty<double>());

            Assert.That(columns, Has.Length.EqualTo(20));
            Assert.That(columns, Has.All.EqualTo(new string(' ', Program.GraphHeight)));
        }

        [Test]
        public void RenderGraph_NullInput_ReturnsTwentyColumns()
        {
            var columns = Monitor.RenderGraph(null!);

            Assert.That(columns, Has.Length.EqualTo(20));
        }

        [Test]
        public void RenderGraph_SingleValue_PlacedInLastColumn()
        {
            var columns = Monitor.RenderGraph(new double[] { 100 });

            Assert.That(columns, Has.Length.EqualTo(20));
            Assert.That(columns[19], Is.EqualTo(new string('#', Program.GraphHeight)));
            for (int i = 0; i < 19; i++)
                Assert.That(columns[i], Is.EqualTo(new string(' ', Program.GraphHeight)));
        }


        [Test]
        public void RenderGraph_ZeroValue_IsEmptyColumn()
        {
            var columns = Monitor.RenderGraph(new double[] { 0 });

            Assert.That(columns[19], Is.EqualTo(new string(' ', Program.GraphHeight)));
        }

        [Test]
        public void RenderGraph_HundredValue_IsFullColumn()
        {
            var columns = Monitor.RenderGraph(new double[] { 100 });

            Assert.That(columns[19], Is.EqualTo(new string('#', Program.GraphHeight)));
        }

        [Test]
        public void RenderGraph_ClampsOutOfRangeValues()
        {
            var columns = Monitor.RenderGraph(new double[] { -50, 150 });

            Assert.That(columns[18], Is.EqualTo(new string(' ', Program.GraphHeight)));
            Assert.That(columns[19], Is.EqualTo(new string('#', Program.GraphHeight)));
        }


        [Test]
        public void RenderGraph_MoreThanTwentyValues_KeepsLastTwenty()
        {
            var data = new double[25];
            for (int i = 0; i < 25; i++) data[i] = 100;

            var columns = Monitor.RenderGraph(data);

            Assert.That(columns, Has.Length.EqualTo(20));
            Assert.That(columns, Has.All.EqualTo(new string('#', Program.GraphHeight)));
        }

        [Test]
        public void RenderGraph_ExactlyTwentyValues_AllColumnsFilled()
        {
            var data = Enumerable.Repeat(100.0, 20).ToArray();

            var columns = Monitor.RenderGraph(data);

            Assert.That(columns, Has.Length.EqualTo(20));
            Assert.That(columns, Has.All.EqualTo(new string('#', Program.GraphHeight)));
        }

        [Test]
        public void RenderGraph_LastValuesWin_WhenOverflow()
        {
            var data = new double[21];
            data[0] = 0;
            for (int i = 1; i < 21; i++) data[i] = 100;

            var columns = Monitor.RenderGraph(data);

            Assert.That(columns, Has.All.EqualTo(new string('#', Program.GraphHeight)));
        }


        [Test]
        public void GraphWidth_IsTwenty()
        {
            Assert.That(Program.GraphWidth, Is.EqualTo(20));
        }

        [Test]
        public void GraphHeight_IsPositive()
        {
            Assert.That(Program.GraphHeight, Is.GreaterThan(0));
        }

        [Test]
        public void Monitor_IsDerivedFromServerMonitor()
        {
            Assert.That(typeof(ServerMonitor).IsAssignableFrom(typeof(Monitor)), Is.True);
        }

        [Test]
        public void ServerMonitor_RenderGraph_InstanceMethodWorks()
        {
            var monitor = new ServerMonitor();

            Assert.DoesNotThrow(() => monitor.RenderGraph());
        }


        [Test]
        public void ServerMonitor_DataReceived_RaisesEvent()
        {
            var monitor = new ServerMonitor();
            double? capturedCpu = null, capturedRam = null, capturedTemp = null;

            monitor.DataReceived += (cpu, ram, temp) =>
            {
                capturedCpu = cpu;
                capturedRam = ram;
                capturedTemp = temp;
            };

            var generate = typeof(ServerMonitor)
                .GetMethod("GenerateData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.That(generate, Is.Not.Null);
            generate!.Invoke(monitor, null);

            Assert.That(capturedCpu, Is.Not.Null);
            Assert.That(capturedRam, Is.Not.Null);
            Assert.That(capturedTemp, Is.Not.Null);
            Assert.That(capturedCpu!.Value, Is.InRange(0.0, 100.0));
            Assert.That(capturedRam!.Value, Is.InRange(10.0, 100.0));
            Assert.That(capturedTemp!.Value, Is.InRange(30.0, 95.0));
        }
    }
}
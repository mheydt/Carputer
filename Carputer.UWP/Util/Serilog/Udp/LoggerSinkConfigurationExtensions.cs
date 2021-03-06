﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Carputer.UWP.Util.Serilog.Udp
{
    /// <summary>
    /// Adds the WriteTo.Udp() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerSinkConfigurationExtensions
    {
        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// Adds a sink that sends log events as UDP packages over the network.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="remoteAddress">
        /// The <see cref="IPAddress"/> of the remote host or multicast group to which the UDP
        /// client should sent the logging event.
        /// </param>
        /// <param name="remotePort">
        /// The TCP port of the remote host or multicast group to which the UDP client should sent
        /// the logging event.
        /// </param>
        /// <param name="localPort">
        /// The TCP port from which the UDP client will communicate. The default is 0 and will
        /// cause the UDP client not to bind to a local port.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        /// The minimum level for events passed through the sink. The default is
        /// <see cref="LevelAlias.Minimum"/>.
        /// </param>
        /// <param name="outputTemplate">
        /// A message template describing the format used to write to the sink. The default is
        /// "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}".
        /// </param>
        /// <param name="formatProvider">
        /// Supplies culture-specific formatting information, or null.
        /// </param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration Udp(
            this LoggerSinkConfiguration sinkConfiguration,
            IPAddress remoteAddress,
            int remotePort,
            int localPort = 0,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputTemplate == null)
                throw new ArgumentNullException(nameof(outputTemplate));

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return Udp(
                sinkConfiguration,
                remoteAddress,
                remotePort,
                formatter,
                localPort,
                restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that sends log events as UDP packages over the network.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="remoteAddress">
        /// The <see cref="IPAddress"/> of the remote host or multicast group to which the UDP
        /// client should sent the logging event.
        /// </param>
        /// <param name="remotePort">
        /// The TCP port of the remote host or multicast group to which the UDP client should sent
        /// the logging event.
        /// </param>
        /// <param name="formatter">
        /// Controls the rendering of log events into text, for example to log JSON. To control
        /// plain text formatting, use the overload that accepts an output template.
        /// </param>
        /// <param name="localPort">
        /// The TCP port from which the UDP client will communicate. The default is 0 and will
        /// cause the UDP client not to bind to a local port.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        /// The minimum level for events passed through the sink. The default is
        /// <see cref="LevelAlias.Minimum"/>.
        /// </param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration Udp(
            this LoggerSinkConfiguration sinkConfiguration,
            IPAddress remoteAddress,
            int remotePort,
            ITextFormatter formatter,
            int localPort = 0,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));

            var client = new UdpClientWrapper(localPort, remoteAddress);
            var sink = new UdpSink(client, remoteAddress, remotePort, formatter);
            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
